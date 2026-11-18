using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class RecoveredMaterialRepository(TrackTrashDBContext context) : GenericRepository<RecoveredMaterial>(context), IRecoveredMaterialRepository
{
    public async Task<IReadOnlyList<RecoveredMaterial>> GetByPickupServiceIdAsync(long pickupServiceId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.ResidueType)
            .Include(x => x.CollectionPoint)
            .Where(x => x.PickupServiceId == pickupServiceId)
            .OrderBy(x => x.RecoveredMaterialId)
            .ToListAsync(cancellationToken);
    }
}
