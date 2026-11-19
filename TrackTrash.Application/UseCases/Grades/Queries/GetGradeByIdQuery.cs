using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Queries;

public sealed record GetGradeByIdQuery : IRequest<GradeDto>
{
    public long GradeId { get; set; }
}

internal sealed class GetGradeByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGradeByIdQuery, GradeDto>
{
    public async Task<GradeDto> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Grades.GetByIdAsync(request.GradeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Grade not found.");

        return data.ToDto();
    }
}
