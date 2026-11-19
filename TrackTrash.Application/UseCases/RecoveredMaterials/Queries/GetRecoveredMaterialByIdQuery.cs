using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Queries;

public sealed record GetRecoveredMaterialByIdQuery : IRequest<RecoveredMaterialDto>
{
    public long RecoveredMaterialId { get; set; }
}

internal sealed class GetRecoveredMaterialByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecoveredMaterialByIdQuery, RecoveredMaterialDto>
{
    public async Task<RecoveredMaterialDto> Handle(GetRecoveredMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RecoveredMaterials.GetByIdAsync(request.RecoveredMaterialId, cancellationToken)
                   ?? throw new KeyNotFoundException("Recovered material not found.");

        return data.ToDto();
    }
}
