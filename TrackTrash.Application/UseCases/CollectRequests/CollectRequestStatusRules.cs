using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests;

/// <summary>
/// Single source of truth for collect request status transitions, shared by the request and pickup service commands.
/// </summary>
internal static class CollectRequestStatusRules
{
    private static readonly Dictionary<CollectRequestStatus, CollectRequestStatus[]> AllowedTransitions = new()
    {
        [CollectRequestStatus.Pending] = [CollectRequestStatus.Assigned, CollectRequestStatus.Cancelled],
        [CollectRequestStatus.Assigned] = [CollectRequestStatus.InProgress, CollectRequestStatus.Pending, CollectRequestStatus.Cancelled],
        [CollectRequestStatus.InProgress] = [CollectRequestStatus.Completed, CollectRequestStatus.Cancelled],
        [CollectRequestStatus.Completed] = [],
        [CollectRequestStatus.Cancelled] = []
    };

    public static bool CanChange(CollectRequestStatus from, CollectRequestStatus to) =>
        AllowedTransitions.TryGetValue(from, out var next) && next.Contains(to);

    /// <summary>Updates the status and appends a history row. The caller saves changes.</summary>
    public static async Task ApplyAsync(
        IUnitOfWork unitOfWork,
        CollectRequest collectRequest,
        CollectRequestStatus newStatus,
        long? userId,
        string? observations,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await unitOfWork.RequestHistories.AddAsync(new RequestHistory
        {
            CollectRequestId = collectRequest.CollectRequestId,
            UserId = userId,
            PreviousStatusId = collectRequest.StatusId,
            NewStatusId = newStatus,
            Observations = observations,
            CreatedAt = now
        }, cancellationToken);

        collectRequest.StatusId = newStatus;
        collectRequest.UpdatedAt = now;
        unitOfWork.CollectRequests.Update(collectRequest);
    }
}
