namespace Lexicon.Application.Identity;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request, string ipAddress);
    Task<AuthResult> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress, string reason);
    Task<bool> RevokeAllUserTokensAsync(Guid userId, string ipAddress, string reason);
}

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string? FirstName = null,
    string? LastName = null
);

public record LoginRequest(
    string UsernameOrEmail,
    string Password
);

public record AuthResult
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? RefreshTokenExpiry { get; init; }
    public UserDto? User { get; init; }
    public string? Error { get; init; }
    public AuthErrorCode? ErrorCode { get; init; }

    public static AuthResult Succeeded(string accessToken, string refreshToken, DateTime refreshTokenExpiry, UserDto user)
        => new()
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry,
            User = user
        };

    public static AuthResult Failed(string error, AuthErrorCode errorCode)
        => new()
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode
        };
}

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string Role,
    string[] Permissions
);

public enum AuthErrorCode
{
    InvalidCredentials,
    UserNotFound,
    UserLocked,
    UserInactive,
    EmailNotConfirmed,
    InvalidToken,
    TokenExpired,
    UsernameExists,
    EmailExists,
    WeakPassword,
    InvalidRequest
}
