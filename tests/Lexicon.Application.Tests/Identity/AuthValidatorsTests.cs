using FluentAssertions;
using Lexicon.Application.Identity;
using Lexicon.Application.Identity.Validators;
using Xunit;

namespace Lexicon.Application.Tests.Identity;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_With_Valid_Request()
    {
        // Arrange
        var request = new RegisterRequest(
            Username: "testuser",
            Email: "test@example.com",
            Password: "SecurePass123!@#"
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Username is required")]
    [InlineData("ab", "Username must be at least 3 characters")]
    [InlineData("test user", "Username can only contain letters, numbers, underscores, and hyphens")]
    [InlineData("test@user", "Username can only contain letters, numbers, underscores, and hyphens")]
    public void Should_Fail_With_Invalid_Username(string username, string expectedError)
    {
        // Arrange
        var request = new RegisterRequest(
            Username: username,
            Email: "test@example.com",
            Password: "SecurePass123!@#"
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Email is required")]
    [InlineData("invalid-email", "Invalid email format")]
    [InlineData("@example.com", "Invalid email format")]
    public void Should_Fail_With_Invalid_Email(string email, string expectedError)
    {
        // Arrange
        var request = new RegisterRequest(
            Username: "testuser",
            Email: email,
            Password: "SecurePass123!@#"
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Password is required")]
    [InlineData("short", "Password must be at least 12 characters")]
    [InlineData("alllowercase1!", "Password must contain at least one uppercase letter")]
    [InlineData("ALLUPPERCASE1!", "Password must contain at least one lowercase letter")]
    [InlineData("NoDigitsHere!", "Password must contain at least one digit")]
    [InlineData("NoSpecialChar1", "Password must contain at least one special character")]
    public void Should_Fail_With_Weak_Password(string password, string expectedError)
    {
        // Arrange
        var request = new RegisterRequest(
            Username: "testuser",
            Email: "test@example.com",
            Password: password
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("ValidPass123!")]
    [InlineData("MySecure@Pass1")]
    [InlineData("Complex#Pass99")]
    [InlineData("Super$ecure123")]
    public void Should_Pass_With_Strong_Password(string password)
    {
        // Arrange
        var request = new RegisterRequest(
            Username: "testuser",
            Email: "test@example.com",
            Password: password
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_With_Valid_Request()
    {
        // Arrange
        var request = new LoginRequest(
            UsernameOrEmail: "testuser",
            Password: "anypassword"
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_With_Empty_UsernameOrEmail()
    {
        // Arrange
        var request = new LoginRequest(
            UsernameOrEmail: "",
            Password: "anypassword"
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Username or email is required");
    }

    [Fact]
    public void Should_Fail_With_Empty_Password()
    {
        // Arrange
        var request = new LoginRequest(
            UsernameOrEmail: "testuser",
            Password: ""
        );

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password is required");
    }
}
