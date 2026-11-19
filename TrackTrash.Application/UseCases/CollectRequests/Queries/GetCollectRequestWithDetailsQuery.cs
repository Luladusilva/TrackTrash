using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetCollectRequestWithDetailsQuery : IRequest<CollectRequestDetailDto>
{
    public long CollectRequestId { get; set; }
}

internal sealed class GetCollectRequestWithDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectRequestWithDetailsQuery, CollectRequestDetailDto>
{
    public async Task<CollectRequestDetailDto> Handle(GetCollectRequestWithDetailsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetWithDetailsAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collect request not found.");

        return data.ToDetailDto();
    }
}
