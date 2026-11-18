using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface ICollectRequestRepository : IGenericRepository<CollectRequest>
{
    Task<CollectRequest?> GetWithDetailsAsync(long collectRequestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CollectRequest>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CollectRequest>> GetPendingAsync(CancellationToken cancellationToken = default);
}
