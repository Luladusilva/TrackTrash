using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Queries;

public sealed record GetPickupServiceByIdQuery : IRequest<PickupServiceDto>
{
    public long PickupServiceId { get; set; }
}

internal sealed class GetPickupServiceByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPickupServiceByIdQuery, PickupServiceDto>
{
    public async Task<PickupServiceDto> Handle(GetPickupServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                   ?? throw new KeyNotFoundException("Pickup service not found.");

        return data.ToDto();
    }
}
