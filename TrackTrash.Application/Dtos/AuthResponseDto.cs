namespace TrackTrash.Application.Dtos;

public sealed record AuthResponseDto
{
    public required string Token { get; init; }

    public long UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string Role { get; init; }
}
