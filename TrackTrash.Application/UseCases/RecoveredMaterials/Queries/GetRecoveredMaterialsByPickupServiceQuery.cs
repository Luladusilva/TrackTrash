using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Queries;

public sealed record GetRecoveredMaterialsByPickupServiceQuery : IRequest<IReadOnlyList<RecoveredMaterialDto>>
{
    public long PickupServiceId { get; set; }
}

internal sealed class GetRecoveredMaterialsByPickupServiceQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecoveredMaterialsByPickupServiceQuery, IReadOnlyList<RecoveredMaterialDto>>
{
    public async Task<IReadOnlyList<RecoveredMaterialDto>> Handle(GetRecoveredMaterialsByPickupServiceQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RecoveredMaterials.GetByPickupServiceIdAsync(request.PickupServiceId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
