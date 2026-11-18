using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IIndicatorRepository
{
    Task<VwGeneralIndicator?> GetGeneralAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VwResiduesByType>> GetResiduesByTypeAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VwGathererGrade>> GetGathererGradesAsync(CancellationToken cancellationToken = default);
}
