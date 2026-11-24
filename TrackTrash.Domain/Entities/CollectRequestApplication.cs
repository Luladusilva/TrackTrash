using TrackTrash.Domain.Enums;

namespace TrackTrash.Domain.Entities;

public partial class CollectRequestApplication
{
    public long CollectRequestApplicationId { get; set; }
    public long CollectRequestId { get; set; }
    public long GathererId { get; set; }
    public PickupApplicationStatus StatusId { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public virtual CollectRequest CollectRequest { get; set; } = null!;
    public virtual Gatherer Gatherer { get; set; } = null!;
}
