using System;
using System.Collections.Generic;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class RequestHistory
{
    public long RequestHistoryId { get; set; }

    public long CollectRequestId { get; set; }

    public long? UserId { get; set; }

    public CollectRequestStatus? PreviousStatusId { get; set; }

    public CollectRequestStatus NewStatusId { get; set; }

    public string? Observations { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CollectRequest CollectRequest { get; set; } = null!;

    public virtual User? User { get; set; }
}
