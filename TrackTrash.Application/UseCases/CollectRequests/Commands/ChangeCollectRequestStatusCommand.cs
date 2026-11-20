using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

/// <summary>
/// Moves a collect request to a new status and records the change in the request history.
/// </summary>
public sealed record ChangeCollectRequestStatusCommand : IRequest<long>
{
    public long CollectRequestId { get; set; }

    public CollectRequestStatus NewStatusId { get; set; }

    /// <summary>User who performs the change, if known.</summary>
    public long? ChangedByUserId { get; set; }

    public string? Observations { get; set; }
}

internal sealed class ChangeCollectRequestStatusCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeCollectRequestStatusCommand, long>
{
    public async Task<long> Handle(ChangeCollectRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collect request not found.");

        if (!CollectRequestStatusRules.CanChange(data.StatusId, request.NewStatusId))
        {
            throw new InvalidOperationException($"Cannot change a request from {data.StatusId} to {request.NewStatusId}.");
        }

        if (request.ChangedByUserId.HasValue &&
            !await unitOfWork.Users.ExistsAsync(x => x.UserId == request.ChangedByUserId, cancellationToken))
        {
            throw new KeyNotFoundException("User not found.");
        }

        await CollectRequestStatusRules.ApplyAsync(unitOfWork, data, request.NewStatusId, request.ChangedByUserId, request.Observations, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectRequestId;
    }
}
