using TrackTrash.Application.Dtos;

namespace TrackTrash.Application.Ports.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto login);
}
