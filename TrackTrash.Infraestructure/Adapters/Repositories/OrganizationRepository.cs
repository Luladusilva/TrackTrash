using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class OrganizationRepository(TrackTrashDBContext context) : GenericRepository<Organization>(context), IOrganizationRepository
{
    public async Task<Organization?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.Location)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
