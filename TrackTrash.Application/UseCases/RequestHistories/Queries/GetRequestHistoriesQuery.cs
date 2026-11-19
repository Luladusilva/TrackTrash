using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestHistories.Queries;

public sealed record GetRequestHistoriesQuery : IRequest<IReadOnlyList<RequestHistoryDto>>;

internal sealed class GetRequestHistoriesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestHistoriesQuery, IReadOnlyList<RequestHistoryDto>>
{
    public async Task<IReadOnlyList<RequestHistoryDto>> Handle(GetRequestHistoriesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestHistories.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
