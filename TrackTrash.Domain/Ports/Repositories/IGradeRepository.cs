using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IGradeRepository : IGenericRepository<Grade>
{
    Task<Grade?> GetByServiceAndUserAsync(long pickupServiceId, long userId, CancellationToken cancellationToken = default);
}
