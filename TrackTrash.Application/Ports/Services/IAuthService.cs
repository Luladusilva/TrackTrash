using TrackTrash.Application.Dtos;

namespace TrackTrash.Application.Ports.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto login, CancellationToken cancellationToken = default);

    Task<AuthResponseDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeUserSessionsAsync(long userId, CancellationToken cancellationToken = default);
}
