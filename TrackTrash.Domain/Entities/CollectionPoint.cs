using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class CollectionPoint
{
    public long CollectionPointId { get; set; }

    public string Name { get; set; } = null!;

    public string? ManagerName { get; set; }

    public string? Phone { get; set; }

    public long LocationId { get; set; }

    public string? OpeningHours { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<RecoveredMaterial> RecoveredMaterials { get; set; } = new List<RecoveredMaterial>();

    public virtual ICollection<ResidueType> ResidueTypes { get; set; } = new List<ResidueType>();
}
