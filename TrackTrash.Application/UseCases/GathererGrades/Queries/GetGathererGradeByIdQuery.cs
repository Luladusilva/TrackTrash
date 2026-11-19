using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.GathererGrades.Queries;

public sealed record GetGathererGradeByIdQuery : IRequest<GathererGradeDto>
{
    public long GathererGradeId { get; set; }
}

internal sealed class GetGathererGradeByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererGradeByIdQuery, GathererGradeDto>
{
    public async Task<GathererGradeDto> Handle(GetGathererGradeByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GathererGrades.GetByIdAsync(request.GathererGradeId, cancellationToken)
                   ?? throw new KeyNotFoundException("Gatherer grade not found.");

        return data.ToDto();
    }
}
