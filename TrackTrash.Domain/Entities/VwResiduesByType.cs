using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class VwResiduesByType
{
    public long? ResidueTypeId { get; set; }

    public string? ResidueType { get; set; }

    public string? Category { get; set; }

    public string? Unit { get; set; }

    public decimal? RecoveredQuantity { get; set; }
}
