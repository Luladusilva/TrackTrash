using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IRequestHistoryRepository : IGenericRepository<RequestHistory>
{
    Task<IReadOnlyList<RequestHistory>> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default);
}
