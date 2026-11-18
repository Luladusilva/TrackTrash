using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class RequestHistoryRepository(TrackTrashDBContext context) : GenericRepository<RequestHistory>(context), IRequestHistoryRepository
{
    public async Task<IReadOnlyList<RequestHistory>> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Where(x => x.CollectRequestId == collectRequestId)
            .OrderBy(x => x.CreatedAt).ThenBy(x => x.RequestHistoryId)
            .ToListAsync(cancellationToken);
    }
}
