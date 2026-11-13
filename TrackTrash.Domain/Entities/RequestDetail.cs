using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class RequestDetail
{
    public long RequestDetailId { get; set; }

    public long CollectRequestId { get; set; }

    public long ResidueTypeId { get; set; }

    public decimal EstimatedQuantity { get; set; }

    public MeasurementUnit UnitId { get; set; }

    public string? Observations { get; set; }

    public virtual CollectRequest CollectRequest { get; set; } = null!;

    public virtual ResidueType ResidueType { get; set; } = null!;
}
