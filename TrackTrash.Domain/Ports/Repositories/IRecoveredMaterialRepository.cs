using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IRecoveredMaterialRepository : IGenericRepository<RecoveredMaterial>
{
    Task<IReadOnlyList<RecoveredMaterial>> GetByPickupServiceIdAsync(long pickupServiceId, CancellationToken cancellationToken = default);
}
