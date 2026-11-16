using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Ports.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
