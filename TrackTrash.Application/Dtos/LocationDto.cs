using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record LocationDto
{
    public long LocationId { get; init; }

    public string Address { get; init; } = null!;

    public string? Reference { get; init; }

    public string District { get; init; } = null!;

    public string? Province { get; init; }

    public string? Department { get; init; }

    public decimal? Latitude { get; init; }

    public decimal? Longitude { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class LocationDtoMapping
{
    public static LocationDto ToDto(this Location entity)
    {
        return new LocationDto
        {
            LocationId = entity.LocationId,
            Address = entity.Address,
            Reference = entity.Reference,
            District = entity.District,
            Province = entity.Province,
            Department = entity.Department,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            CreatedAt = entity.CreatedAt
        };
    }
}
