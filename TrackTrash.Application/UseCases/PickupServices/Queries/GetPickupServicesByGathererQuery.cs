using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Queries;

public sealed record GetPickupServicesByGathererQuery : IRequest<IReadOnlyList<PickupServiceDto>>
{
    public long GathererId { get; set; }
}

internal sealed class GetPickupServicesByGathererQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPickupServicesByGathererQuery, IReadOnlyList<PickupServiceDto>>
{
    public async Task<IReadOnlyList<PickupServiceDto>> Handle(GetPickupServicesByGathererQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByGathererIdAsync(request.GathererId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
