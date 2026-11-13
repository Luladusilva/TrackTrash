using System;
using System.Collections.Generic;

namespace TrackTrash.Domain.Entities;

public partial class VwGeneralIndicator
{
    public long? TotalUsers { get; set; }

    public long? TotalOrganizations { get; set; }

    public long? VerifiedGatherers { get; set; }

    public long? TotalRequests { get; set; }

    public long? CompletedRequests { get; set; }

    public long? CompletedServices { get; set; }

    public decimal? RecoveredKilograms { get; set; }
}
