using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Queries;

public sealed record GetPickupServicesQuery : IRequest<IReadOnlyList<PickupServiceDto>>;

internal sealed class GetPickupServicesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPickupServicesQuery, IReadOnlyList<PickupServiceDto>>
{
    public async Task<IReadOnlyList<PickupServiceDto>> Handle(GetPickupServicesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
