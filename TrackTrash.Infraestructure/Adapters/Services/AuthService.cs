using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.UnitOfWork;
using TrackTrash.Infraestructure.Configuration;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Services;

public class AuthService(
    TrackTrashDBContext context,
    IUnitOfWork unitOfWork,
    IPasswordHashingService passwordHashingService,
    ITokenService tokenService,
    JwtOptions options) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginDto login, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
            return null;

        var user = await unitOfWork.Users.GetByEmailAsync(login.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is null || !user.IsActive ||
            !passwordHashingService.VerifyPassword(user, login.Password, user.PasswordHash))
            return null;

        return await CreateSessionAsync(user, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var hash = HashToken(refreshToken);
        var now = DateTime.UtcNow;
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var current = await context.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (current is null)
        {
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        if (current.RevokedAt is not null)
        {
            await RevokeFamilyAsync(current.FamilyId, now, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        if (current.ExpiresAt <= now || !current.User.IsActive)
        {
            await RevokeFamilyAsync(current.FamilyId, now, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        var claimed = await context.RefreshTokens
            .Where(x => x.RefreshTokenId == current.RefreshTokenId && x.RevokedAt == null && x.ExpiresAt > now)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.RevokedAt, now), cancellationToken);

        if (claimed != 1)
        {
            await RevokeFamilyAsync(current.FamilyId, now, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        var response = await CreateSessionAsync(current.User, current.FamilyId, cancellationToken);
        var replacementHash = HashToken(response.RefreshToken);
        var replacementId = await context.RefreshTokens
            .Where(x => x.TokenHash == replacementHash)
            .Select(x => x.RefreshTokenId)
            .SingleAsync(cancellationToken);

        current.RevokedAt = now;
        current.ReplacedByRefreshTokenId = replacementId;
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var hash = HashToken(refreshToken);
        var token = await context.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (token is null)
            return;

        await RevokeFamilyAsync(token.FamilyId, DateTime.UtcNow, cancellationToken);
    }

    public async Task RevokeUserSessionsAsync(long userId, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.RevokedAt, DateTime.UtcNow), cancellationToken);
    }

    private async Task<AuthResponseDto> CreateSessionAsync(User user, Guid familyId, CancellationToken cancellationToken)
    {
        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, familyId);
        var rawRefreshToken = CreateOpaqueToken();
        var now = DateTime.UtcNow;

        await context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = HashToken(rawRefreshToken),
            FamilyId = familyId,
            CreatedAt = now,
            ExpiresAt = now.AddDays(options.RefreshTokenExpirationDays)
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            ExpiresIn = Math.Max(0, (long)(expiresAt - now).TotalSeconds),
            RefreshAfterSeconds = 1800
        };
    }

    private async Task RevokeFamilyAsync(Guid familyId, DateTime revokedAt, CancellationToken cancellationToken)
    {
        await context.RefreshTokens
            .Where(x => x.FamilyId == familyId && x.RevokedAt == null)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.RevokedAt, revokedAt), cancellationToken);
    }

    private static string CreateOpaqueToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
