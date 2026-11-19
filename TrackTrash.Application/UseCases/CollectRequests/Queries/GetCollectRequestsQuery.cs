using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetCollectRequestsQuery : IRequest<IReadOnlyList<CollectRequestDto>>;

internal sealed class GetCollectRequestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectRequestsQuery, IReadOnlyList<CollectRequestDto>>
{
    public async Task<IReadOnlyList<CollectRequestDto>> Handle(GetCollectRequestsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
