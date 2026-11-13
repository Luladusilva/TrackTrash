using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class CollectRequest
{
    public long CollectRequestId { get; set; }

    public long UserId { get; set; }

    public long? OrganizationId { get; set; }

    public long LocationId { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public CollectRequestStatus StatusId { get; set; }

    public string? Observations { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Organization? Organization { get; set; }

    public virtual PickupService? PickupService { get; set; }

    public virtual ICollection<RequestDetail> RequestDetails { get; set; } = new List<RequestDetail>();

    public virtual ICollection<RequestHistory> RequestHistories { get; set; } = new List<RequestHistory>();

    public virtual User User { get; set; } = null!;
}
