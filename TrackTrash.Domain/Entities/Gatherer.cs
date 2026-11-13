using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class Gatherer
{
    public long GathererId { get; set; }

    public long UserId { get; set; }

    public GathererType GathererTypeId { get; set; }

    public string? Description { get; set; }

    public string? VerificationDocument { get; set; }

    public VerificationStatus VerificationStatusId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<GathererGrade> GathererGrades { get; set; } = new List<GathererGrade>();

    public virtual ICollection<PickupService> PickupServices { get; set; } = new List<PickupService>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ResidueType> ResidueTypes { get; set; } = new List<ResidueType>();
}
