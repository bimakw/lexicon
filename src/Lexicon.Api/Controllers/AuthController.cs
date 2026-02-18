using Lexicon.Application.DTOs;
using Lexicon.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await authService.RegisterAsync(new RegisterRequest(
            request.Username,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName));

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.Error!, result.ErrorCode.ToString()!));
        }

        SetRefreshTokenCookie(result.RefreshToken!, result.RefreshTokenExpiry!.Value);

        return Ok(new AuthResponse(
            result.AccessToken!,
            result.User!));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var ipAddress = GetIpAddress();
        var result = await authService.LoginAsync(
            new LoginRequest(request.UsernameOrEmail, request.Password),
            ipAddress);

        if (!result.Success)
        {
            var statusCode = result.ErrorCode switch
            {
                AuthErrorCode.UserLocked => StatusCodes.Status403Forbidden,
                AuthErrorCode.UserInactive => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status401Unauthorized
            };

            return StatusCode(statusCode, new ErrorResponse(result.Error!, result.ErrorCode.ToString()!));
        }

        SetRefreshTokenCookie(result.RefreshToken!, result.RefreshTokenExpiry!.Value);

        logger.LogInformation("User {Username} logged in from {IpAddress}",
            result.User!.Username, ipAddress);

        return Ok(new AuthResponse(
            result.AccessToken!,
            result.User!));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new ErrorResponse("Refresh token not found", "InvalidToken"));
        }

        var ipAddress = GetIpAddress();
        var result = await authService.RefreshTokenAsync(refreshToken, ipAddress);

        if (!result.Success)
        {
            ClearRefreshTokenCookie();
            return Unauthorized(new ErrorResponse(result.Error!, result.ErrorCode.ToString()!));
        }

        SetRefreshTokenCookie(result.RefreshToken!, result.RefreshTokenExpiry!.Value);

        return Ok(new AuthResponse(
            result.AccessToken!,
            result.User!));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var ipAddress = GetIpAddress();
            await authService.RevokeTokenAsync(refreshToken, ipAddress, "User logout");
        }

        ClearRefreshTokenCookie();

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("revoke-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            return Unauthorized();
        }

        var ipAddress = GetIpAddress();
        await authService.RevokeAllUserTokensAsync(userGuid, ipAddress, "User requested");

        ClearRefreshTokenCookie();

        return Ok(new { message = "All sessions revoked" });
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var permissions = User.FindAll("permission").Select(c => c.Value).ToArray();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(new UserDto(
            Guid.Parse(userId),
            username ?? "",
            email ?? "",
            null, // FirstName not in token
            null, // LastName not in token
            null, // AvatarUrl not in token
            role ?? "",
            permissions));
    }

    private void SetRefreshTokenCookie(string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expires
        };

        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";
    }
}
