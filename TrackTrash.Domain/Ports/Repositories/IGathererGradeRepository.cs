using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IGathererGradeRepository : IGenericRepository<GathererGrade>
{
    Task<IReadOnlyList<GathererGrade>> GetByGathererIdAsync(long gathererId, CancellationToken cancellationToken = default);
}
