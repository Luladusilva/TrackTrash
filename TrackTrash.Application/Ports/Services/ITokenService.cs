using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Ports.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, Guid sessionFamilyId);
}
