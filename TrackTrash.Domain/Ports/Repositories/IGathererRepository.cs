using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IGathererRepository : IGenericRepository<Gatherer>
{
    Task<Gatherer?> GetProfileByIdAsync(long gathererId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Gatherer>> GetAllProfilesAsync(CancellationToken cancellationToken = default);

    Task<Gatherer?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Gatherer>> GetAvailableByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Gatherer>> GetAvailableForCollectRequestAsync(long collectRequestId, CancellationToken cancellationToken = default);

    Task UpdateSupportedResiduesAsync(long gathererId, long actorUserId, bool isAdmin, IReadOnlyCollection<long> residueTypeIds, CancellationToken cancellationToken = default);
}
