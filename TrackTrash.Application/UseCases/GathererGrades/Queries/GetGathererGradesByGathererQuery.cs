using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.GathererGrades.Queries;

public sealed record GetGathererGradesByGathererQuery : IRequest<IReadOnlyList<GathererGradeDto>>
{
    public long GathererId { get; set; }
}

internal sealed class GetGathererGradesByGathererQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererGradesByGathererQuery, IReadOnlyList<GathererGradeDto>>
{
    public async Task<IReadOnlyList<GathererGradeDto>> Handle(GetGathererGradesByGathererQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GathererGrades.GetByGathererIdAsync(request.GathererId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
