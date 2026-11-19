using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Grades.Queries;

public sealed record GetGradeByServiceAndUserQuery : IRequest<GradeDto>
{
    public long PickupServiceId { get; set; }

    public long UserId { get; set; }
}

internal sealed class GetGradeByServiceAndUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGradeByServiceAndUserQuery, GradeDto>
{
    public async Task<GradeDto> Handle(GetGradeByServiceAndUserQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Grades.GetByServiceAndUserAsync(request.PickupServiceId, request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("Grade not found.");

        return data.ToDto();
    }
}
