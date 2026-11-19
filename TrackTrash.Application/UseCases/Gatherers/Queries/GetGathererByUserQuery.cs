using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Queries;

public sealed record GetGathererByUserQuery : IRequest<GathererDto>
{
    public long UserId { get; set; }
}

internal sealed class GetGathererByUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererByUserQuery, GathererDto>
{
    public async Task<GathererDto> Handle(GetGathererByUserQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetByUserIdAsync(request.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException("Gatherer not found.");

        return data.ToDto();
    }
}
