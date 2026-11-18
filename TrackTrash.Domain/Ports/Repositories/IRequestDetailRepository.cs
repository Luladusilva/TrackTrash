using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IRequestDetailRepository : IGenericRepository<RequestDetail>
{
    Task<IReadOnlyList<RequestDetail>> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default);
}
