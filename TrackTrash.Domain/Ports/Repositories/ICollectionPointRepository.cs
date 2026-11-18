using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface ICollectionPointRepository : IGenericRepository<CollectionPoint>
{
    Task<IReadOnlyList<CollectionPoint>> GetActiveByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default);
}
