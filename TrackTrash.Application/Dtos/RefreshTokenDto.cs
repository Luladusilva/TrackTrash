using System.ComponentModel.DataAnnotations;

namespace TrackTrash.Application.Dtos;

public sealed record RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; init; } = null!;
}
