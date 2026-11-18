using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class GathererRepository(TrackTrashDBContext context) : GenericRepository<Gatherer>(context), IGathererRepository
{
    public async Task<Gatherer?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.ResidueTypes)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Gatherer>> GetAvailableByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Where(x => x.IsActive && x.User.IsActive && x.VerificationStatusId == VerificationStatus.Verified && x.ResidueTypes.Any(r => r.ResidueTypeId == residueTypeId))
            .OrderBy(x => x.GathererId)
            .ToListAsync(cancellationToken);
    }
}
