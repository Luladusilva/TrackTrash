using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class GradeRepository(TrackTrashDBContext context) : GenericRepository<Grade>(context), IGradeRepository
{
    public async Task<Grade?> GetByServiceAndUserAsync(long pickupServiceId, long userId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .SingleOrDefaultAsync(x => x.PickupServiceId == pickupServiceId && x.UserId == userId, cancellationToken);
    }
}
