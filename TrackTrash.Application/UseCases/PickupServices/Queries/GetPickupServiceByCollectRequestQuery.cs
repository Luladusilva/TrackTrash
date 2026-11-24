using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Queries;

public sealed record GetPickupServiceByCollectRequestQuery : IRequest<PickupServiceDto>
{
    public long CollectRequestId { get; set; }
}

internal sealed class GetPickupServiceByCollectRequestQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPickupServiceByCollectRequestQuery, PickupServiceDto>
{
    public async Task<PickupServiceDto> Handle(GetPickupServiceByCollectRequestQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByCollectRequestIdAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Pickup service not found.");

        return data.ToDto();
    }
}
