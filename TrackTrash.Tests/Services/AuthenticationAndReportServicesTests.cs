using ClosedXML.Excel;
using System.IdentityModel.Tokens.Jwt;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Infraestructure.Adapters.Services;
using TrackTrash.Infraestructure.Configuration;

namespace TrackTrash.Tests.Services;

public class AuthenticationAndReportServicesTests
{
    [Fact]
    public void PasswordHashingService_HashesAndVerifiesPassword()
    {
        var user = CreateUser();
        var service = new PasswordHashingService();

        var hash = service.HashPassword(user, "Password123");

        Assert.NotEqual("Password123", hash);
        Assert.True(service.VerifyPassword(user, "Password123", hash));
        Assert.False(service.VerifyPassword(user, "Incorrect", hash));
    }

    [Fact]
    public void JwtTokenService_GeneratesValidToken()
    {
        var options = new JwtOptions
        {
            Key = "TrackTrash-Test-Key-At-Least-32-Characters",
            Issuer = "TrackTrashAPI",
            Audience = "TrackTrashClient",
            AccessTokenExpirationHours = 4
        };
        var service = new JwtTokenService(options);

        var familyId = Guid.NewGuid();
        var (token, expiresAt) = service.GenerateAccessToken(CreateUser(), familyId);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(options.Issuer, jwt.Issuer);
        Assert.Contains(options.Audience, jwt.Audiences);
        Assert.Contains(jwt.Claims, claim => claim.Type == "session_family" && claim.Value == familyId.ToString("D"));
        Assert.InRange((expiresAt - DateTime.UtcNow).TotalMinutes, 239, 240);
    }

    [Fact]
    public void XMLService_CreatesUsersWorkbook()
    {
        var users = new List<UserDto>
        {
            new()
            {
                UserId = 1,
                FirstName = "Andre",
                LastName = "Ancco",
                Email = "andre@tracktrash.com",
                RoleId = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        var service = new XMLService();

        var file = service.CreateUsersReport(users);

        using var workbook = new XLWorkbook(new MemoryStream(file));
        var worksheet = workbook.Worksheet("Usuarios");

        Assert.Equal("Nombre", worksheet.Cell(1, 2).GetString());
        Assert.Equal("Andre", worksheet.Cell(2, 2).GetString());
        Assert.Equal("andre@tracktrash.com", worksheet.Cell(2, 4).GetString());
    }

    private static User CreateUser()
    {
        return new User
        {
            UserId = 1,
            FirstName = "Andre",
            LastName = "Ancco",
            Email = "andre@tracktrash.com",
            PasswordHash = string.Empty,
            RoleId = UserRole.Admin,
            IsActive = true
        };
    }
}
