namespace TrackTrash.Application.Dtos;

public sealed record AuthResponseDto
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required string TokenType { get; init; }

    public DateTime ExpiresAt { get; init; }

    public long ExpiresIn { get; init; }

    public int RefreshAfterSeconds { get; init; } = 1800;
}
