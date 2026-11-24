using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class GathererRepository(TrackTrashDBContext context) : GenericRepository<Gatherer>(context), IGathererRepository
{
    public Task<Gatherer?> GetProfileByIdAsync(long gathererId, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ResidueTypes)
            .SingleOrDefaultAsync(x => x.GathererId == gathererId, cancellationToken);

    public async Task<IReadOnlyList<Gatherer>> GetAllProfilesAsync(CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ResidueTypes)
            .OrderBy(x => x.GathererId)
            .ToListAsync(cancellationToken);

    public async Task<Gatherer?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ResidueTypes)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Gatherer>> GetAvailableByResidueTypeAsync(long residueTypeId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ResidueTypes)
            .Where(x => x.IsActive && x.User.IsActive && x.VerificationStatusId == VerificationStatus.Verified && x.ResidueTypes.Any(r => r.ResidueTypeId == residueTypeId))
            .OrderBy(x => x.GathererId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Gatherer>> GetAvailableForCollectRequestAsync(long collectRequestId, CancellationToken cancellationToken = default)
    {
        var residueTypeIds = await Context.CollectRequests
            .Where(x => x.CollectRequestId == collectRequestId && x.StatusId == CollectRequestStatus.Pending && x.PickupService == null)
            .SelectMany(x => x.RequestDetails.Select(detail => detail.ResidueTypeId))
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (residueTypeIds.Length == 0)
            return [];

        return await Set.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ResidueTypes)
            .Where(x => x.IsActive && x.User.IsActive && x.VerificationStatusId == VerificationStatus.Verified &&
                        x.ResidueTypes.Count(type => residueTypeIds.Contains(type.ResidueTypeId)) == residueTypeIds.Length)
            .OrderBy(x => x.GathererId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateSupportedResiduesAsync(
        long gathererId,
        long actorUserId,
        bool isAdmin,
        IReadOnlyCollection<long> residueTypeIds,
        CancellationToken cancellationToken = default)
    {
        if (residueTypeIds.Distinct().Count() != residueTypeIds.Count)
            throw new InvalidOperationException("Residue type identifiers must be unique.");

        var gatherer = await Set
            .Include(x => x.ResidueTypes)
            .SingleOrDefaultAsync(x => x.GathererId == gathererId, cancellationToken)
            ?? throw new KeyNotFoundException("Gatherer not found.");

        if (!isAdmin && gatherer.UserId != actorUserId)
            throw new UnauthorizedAccessException("You cannot edit another gatherer's supported residues.");

        var types = await Context.ResidueTypes
            .Where(x => residueTypeIds.Contains(x.ResidueTypeId))
            .ToListAsync(cancellationToken);
        if (types.Count != residueTypeIds.Count || types.Any(x => !x.IsActive || !x.IsRecyclable))
            throw new InvalidOperationException("Every selected residue type must be active and recyclable.");

        gatherer.ResidueTypes.Clear();
        foreach (var type in types) gatherer.ResidueTypes.Add(type);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
