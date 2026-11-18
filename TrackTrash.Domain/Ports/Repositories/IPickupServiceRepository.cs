using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IPickupServiceRepository : IGenericRepository<PickupService>
{
    Task<PickupService?> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PickupService>> GetByGathererIdAsync(long gathererId, CancellationToken cancellationToken = default);
}
