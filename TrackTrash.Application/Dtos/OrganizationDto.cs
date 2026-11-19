using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record OrganizationDto
{
    public long OrganizationId { get; init; }

    public long UserId { get; init; }

    public string BusinessName { get; init; } = null!;

    public string? TaxId { get; init; }

    public OrganizationType OrganizationTypeId { get; init; }

    public long? LocationId { get; init; }

    public SubscriptionPlan SubscriptionPlanId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class OrganizationDtoMapping
{
    public static OrganizationDto ToDto(this Organization entity)
    {
        return new OrganizationDto
        {
            OrganizationId = entity.OrganizationId,
            UserId = entity.UserId,
            BusinessName = entity.BusinessName,
            TaxId = entity.TaxId,
            OrganizationTypeId = entity.OrganizationTypeId,
            LocationId = entity.LocationId,
            SubscriptionPlanId = entity.SubscriptionPlanId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}
