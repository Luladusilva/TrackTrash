using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Queries;

public sealed record GetAvailableGatherersForCollectRequestQuery(long CollectRequestId) : IRequest<IReadOnlyList<GathererDto>>;

internal sealed class GetAvailableGatherersForCollectRequestQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAvailableGatherersForCollectRequestQuery, IReadOnlyList<GathererDto>>
{
    public async Task<IReadOnlyList<GathererDto>> Handle(
        GetAvailableGatherersForCollectRequestQuery request,
        CancellationToken cancellationToken)
    {
        var gatherers = await unitOfWork.Gatherers.GetAvailableForCollectRequestAsync(request.CollectRequestId, cancellationToken);
        return gatherers.Select(x => x.ToDto()).ToList();
    }
}
