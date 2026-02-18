using Lexicon.Application.Identity;

namespace Lexicon.Application.DTOs;

public record RegisterDto(
    string Username,
    string Email,
    string Password,
    string? FirstName = null,
    string? LastName = null
);

public record LoginDto(
    string UsernameOrEmail,
    string Password
);

public record AuthResponse(
    string AccessToken,
    UserDto User
);

public record ErrorResponse(
    string Message,
    string Code
);
