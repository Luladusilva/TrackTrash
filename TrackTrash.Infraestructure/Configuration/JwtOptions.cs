namespace TrackTrash.Infraestructure.Configuration;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationHours { get; set; } = 4;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
