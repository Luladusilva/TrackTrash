using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record RecoveredMaterialDto
{
    public long RecoveredMaterialId { get; init; }

    public long PickupServiceId { get; init; }

    public long ResidueTypeId { get; init; }

    public decimal Quantity { get; init; }

    public MeasurementUnit UnitId { get; init; }

    public long? CollectionPointId { get; init; }

    public DateTime CreatedAt { get; init; }

    public string? Observations { get; init; }
}

internal static class RecoveredMaterialDtoMapping
{
    public static RecoveredMaterialDto ToDto(this RecoveredMaterial entity)
    {
        return new RecoveredMaterialDto
        {
            RecoveredMaterialId = entity.RecoveredMaterialId,
            PickupServiceId = entity.PickupServiceId,
            ResidueTypeId = entity.ResidueTypeId,
            Quantity = entity.Quantity,
            UnitId = entity.UnitId,
            CollectionPointId = entity.CollectionPointId,
            CreatedAt = entity.CreatedAt,
            Observations = entity.Observations
        };
    }
}
