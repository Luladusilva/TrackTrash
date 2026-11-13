using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class PickupService
{
    public long PickupServiceId { get; set; }

    public long CollectRequestId { get; set; }

    public long GathererId { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public PickupServiceStatus StatusId { get; set; }

    public decimal CommissionPercentage { get; set; }

    public string? Observations { get; set; }

    public virtual CollectRequest CollectRequest { get; set; } = null!;

    public virtual Gatherer Gatherer { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<RecoveredMaterial> RecoveredMaterials { get; set; } = new List<RecoveredMaterial>();
}
