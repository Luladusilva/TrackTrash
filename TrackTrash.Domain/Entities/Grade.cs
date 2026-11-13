using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class Grade
{
    public long GradeId { get; set; }

    public long PickupServiceId { get; set; }

    public long UserId { get; set; }

    public short Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual GathererGrade? GathererGrade { get; set; }

    public virtual PickupService PickupService { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
