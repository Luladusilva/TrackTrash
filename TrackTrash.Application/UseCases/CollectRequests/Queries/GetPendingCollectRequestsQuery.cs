using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetPendingCollectRequestsQuery : IRequest<IReadOnlyList<CollectRequestDto>>;

internal sealed class GetPendingCollectRequestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPendingCollectRequestsQuery, IReadOnlyList<CollectRequestDto>>
{
    public async Task<IReadOnlyList<CollectRequestDto>> Handle(GetPendingCollectRequestsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetPendingAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
