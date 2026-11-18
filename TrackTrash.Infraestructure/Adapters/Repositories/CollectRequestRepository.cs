using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class CollectRequestRepository(TrackTrashDBContext context) : GenericRepository<CollectRequest>(context), ICollectRequestRepository
{
    public async Task<CollectRequest?> GetWithDetailsAsync(long collectRequestId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.Location)
            .Include(x => x.RequestDetails).ThenInclude(x => x.ResidueType)
            .Include(x => x.PickupService)
            .SingleOrDefaultAsync(x => x.CollectRequestId == collectRequestId, cancellationToken);
    }

    public async Task<IReadOnlyList<CollectRequest>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.RequestedAt).ThenByDescending(x => x.CollectRequestId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CollectRequest>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.Location)
            .Include(x => x.RequestDetails).ThenInclude(x => x.ResidueType)
            .Where(x => x.StatusId == CollectRequestStatus.Pending && x.PickupService == null)
            .OrderBy(x => x.ScheduledDate).ThenBy(x => x.ScheduledTime).ThenBy(x => x.CollectRequestId)
            .ToListAsync(cancellationToken);
    }
}
