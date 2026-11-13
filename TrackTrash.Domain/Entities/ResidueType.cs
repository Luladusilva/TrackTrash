using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class ResidueType
{
    public long ResidueTypeId { get; set; }

    public string Name { get; set; } = null!;

    public ResidueCategory ResidueCategoryId { get; set; }

    public string? Description { get; set; }

    public bool IsRecyclable { get; set; }

    public string? ClassificationInstructions { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RecoveredMaterial> RecoveredMaterials { get; set; } = new List<RecoveredMaterial>();

    public virtual ICollection<RequestDetail> RequestDetails { get; set; } = new List<RequestDetail>();

    public virtual ICollection<CollectionPoint> CollectionPoints { get; set; } = new List<CollectionPoint>();

    public virtual ICollection<Gatherer> Gatherers { get; set; } = new List<Gatherer>();
}
