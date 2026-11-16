using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Ports.Services;

public interface IPasswordHashingService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password, string passwordHash);
}
