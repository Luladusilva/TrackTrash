using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class PickupServiceRepository(TrackTrashDBContext context) : GenericRepository<PickupService>(context), IPickupServiceRepository
{
    public async Task<PickupService?> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .SingleOrDefaultAsync(x => x.CollectRequestId == collectRequestId, cancellationToken);
    }

    public async Task<IReadOnlyList<PickupService>> GetByGathererIdAsync(long gathererId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.CollectRequest).ThenInclude(x => x.Location)
            .Where(x => x.GathererId == gathererId)
            .OrderByDescending(x => x.AssignedAt).ThenByDescending(x => x.PickupServiceId)
            .ToListAsync(cancellationToken);
    }
}
