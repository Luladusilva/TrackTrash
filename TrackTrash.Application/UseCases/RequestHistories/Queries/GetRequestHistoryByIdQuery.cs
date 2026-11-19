using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RequestHistories.Queries;

public sealed record GetRequestHistoryByIdQuery : IRequest<RequestHistoryDto>
{
    public long RequestHistoryId { get; set; }
}

internal sealed class GetRequestHistoryByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRequestHistoryByIdQuery, RequestHistoryDto>
{
    public async Task<RequestHistoryDto> Handle(GetRequestHistoryByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RequestHistories.GetByIdAsync(request.RequestHistoryId, cancellationToken)
                   ?? throw new KeyNotFoundException("Request history not found.");

        return data.ToDto();
    }
}
