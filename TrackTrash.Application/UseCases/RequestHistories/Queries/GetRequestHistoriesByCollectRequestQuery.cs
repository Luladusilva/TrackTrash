using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestHistories.Queries;

public sealed record GetRequestHistoriesByCollectRequestQuery : IRequest<IReadOnlyList<RequestHistoryDto>>
{
    public long CollectRequestId { get; set; }
}

internal sealed class GetRequestHistoriesByCollectRequestQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestHistoriesByCollectRequestQuery, IReadOnlyList<RequestHistoryDto>>
{
    public async Task<IReadOnlyList<RequestHistoryDto>> Handle(GetRequestHistoriesByCollectRequestQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestHistories.GetByCollectRequestIdAsync(request.CollectRequestId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
