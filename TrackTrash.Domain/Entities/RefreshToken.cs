namespace TrackTrash.Domain.Entities;

public partial class RefreshToken
{
    public long RefreshTokenId { get; set; }
    public long UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public long? ReplacedByRefreshTokenId { get; set; }
    public Guid FamilyId { get; set; }
    public virtual User User { get; set; } = null!;
}
