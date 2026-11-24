using MediatR;
using TrackTrash.Application.UseCases.CollectRequests;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Commands;

/// <summary>
/// Starts, completes or cancels a pickup service and keeps its collect request in sync:
/// InProgress starts the request, Completed closes it, and Cancelled returns it to Pending so it can be reassigned.
/// </summary>
public sealed record ChangePickupServiceStatusCommand : IRequest<long>
{
    public long PickupServiceId { get; set; }

    public PickupServiceStatus NewStatusId { get; set; }

    public long? ChangedByUserId { get; set; }

    public string? Observations { get; set; }
}

internal sealed class ChangePickupServiceStatusCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePickupServiceStatusCommand, long>
{
    private static readonly Dictionary<PickupServiceStatus, PickupServiceStatus[]> AllowedTransitions = new()
    {
        [PickupServiceStatus.Pending] = [PickupServiceStatus.Assigned, PickupServiceStatus.Cancelled],
        [PickupServiceStatus.Assigned] = [PickupServiceStatus.InProgress, PickupServiceStatus.Cancelled],
        [PickupServiceStatus.InProgress] = [PickupServiceStatus.Completed, PickupServiceStatus.Cancelled],
        [PickupServiceStatus.Completed] = [],
        [PickupServiceStatus.Cancelled] = []
    };

    public async Task<long> Handle(ChangePickupServiceStatusCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                   ?? throw new KeyNotFoundException("Pickup service not found.");

        if (!AllowedTransitions[data.StatusId].Contains(request.NewStatusId))
        {
            throw new InvalidOperationException($"Cannot change a pickup service from {data.StatusId} to {request.NewStatusId}.");
        }

        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(data.CollectRequestId, cancellationToken)
                             ?? throw new KeyNotFoundException("Collect request not found.");

        var now = DateTime.UtcNow;
        var requestStatus = request.NewStatusId switch
        {
            PickupServiceStatus.InProgress => CollectRequestStatus.InProgress,
            PickupServiceStatus.Completed => CollectRequestStatus.Completed,
            PickupServiceStatus.Cancelled => CollectRequestStatus.Pending,
            _ => collectRequest.StatusId
        };

        if (request.NewStatusId == PickupServiceStatus.InProgress) data.StartedAt = now;
        if (request.NewStatusId == PickupServiceStatus.Completed) data.CompletedAt = now;
        data.StatusId = request.NewStatusId;
        if (request.Observations is not null) data.Observations = request.Observations;
        unitOfWork.PickupServices.Update(data);

        if (requestStatus != collectRequest.StatusId && CollectRequestStatusRules.CanChange(collectRequest.StatusId, requestStatus))
        {
            await CollectRequestStatusRules.ApplyAsync(
                unitOfWork, collectRequest, requestStatus, request.ChangedByUserId, $"Pickup service {request.NewStatusId}.", cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.PickupServiceId;
    }
}
