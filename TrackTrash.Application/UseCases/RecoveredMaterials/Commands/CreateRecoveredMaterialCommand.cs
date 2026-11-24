using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.RecoveredMaterials.Commands;

public sealed record CreateRecoveredMaterialCommand : IRequest<long>
{
    public long PickupServiceId { get; set; }

    public long ResidueTypeId { get; set; }

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Quantity { get; set; }

    public MeasurementUnit UnitId { get; set; } = MeasurementUnit.Kg;

    public long? CollectionPointId { get; set; }

    public string? Observations { get; set; }
}

internal sealed class CreateRecoveredMaterialCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRecoveredMaterialCommand, long>
{
    public async Task<long> Handle(CreateRecoveredMaterialCommand request, CancellationToken cancellationToken)
    {
        var service = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                      ?? throw new KeyNotFoundException("Pickup service not found.");

        if (service.StatusId is not (PickupServiceStatus.InProgress or PickupServiceStatus.Completed))
        {
            throw new InvalidOperationException("Materials can only be recorded once the pickup has started.");
        }

        await RecoveredMaterialRules.ValidateReferencesAsync(unitOfWork, request.ResidueTypeId, request.CollectionPointId, cancellationToken);

        var data = new RecoveredMaterial
        {
            PickupServiceId = request.PickupServiceId,
            ResidueTypeId = request.ResidueTypeId,
            Quantity = request.Quantity,
            UnitId = request.UnitId,
            CollectionPointId = request.CollectionPointId,
            Observations = request.Observations,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.RecoveredMaterials.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.RecoveredMaterialId;
    }
}

internal static class RecoveredMaterialRules
{
    public static async Task ValidateReferencesAsync(
        IUnitOfWork unitOfWork, long residueTypeId, long? collectionPointId, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.ResidueTypes.ExistsAsync(x => x.ResidueTypeId == residueTypeId, cancellationToken))
        {
            throw new KeyNotFoundException("Residue type not found.");
        }

        if (collectionPointId.HasValue &&
            !await unitOfWork.CollectionPoints.ExistsAsync(x => x.CollectionPointId == collectionPointId && x.IsActive, cancellationToken))
        {
            throw new KeyNotFoundException("Active collection point not found.");
        }
    }
}
