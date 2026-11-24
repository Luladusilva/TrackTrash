using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Queries;

public sealed record GetRecoveredMaterialsQuery : IRequest<IReadOnlyList<RecoveredMaterialDto>>;

internal sealed class GetRecoveredMaterialsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecoveredMaterialsQuery, IReadOnlyList<RecoveredMaterialDto>>
{
    public async Task<IReadOnlyList<RecoveredMaterialDto>> Handle(GetRecoveredMaterialsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RecoveredMaterials.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
