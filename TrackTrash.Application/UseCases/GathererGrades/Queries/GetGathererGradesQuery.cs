using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.GathererGrades.Queries;

public sealed record GetGathererGradesQuery : IRequest<IReadOnlyList<GathererGradeDto>>;

internal sealed class GetGathererGradesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererGradesQuery, IReadOnlyList<GathererGradeDto>>
{
    public async Task<IReadOnlyList<GathererGradeDto>> Handle(GetGathererGradesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GathererGrades.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
