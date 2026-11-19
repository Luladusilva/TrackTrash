using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetCollectRequestsByUserQuery : IRequest<IReadOnlyList<CollectRequestDto>>
{
    public long UserId { get; set; }
}

internal sealed class GetCollectRequestsByUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectRequestsByUserQuery, IReadOnlyList<CollectRequestDto>>
{
    public async Task<IReadOnlyList<CollectRequestDto>> Handle(GetCollectRequestsByUserQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetByUserIdAsync(request.UserId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
