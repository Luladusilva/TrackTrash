using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Infraestructure.Context;

public sealed class CollectRequestApplicationConfiguration : IEntityTypeConfiguration<CollectRequestApplication>
{
    public void Configure(EntityTypeBuilder<CollectRequestApplication> entity)
    {
        entity.HasKey(x => x.CollectRequestApplicationId).HasName("collect_request_applications_pkey");
        entity.ToTable("collect_request_applications");
        entity.HasIndex(x => new { x.CollectRequestId, x.GathererId }, "uq_application_request_gatherer").IsUnique();
        entity.HasIndex(x => new { x.CollectRequestId, x.StatusId }, "idx_applications_request_status");
        entity.Property(x => x.CollectRequestApplicationId)
            .UseIdentityAlwaysColumn()
            .HasColumnName("collect_request_application_id");
        entity.Property(x => x.CollectRequestId).HasColumnName("collect_request_id");
        entity.Property(x => x.GathererId).HasColumnName("gatherer_id");
        entity.Property(x => x.StatusId)
            .HasConversion<short>()
            .HasDefaultValue(PickupApplicationStatus.Pending)
            .HasColumnName("status_id");
        entity.Property(x => x.AppliedAt).HasColumnName("applied_at");
        entity.Property(x => x.RespondedAt).HasColumnName("responded_at");
        entity.HasOne(x => x.CollectRequest).WithMany(x => x.Applications)
            .HasForeignKey(x => x.CollectRequestId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("collect_request_applications_request_id_fkey");
        entity.HasOne(x => x.Gatherer).WithMany(x => x.Applications)
            .HasForeignKey(x => x.GathererId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("collect_request_applications_gatherer_id_fkey");
    }
}
