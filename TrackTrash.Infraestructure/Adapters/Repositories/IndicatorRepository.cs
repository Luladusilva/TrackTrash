using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class IndicatorRepository(TrackTrashDBContext context) : IIndicatorRepository
{
    protected TrackTrashDBContext Context { get; } = context;

    public async Task<VwGeneralIndicator?> GetGeneralAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<VwGeneralIndicator>().AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VwResiduesByType>> GetResiduesByTypeAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<VwResiduesByType>().AsNoTracking()
            .OrderBy(x => x.ResidueTypeId).ThenBy(x => x.Unit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VwGathererGrade>> GetGathererGradesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<VwGathererGrade>().AsNoTracking()
            .OrderByDescending(x => x.AverageScore).ThenBy(x => x.GathererId)
            .ToListAsync(cancellationToken);
    }
}
