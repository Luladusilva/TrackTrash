using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Infraestructure.Context;

public partial class TrackTrashDBContext : DbContext
{
    public TrackTrashDBContext(DbContextOptions<TrackTrashDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CollectRequest> CollectRequests { get; set; }

    public virtual DbSet<CollectionPoint> CollectionPoints { get; set; }

    public virtual DbSet<Gatherer> Gatherers { get; set; }

    public virtual DbSet<GathererGrade> GathererGrades { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<PickupService> PickupServices { get; set; }

    public virtual DbSet<RecoveredMaterial> RecoveredMaterials { get; set; }

    public virtual DbSet<RequestDetail> RequestDetails { get; set; }

    public virtual DbSet<RequestHistory> RequestHistories { get; set; }

    public virtual DbSet<ResidueType> ResidueTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwGathererGrade> VwGathererGrades { get; set; }

    public virtual DbSet<VwGeneralIndicator> VwGeneralIndicators { get; set; }

    public virtual DbSet<VwResiduesByType> VwResiduesByTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CollectRequest>(entity =>
        {
            entity.HasKey(e => e.CollectRequestId).HasName("collect_requests_pkey");

            entity.ToTable("collect_requests");

            entity.HasIndex(e => e.ScheduledDate, "idx_requests_date");

            entity.HasIndex(e => e.OrganizationId, "idx_requests_organization");

            entity.HasIndex(e => e.StatusId, "idx_requests_status");

            entity.HasIndex(e => e.UserId, "idx_requests_user");

            entity.Property(e => e.CollectRequestId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("collect_request_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Observations).HasColumnName("observations");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("requested_at");
            entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
            entity.Property(e => e.ScheduledTime).HasColumnName("scheduled_time");
            entity.Property(e => e.StatusId)
                .HasConversion<short>()
                .HasDefaultValue(CollectRequestStatus.Pending)
                .HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Location).WithMany(p => p.CollectRequests)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("collect_requests_location_id_fkey");

            entity.HasOne(d => d.Organization).WithMany(p => p.CollectRequests)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("collect_requests_organization_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CollectRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("collect_requests_user_id_fkey");
        });

        modelBuilder.Entity<CollectionPoint>(entity =>
        {
            entity.HasKey(e => e.CollectionPointId).HasName("collection_points_pkey");

            entity.ToTable("collection_points");

            entity.Property(e => e.CollectionPointId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("collection_point_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.ManagerName)
                .HasMaxLength(120)
                .HasColumnName("manager_name");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.OpeningHours)
                .HasMaxLength(150)
                .HasColumnName("opening_hours");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");

            entity.HasOne(d => d.Location).WithMany(p => p.CollectionPoints)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("collection_points_location_id_fkey");

            entity.HasMany(d => d.ResidueTypes).WithMany(p => p.CollectionPoints)
                .UsingEntity<Dictionary<string, object>>(
                    "CollectionPointResidueType",
                    r => r.HasOne<ResidueType>().WithMany()
                        .HasForeignKey("ResidueTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("collection_point_residue_types_residue_type_id_fkey"),
                    l => l.HasOne<CollectionPoint>().WithMany()
                        .HasForeignKey("CollectionPointId")
                        .HasConstraintName("collection_point_residue_types_collection_point_id_fkey"),
                    j =>
                    {
                        j.HasKey("CollectionPointId", "ResidueTypeId").HasName("collection_point_residue_types_pkey");
                        j.ToTable("collection_point_residue_types");
                        j.IndexerProperty<long>("CollectionPointId").HasColumnName("collection_point_id");
                        j.IndexerProperty<long>("ResidueTypeId").HasColumnName("residue_type_id");
                    });
        });

        modelBuilder.Entity<Gatherer>(entity =>
        {
            entity.HasKey(e => e.GathererId).HasName("gatherers_pkey");

            entity.ToTable("gatherers");

            entity.HasIndex(e => e.UserId, "gatherers_user_id_key").IsUnique();

            entity.HasIndex(e => e.VerificationStatusId, "idx_gatherers_verification");

            entity.Property(e => e.GathererId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("gatherer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GathererTypeId)
                .HasConversion<short>()
                .HasColumnName("gatherer_type_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerificationDocument)
                .HasMaxLength(50)
                .HasColumnName("verification_document");
            entity.Property(e => e.VerificationStatusId)
                .HasConversion<short>()
                .HasDefaultValue(VerificationStatus.Pending)
                .HasColumnName("verification_status_id");

            entity.HasOne(d => d.User).WithOne(p => p.Gatherer)
                .HasForeignKey<Gatherer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gatherers_user_id_fkey");

            entity.HasMany(d => d.ResidueTypes).WithMany(p => p.Gatherers)
                .UsingEntity<Dictionary<string, object>>(
                    "GathererResidueType",
                    r => r.HasOne<ResidueType>().WithMany()
                        .HasForeignKey("ResidueTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("gatherer_residue_types_residue_type_id_fkey"),
                    l => l.HasOne<Gatherer>().WithMany()
                        .HasForeignKey("GathererId")
                        .HasConstraintName("gatherer_residue_types_gatherer_id_fkey"),
                    j =>
                    {
                        j.HasKey("GathererId", "ResidueTypeId").HasName("gatherer_residue_types_pkey");
                        j.ToTable("gatherer_residue_types");
                        j.IndexerProperty<long>("GathererId").HasColumnName("gatherer_id");
                        j.IndexerProperty<long>("ResidueTypeId").HasColumnName("residue_type_id");
                    });
        });

        modelBuilder.Entity<GathererGrade>(entity =>
        {
            entity.HasKey(e => e.GathererGradeId).HasName("gatherer_grades_pkey");

            entity.ToTable("gatherer_grades");

            entity.HasIndex(e => e.GradeId, "gatherer_grades_grade_id_key").IsUnique();

            entity.Property(e => e.GathererGradeId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("gatherer_grade_id");
            entity.Property(e => e.GathererId).HasColumnName("gatherer_id");
            entity.Property(e => e.GradeId).HasColumnName("grade_id");

            entity.HasOne(d => d.Gatherer).WithMany(p => p.GathererGrades)
                .HasForeignKey(d => d.GathererId)
                .HasConstraintName("gatherer_grades_gatherer_id_fkey");

            entity.HasOne(d => d.Grade).WithOne(p => p.GathererGrade)
                .HasForeignKey<GathererGrade>(d => d.GradeId)
                .HasConstraintName("gatherer_grades_grade_id_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("grades_pkey");

            entity.ToTable("grades");

            entity.HasIndex(e => new { e.PickupServiceId, e.UserId }, "uq_grade_service_user").IsUnique();

            entity.Property(e => e.GradeId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("grade_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.PickupServiceId).HasColumnName("pickup_service_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PickupService).WithMany(p => p.Grades)
                .HasForeignKey(d => d.PickupServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grades_pickup_service_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Grades)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grades_user_id_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("locations_pkey");

            entity.ToTable("locations");

            entity.HasIndex(e => e.District, "idx_locations_district");

            entity.Property(e => e.LocationId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("location_id");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Arequipa'::character varying")
                .HasColumnName("department");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(10, 7)
                .HasColumnName("longitude");
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Arequipa'::character varying")
                .HasColumnName("province");
            entity.Property(e => e.Reference)
                .HasMaxLength(250)
                .HasColumnName("reference");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("organizations_pkey");

            entity.ToTable("organizations");

            entity.HasIndex(e => e.OrganizationTypeId, "idx_organizations_type");

            entity.HasIndex(e => e.TaxId, "organizations_tax_id_key").IsUnique();

            entity.HasIndex(e => e.UserId, "organizations_user_id_key").IsUnique();

            entity.Property(e => e.OrganizationId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("organization_id");
            entity.Property(e => e.BusinessName)
                .HasMaxLength(180)
                .HasColumnName("business_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.OrganizationTypeId)
                .HasConversion<short>()
                .HasColumnName("organization_type_id");
            entity.Property(e => e.SubscriptionPlanId)
                .HasConversion<short>()
                .HasDefaultValue(SubscriptionPlan.Basic)
                .HasColumnName("subscription_plan_id");
            entity.Property(e => e.TaxId)
                .HasMaxLength(11)
                .HasColumnName("tax_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Location).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("organizations_location_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Organization)
                .HasForeignKey<Organization>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organizations_user_id_fkey");
        });

        modelBuilder.Entity<PickupService>(entity =>
        {
            entity.HasKey(e => e.PickupServiceId).HasName("pickup_services_pkey");

            entity.ToTable("pickup_services");

            entity.HasIndex(e => e.GathererId, "idx_services_gatherer");

            entity.HasIndex(e => e.StatusId, "idx_services_status");

            entity.HasIndex(e => e.CollectRequestId, "pickup_services_collect_request_id_key").IsUnique();

            entity.Property(e => e.PickupServiceId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("pickup_service_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("assigned_at");
            entity.Property(e => e.CollectRequestId).HasColumnName("collect_request_id");
            entity.Property(e => e.CommissionPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValue(10.00m)
                .HasColumnName("commission_percentage");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.GathererId).HasColumnName("gatherer_id");
            entity.Property(e => e.Observations).HasColumnName("observations");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.StatusId)
                .HasConversion<short>()
                .HasDefaultValue(PickupServiceStatus.Pending)
                .HasColumnName("status_id");

            entity.HasOne(d => d.CollectRequest).WithOne(p => p.PickupService)
                .HasForeignKey<PickupService>(d => d.CollectRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pickup_services_collect_request_id_fkey");

            entity.HasOne(d => d.Gatherer).WithMany(p => p.PickupServices)
                .HasForeignKey(d => d.GathererId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pickup_services_gatherer_id_fkey");
        });

        modelBuilder.Entity<RecoveredMaterial>(entity =>
        {
            entity.HasKey(e => e.RecoveredMaterialId).HasName("recovered_materials_pkey");

            entity.ToTable("recovered_materials");

            entity.HasIndex(e => e.PickupServiceId, "idx_recovered_service");

            entity.Property(e => e.RecoveredMaterialId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("recovered_material_id");
            entity.Property(e => e.CollectionPointId).HasColumnName("collection_point_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Observations).HasColumnName("observations");
            entity.Property(e => e.PickupServiceId).HasColumnName("pickup_service_id");
            entity.Property(e => e.Quantity)
                .HasPrecision(12, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.ResidueTypeId).HasColumnName("residue_type_id");
            entity.Property(e => e.UnitId)
                .HasConversion<short>()
                .HasDefaultValue(MeasurementUnit.Kg)
                .HasColumnName("unit_id");

            entity.HasOne(d => d.CollectionPoint).WithMany(p => p.RecoveredMaterials)
                .HasForeignKey(d => d.CollectionPointId)
                .HasConstraintName("recovered_materials_collection_point_id_fkey");

            entity.HasOne(d => d.PickupService).WithMany(p => p.RecoveredMaterials)
                .HasForeignKey(d => d.PickupServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recovered_materials_pickup_service_id_fkey");

            entity.HasOne(d => d.ResidueType).WithMany(p => p.RecoveredMaterials)
                .HasForeignKey(d => d.ResidueTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recovered_materials_residue_type_id_fkey");
        });

        modelBuilder.Entity<RequestDetail>(entity =>
        {
            entity.HasKey(e => e.RequestDetailId).HasName("request_details_pkey");

            entity.ToTable("request_details");

            entity.HasIndex(e => new { e.CollectRequestId, e.ResidueTypeId }, "uq_request_residue").IsUnique();

            entity.Property(e => e.RequestDetailId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("request_detail_id");
            entity.Property(e => e.CollectRequestId).HasColumnName("collect_request_id");
            entity.Property(e => e.EstimatedQuantity)
                .HasPrecision(12, 2)
                .HasColumnName("estimated_quantity");
            entity.Property(e => e.Observations).HasColumnName("observations");
            entity.Property(e => e.ResidueTypeId).HasColumnName("residue_type_id");
            entity.Property(e => e.UnitId)
                .HasConversion<short>()
                .HasDefaultValue(MeasurementUnit.Kg)
                .HasColumnName("unit_id");

            entity.HasOne(d => d.CollectRequest).WithMany(p => p.RequestDetails)
                .HasForeignKey(d => d.CollectRequestId)
                .HasConstraintName("request_details_collect_request_id_fkey");

            entity.HasOne(d => d.ResidueType).WithMany(p => p.RequestDetails)
                .HasForeignKey(d => d.ResidueTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("request_details_residue_type_id_fkey");
        });

        modelBuilder.Entity<RequestHistory>(entity =>
        {
            entity.HasKey(e => e.RequestHistoryId).HasName("request_history_pkey");

            entity.ToTable("request_history");

            entity.HasIndex(e => new { e.CollectRequestId, e.CreatedAt }, "idx_history_request");

            entity.Property(e => e.RequestHistoryId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("request_history_id");
            entity.Property(e => e.CollectRequestId).HasColumnName("collect_request_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.NewStatusId)
                .HasConversion<short>()
                .HasColumnName("new_status_id");
            entity.Property(e => e.Observations).HasColumnName("observations");
            entity.Property(e => e.PreviousStatusId)
                .HasConversion<short>()
                .HasColumnName("previous_status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CollectRequest).WithMany(p => p.RequestHistories)
                .HasForeignKey(d => d.CollectRequestId)
                .HasConstraintName("request_history_collect_request_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.RequestHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("request_history_user_id_fkey");
        });

        modelBuilder.Entity<ResidueType>(entity =>
        {
            entity.HasKey(e => e.ResidueTypeId).HasName("residue_types_pkey");

            entity.ToTable("residue_types");

            entity.HasIndex(e => e.ResidueCategoryId, "idx_residue_types_category");

            entity.HasIndex(e => e.Name, "residue_types_name_key").IsUnique();

            entity.Property(e => e.ResidueTypeId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("residue_type_id");
            entity.Property(e => e.ClassificationInstructions).HasColumnName("classification_instructions");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsRecyclable).HasColumnName("is_recyclable");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ResidueCategoryId)
                .HasConversion<short>()
                .HasColumnName("residue_category_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.RoleId, "idx_users_role");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(120)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(120)
                .HasColumnName("last_name");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId)
                .HasConversion<short>()
                .HasDefaultValue(UserRole.Generator)
                .HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Location).WithMany(p => p.Users)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("users_location_id_fkey");
        });

        modelBuilder.Entity<VwGathererGrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_gatherer_grades");

            entity.Property(e => e.AverageScore).HasColumnName("average_score");
            entity.Property(e => e.FirstName)
                .HasMaxLength(120)
                .HasColumnName("first_name");
            entity.Property(e => e.GathererId).HasColumnName("gatherer_id");
            entity.Property(e => e.LastName)
                .HasMaxLength(120)
                .HasColumnName("last_name");
            entity.Property(e => e.TotalGrades).HasColumnName("total_grades");
        });

        modelBuilder.Entity<VwGeneralIndicator>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_general_indicators");

            entity.Property(e => e.CompletedRequests).HasColumnName("completed_requests");
            entity.Property(e => e.CompletedServices).HasColumnName("completed_services");
            entity.Property(e => e.RecoveredKilograms).HasColumnName("recovered_kilograms");
            entity.Property(e => e.TotalOrganizations).HasColumnName("total_organizations");
            entity.Property(e => e.TotalRequests).HasColumnName("total_requests");
            entity.Property(e => e.TotalUsers).HasColumnName("total_users");
            entity.Property(e => e.VerifiedGatherers).HasColumnName("verified_gatherers");
        });

        modelBuilder.Entity<VwResiduesByType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_residues_by_type");

            entity.Property(e => e.Category)
                .HasMaxLength(40)
                .HasColumnName("category");
            entity.Property(e => e.RecoveredQuantity).HasColumnName("recovered_quantity");
            entity.Property(e => e.ResidueType)
                .HasMaxLength(100)
                .HasColumnName("residue_type");
            entity.Property(e => e.ResidueTypeId).HasColumnName("residue_type_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(40)
                .HasColumnName("unit");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
