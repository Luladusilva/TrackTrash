using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackTrash.Domain.Entities;

namespace TrackTrash.Infraestructure.Context;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(x => x.RefreshTokenId).HasName("refresh_tokens_pkey");
        entity.ToTable("refresh_tokens");
        entity.HasIndex(x => x.TokenHash, "refresh_tokens_token_hash_key").IsUnique();
        entity.HasIndex(x => x.FamilyId, "idx_refresh_tokens_family");
        entity.HasIndex(x => new { x.UserId, x.ExpiresAt }, "idx_refresh_tokens_user_expiry");
        entity.Property(x => x.RefreshTokenId)
            .UseIdentityAlwaysColumn()
            .HasColumnName("refresh_token_id");
        entity.Property(x => x.UserId).HasColumnName("user_id");
        entity.Property(x => x.TokenHash).HasMaxLength(64).HasColumnName("token_hash");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at");
        entity.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        entity.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        entity.Property(x => x.ReplacedByRefreshTokenId).HasColumnName("replaced_by_refresh_token_id");
        entity.Property(x => x.FamilyId).HasColumnName("family_id");
        entity.HasOne(x => x.User).WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("refresh_tokens_user_id_fkey");
        entity.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(x => x.ReplacedByRefreshTokenId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("refresh_tokens_replaced_by_fkey");
    }
}
