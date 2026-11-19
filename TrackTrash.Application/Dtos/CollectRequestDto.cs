using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record CollectRequestDto
{
    public long CollectRequestId { get; init; }

    public long UserId { get; init; }

    public long? OrganizationId { get; init; }

    public long LocationId { get; init; }

    public DateTime RequestedAt { get; init; }

    public DateOnly ScheduledDate { get; init; }

    public TimeOnly ScheduledTime { get; init; }

    public CollectRequestStatus StatusId { get; init; }

    public string? Observations { get; init; }

    public DateTime UpdatedAt { get; init; }
}

internal static class CollectRequestDtoMapping
{
    public static CollectRequestDto ToDto(this CollectRequest entity)
    {
        return new CollectRequestDto
        {
            CollectRequestId = entity.CollectRequestId,
            UserId = entity.UserId,
            OrganizationId = entity.OrganizationId,
            LocationId = entity.LocationId,
            RequestedAt = entity.RequestedAt,
            ScheduledDate = entity.ScheduledDate,
            ScheduledTime = entity.ScheduledTime,
            StatusId = entity.StatusId,
            Observations = entity.Observations,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
