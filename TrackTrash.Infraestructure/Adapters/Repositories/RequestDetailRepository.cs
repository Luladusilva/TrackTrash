using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class RequestDetailRepository(TrackTrashDBContext context) : GenericRepository<RequestDetail>(context), IRequestDetailRepository
{
    public async Task<IReadOnlyList<RequestDetail>> GetByCollectRequestIdAsync(long collectRequestId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.ResidueType)
            .Where(x => x.CollectRequestId == collectRequestId)
            .OrderBy(x => x.RequestDetailId)
            .ToListAsync(cancellationToken);
    }
}
