using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Queries;

public sealed record GetGradesQuery : IRequest<IReadOnlyList<GradeDto>>;

internal sealed class GetGradesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGradesQuery, IReadOnlyList<GradeDto>>
{
    public async Task<IReadOnlyList<GradeDto>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Grades.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
