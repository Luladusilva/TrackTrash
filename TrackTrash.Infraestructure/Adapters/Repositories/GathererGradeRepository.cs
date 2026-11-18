using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class GathererGradeRepository(TrackTrashDBContext context) : GenericRepository<GathererGrade>(context), IGathererGradeRepository
{
    public async Task<IReadOnlyList<GathererGrade>> GetByGathererIdAsync(long gathererId, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking()
            .Include(x => x.Grade)
            .Where(x => x.GathererId == gathererId)
            .OrderByDescending(x => x.Grade.CreatedAt).ThenByDescending(x => x.GathererGradeId)
            .ToListAsync(cancellationToken);
    }
}
