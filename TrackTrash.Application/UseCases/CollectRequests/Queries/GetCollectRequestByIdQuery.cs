using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetCollectRequestByIdQuery : IRequest<CollectRequestDto>
{
    public long CollectRequestId { get; set; }
}

internal sealed class GetCollectRequestByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectRequestByIdQuery, CollectRequestDto>
{
    public async Task<CollectRequestDto> Handle(GetCollectRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collect request not found.");

        return data.ToDto();
    }
}
