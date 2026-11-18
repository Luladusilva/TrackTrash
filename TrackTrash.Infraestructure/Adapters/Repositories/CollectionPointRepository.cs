using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class CollectionPointRepository(TrackTrashDBContext context) : GenericRepository<CollectionPoint>(context), ICollectionPointRepository
{
    public async Task<IReadOnlyList<CollectionPoint>> GetActiveByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.Location)
            .Where(x => x.IsActive && x.ResidueTypes.Any(r => r.ResidueTypeId == residueTypeId))
            .OrderBy(x => x.Name).ThenBy(x => x.CollectionPointId)
            .ToListAsync(cancellationToken);
    }
}
