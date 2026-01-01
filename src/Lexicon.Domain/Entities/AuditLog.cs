using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AdditionalData { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public static class AuditActions
{
    // Authentication
    public const string Login = "Login";
    public const string LoginFailed = "LoginFailed";
    public const string Logout = "Logout";
    public const string Register = "Register";
    public const string PasswordChange = "PasswordChange";
    public const string PasswordReset = "PasswordReset";
    public const string TokenRefresh = "TokenRefresh";
    public const string TokenRevoked = "TokenRevoked";
    public const string AccountLocked = "AccountLocked";

    // CRUD operations
    public const string Create = "Create";
    public const string Read = "Read";
    public const string Update = "Update";
    public const string Delete = "Delete";

    // Content specific
    public const string Publish = "Publish";
    public const string Unpublish = "Unpublish";

    // Admin actions
    public const string RoleAssigned = "RoleAssigned";
    public const string PermissionChanged = "PermissionChanged";
    public const string UserActivated = "UserActivated";
    public const string UserDeactivated = "UserDeactivated";
}
