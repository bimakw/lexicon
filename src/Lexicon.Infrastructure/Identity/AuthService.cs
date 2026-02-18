using System.Security.Cryptography;
using System.Text;
using Lexicon.Application.Identity;
using Lexicon.Domain.Entities;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lexicon.Infrastructure.Identity;

public class AuthService(
    LexiconDbContext context,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly LexiconDbContext _context = context;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ILogger<AuthService> _logger = logger;

    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Check password strength
        if (!IsPasswordStrong(request.Password))
        {
            return AuthResult.Failed(
                "Password must be at least 12 characters with uppercase, lowercase, number, and symbol",
                AuthErrorCode.WeakPassword);
        }

        // Check if username exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return AuthResult.Failed("Username already exists", AuthErrorCode.UsernameExists);
        }

        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return AuthResult.Failed("Email already exists", AuthErrorCode.EmailExists);
        }

        // Get default role (Reader)
        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Reader);
        if (defaultRole == null)
        {
            _logger.LogError("Default role 'Reader' not found. Please seed roles first.");
            return AuthResult.Failed("Registration unavailable", AuthErrorCode.InvalidRequest);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleId = defaultRole.Id,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {Username} registered successfully", user.Username);

        // Auto-login after registration
        user.Role = defaultRole;
        return await GenerateAuthResultAsync(user, "127.0.0.1");
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, string ipAddress)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Username == request.UsernameOrEmail ||
                u.Email == request.UsernameOrEmail.ToLowerInvariant());

        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent user: {UsernameOrEmail}", request.UsernameOrEmail);
            return AuthResult.Failed("Invalid credentials", AuthErrorCode.InvalidCredentials);
        }

        // Check lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("Login attempt for locked user: {Username}", user.Username);
            return AuthResult.Failed(
                $"Account locked. Try again after {user.LockoutEnd:HH:mm}",
                AuthErrorCode.UserLocked);
        }

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            await HandleFailedLoginAsync(user);
            return AuthResult.Failed("Invalid credentials", AuthErrorCode.InvalidCredentials);
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return AuthResult.Failed("Account is inactive", AuthErrorCode.UserInactive);
        }

        // Reset failed attempts on successful login
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        // Rehash password if needed
        if (_passwordHasher.NeedsRehash(user.PasswordHash))
        {
            user.PasswordHash = _passwordHasher.Hash(request.Password);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {Username} logged in from {IpAddress}", user.Username, ipAddress);

        return await GenerateAuthResultAsync(user, ipAddress);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        if (storedToken == null)
        {
            return AuthResult.Failed("Invalid token", AuthErrorCode.InvalidToken);
        }

        if (!storedToken.IsActive)
        {
            // Token reuse detected - revoke all user tokens
            if (storedToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token reuse detected for user {UserId}", storedToken.UserId);
                await RevokeAllUserTokensAsync(storedToken.UserId, ipAddress, "Token reuse detected");
            }
            return AuthResult.Failed("Token expired or revoked", AuthErrorCode.TokenExpired);
        }

        var user = storedToken.User;
        if (!user.IsActive)
        {
            return AuthResult.Failed("Account is inactive", AuthErrorCode.UserInactive);
        }

        // Rotate refresh token
        var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress);
        newRefreshToken.UserId = user.Id;

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.RevokedReason = "Replaced by new token";
        storedToken.ReplacedByToken = newRefreshToken.TokenHash;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var accessToken = _tokenService.GenerateAccessToken(user);

        return AuthResult.Succeeded(
            accessToken,
            newRefreshToken.Token,
            newRefreshToken.ExpiresAt,
            MapToUserDto(user));
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress, string reason)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        if (storedToken == null || !storedToken.IsActive)
        {
            return false;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.RevokedReason = reason;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId, string ipAddress, string reason)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.RevokedReason = reason;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task HandleFailedLoginAsync(User user)
    {
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= MaxFailedAttempts)
        {
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            _logger.LogWarning("User {Username} locked out after {Attempts} failed attempts",
                user.Username, user.FailedLoginAttempts);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<AuthResult> GenerateAuthResultAsync(User user, string ipAddress)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return AuthResult.Succeeded(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            MapToUserDto(user));
    }

    private static UserDto MapToUserDto(User user)
    {
        var permissions = string.IsNullOrEmpty(user.Role.Permissions)
            ? Array.Empty<string>()
            : user.Role.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.AvatarUrl,
            user.Role.Name,
            permissions);
    }

    private static bool IsPasswordStrong(string password)
    {
        if (password.Length < 12) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
