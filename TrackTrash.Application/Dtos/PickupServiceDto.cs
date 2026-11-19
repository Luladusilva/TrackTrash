using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record PickupServiceDto
{
    public long PickupServiceId { get; init; }

    public long CollectRequestId { get; init; }

    public long GathererId { get; init; }

    public DateTime AssignedAt { get; init; }

    public DateTime? StartedAt { get; init; }

    public DateTime? CompletedAt { get; init; }

    public PickupServiceStatus StatusId { get; init; }

    public decimal CommissionPercentage { get; init; }

    public string? Observations { get; init; }
}

internal static class PickupServiceDtoMapping
{
    public static PickupServiceDto ToDto(this PickupService entity)
    {
        return new PickupServiceDto
        {
            PickupServiceId = entity.PickupServiceId,
            CollectRequestId = entity.CollectRequestId,
            GathererId = entity.GathererId,
            AssignedAt = entity.AssignedAt,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
            StatusId = entity.StatusId,
            CommissionPercentage = entity.CommissionPercentage,
            Observations = entity.Observations
        };
    }
}
