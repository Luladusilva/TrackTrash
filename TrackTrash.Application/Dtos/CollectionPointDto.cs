using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record CollectionPointDto
{
    public long CollectionPointId { get; init; }

    public string Name { get; init; } = null!;

    public string? ManagerName { get; init; }

    public string? Phone { get; init; }

    public long LocationId { get; init; }

    public string? OpeningHours { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class CollectionPointDtoMapping
{
    public static CollectionPointDto ToDto(this CollectionPoint entity)
    {
        return new CollectionPointDto
        {
            CollectionPointId = entity.CollectionPointId,
            Name = entity.Name,
            ManagerName = entity.ManagerName,
            Phone = entity.Phone,
            LocationId = entity.LocationId,
            OpeningHours = entity.OpeningHours,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}
