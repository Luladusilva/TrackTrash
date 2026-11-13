using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class Organization
{
    public long OrganizationId { get; set; }

    public long UserId { get; set; }

    public string BusinessName { get; set; } = null!;

    public string? TaxId { get; set; }

    public OrganizationType OrganizationTypeId { get; set; }

    public long? LocationId { get; set; }

    public SubscriptionPlan SubscriptionPlanId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CollectRequest> CollectRequests { get; set; } = new List<CollectRequest>();

    public virtual Location? Location { get; set; }

    public virtual User User { get; set; } = null!;
}
