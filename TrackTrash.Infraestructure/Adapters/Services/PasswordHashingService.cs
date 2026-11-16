using Microsoft.AspNetCore.Identity;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Entities;

namespace TrackTrash.Infraestructure.Adapters.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
