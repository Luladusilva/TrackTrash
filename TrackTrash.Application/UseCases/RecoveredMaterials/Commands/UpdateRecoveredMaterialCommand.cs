using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Commands;

public sealed record UpdateRecoveredMaterialCommand : IRequest<long>
{
    public long RecoveredMaterialId { get; set; }

    public long ResidueTypeId { get; set; }

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Quantity { get; set; }

    public MeasurementUnit UnitId { get; set; }

    public long? CollectionPointId { get; set; }

    public string? Observations { get; set; }
}

internal sealed class UpdateRecoveredMaterialCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecoveredMaterialCommand, long>
{
    public async Task<long> Handle(UpdateRecoveredMaterialCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.RecoveredMaterials.GetByIdAsync(request.RecoveredMaterialId, cancellationToken)
                   ?? throw new KeyNotFoundException("Recovered material not found.");

        await RecoveredMaterialRules.ValidateReferencesAsync(unitOfWork, request.ResidueTypeId, request.CollectionPointId, cancellationToken);

        data.ResidueTypeId = request.ResidueTypeId;
        data.Quantity = request.Quantity;
        data.UnitId = request.UnitId;
        data.CollectionPointId = request.CollectionPointId;
        data.Observations = request.Observations;

        unitOfWork.RecoveredMaterials.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.RecoveredMaterialId;
    }
}
