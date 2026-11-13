using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class RecoveredMaterial
{
    public long RecoveredMaterialId { get; set; }

    public long PickupServiceId { get; set; }

    public long ResidueTypeId { get; set; }

    public decimal Quantity { get; set; }

    public MeasurementUnit UnitId { get; set; }

    public long? CollectionPointId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Observations { get; set; }

    public virtual CollectionPoint? CollectionPoint { get; set; }

    public virtual PickupService PickupService { get; set; } = null!;

    public virtual ResidueType ResidueType { get; set; } = null!;
}
