using Lexicon.Domain.Entities;

namespace Lexicon.Application.Identity;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(string ipAddress);
    bool ValidateAccessToken(string token);
}

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
