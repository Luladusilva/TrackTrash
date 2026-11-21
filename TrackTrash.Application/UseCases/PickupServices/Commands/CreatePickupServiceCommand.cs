using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Application.UseCases.CollectRequests;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Commands;

/// <summary>
/// Assigns a verified, active gatherer to a pending collect request.
/// </summary>
public sealed record CreatePickupServiceCommand : IRequest<long>
{
    public long CollectRequestId { get; set; }

    public long GathererId { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal CommissionPercentage { get; set; }

    public string? Observations { get; set; }

    /// <summary>User who makes the assignment, stored in the request history.</summary>
    public long? AssignedByUserId { get; set; }
}

internal sealed class CreatePickupServiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreatePickupServiceCommand, long>
{
    public async Task<long> Handle(CreatePickupServiceCommand request, CancellationToken cancellationToken)
    {
        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                             ?? throw new KeyNotFoundException("Collect request not found.");

        if (collectRequest.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be assigned.");
        }

        if (await unitOfWork.PickupServices.GetByCollectRequestIdAsync(request.CollectRequestId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("This request already has a pickup service.");
        }

        var gatherer = await unitOfWork.Gatherers.GetByIdAsync(request.GathererId, cancellationToken)
                       ?? throw new KeyNotFoundException("Gatherer not found.");

        if (!gatherer.IsActive || gatherer.VerificationStatusId != VerificationStatus.Verified)
        {
            throw new InvalidOperationException("The gatherer must be active and verified.");
        }

        var data = new PickupService
        {
            CollectRequestId = request.CollectRequestId,
            GathererId = request.GathererId,
            AssignedAt = DateTime.UtcNow,
            StatusId = PickupServiceStatus.Assigned,
            CommissionPercentage = request.CommissionPercentage,
            Observations = request.Observations
        };

        await unitOfWork.PickupServices.AddAsync(data, cancellationToken);
        await CollectRequestStatusRules.ApplyAsync(
            unitOfWork, collectRequest, CollectRequestStatus.Assigned, request.AssignedByUserId, "Gatherer assigned.", cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.PickupServiceId;
    }
}
