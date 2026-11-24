using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record UserDto
{
    public long UserId { get; init; }

    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string? Phone { get; init; }

    public UserRole RoleId { get; init; }

    public long? LocationId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}

internal static class UserDtoMapping
{
    public static UserDto ToDto(this User entity)
    {
        return new UserDto
        {
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            RoleId = entity.RoleId,
            LocationId = entity.LocationId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
