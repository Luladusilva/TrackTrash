using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record RequesterProfileDto
{
    public long UserId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public UserRole RoleId { get; init; }
    public long? LocationId { get; init; }
    public string? OrganizationName { get; init; }
}
