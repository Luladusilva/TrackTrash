using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Infraestructure.Adapters.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHashingService passwordHashingService,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginDto login)
    {
        if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
            return null;

        var user = await unitOfWork.Users.GetByEmailAsync(login.Email);

        if (user is null || !user.IsActive)
            return null;

        if (!passwordHashingService.VerifyPassword(user, login.Password, user.PasswordHash))
            return null;

        return new AuthResponseDto
        {
            Token = tokenService.GenerateToken(user),
            UserId = user.UserId,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.RoleId.ToString()
        };
    }
}
