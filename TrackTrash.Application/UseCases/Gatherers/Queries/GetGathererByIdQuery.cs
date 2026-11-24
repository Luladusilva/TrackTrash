using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Queries;

public sealed record GetGathererByIdQuery : IRequest<GathererDto>
{
    public long GathererId { get; set; }
}

internal sealed class GetGathererByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGathererByIdQuery, GathererDto>
{
    public async Task<GathererDto> Handle(GetGathererByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetProfileByIdAsync(request.GathererId, cancellationToken)
                   ?? throw new KeyNotFoundException("Gatherer not found.");

        return data.ToDto();
    }
}
