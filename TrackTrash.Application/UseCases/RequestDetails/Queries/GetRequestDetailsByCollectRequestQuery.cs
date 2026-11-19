using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestDetails.Queries;

public sealed record GetRequestDetailsByCollectRequestQuery : IRequest<IReadOnlyList<RequestDetailDto>>
{
    public long CollectRequestId { get; set; }
}

internal sealed class GetRequestDetailsByCollectRequestQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestDetailsByCollectRequestQuery, IReadOnlyList<RequestDetailDto>>
{
    public async Task<IReadOnlyList<RequestDetailDto>> Handle(GetRequestDetailsByCollectRequestQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestDetails.GetByCollectRequestIdAsync(request.CollectRequestId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
