using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IGathererRepository : IGenericRepository<Gatherer>
{
    Task<Gatherer?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Gatherer>> GetAvailableByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default);
}
