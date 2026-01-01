using Microsoft.AspNetCore.Authorization;

namespace Lexicon.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll("permission").Select(c => c.Value);

        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

// Extension method for adding permission policies
public static class AuthorizationExtensions
{
    public static void AddPermissionPolicies(this AuthorizationOptions options)
    {
        // Posts
        options.AddPolicy("CanReadPosts", policy =>
            policy.Requirements.Add(new PermissionRequirement("posts:read")));
        options.AddPolicy("CanCreatePosts", policy =>
            policy.Requirements.Add(new PermissionRequirement("posts:create")));
        options.AddPolicy("CanUpdatePosts", policy =>
            policy.Requirements.Add(new PermissionRequirement("posts:update")));
        options.AddPolicy("CanDeletePosts", policy =>
            policy.Requirements.Add(new PermissionRequirement("posts:delete")));
        options.AddPolicy("CanPublishPosts", policy =>
            policy.Requirements.Add(new PermissionRequirement("posts:publish")));

        // Categories
        options.AddPolicy("CanReadCategories", policy =>
            policy.Requirements.Add(new PermissionRequirement("categories:read")));
        options.AddPolicy("CanManageCategories", policy =>
            policy.Requirements.Add(new PermissionRequirement("categories:manage")));

        // Tags
        options.AddPolicy("CanReadTags", policy =>
            policy.Requirements.Add(new PermissionRequirement("tags:read")));
        options.AddPolicy("CanManageTags", policy =>
            policy.Requirements.Add(new PermissionRequirement("tags:manage")));

        // Comments
        options.AddPolicy("CanReadComments", policy =>
            policy.Requirements.Add(new PermissionRequirement("comments:read")));
        options.AddPolicy("CanCreateComments", policy =>
            policy.Requirements.Add(new PermissionRequirement("comments:create")));
        options.AddPolicy("CanModerateComments", policy =>
            policy.Requirements.Add(new PermissionRequirement("comments:moderate")));

        // Media
        options.AddPolicy("CanReadMedia", policy =>
            policy.Requirements.Add(new PermissionRequirement("media:read")));
        options.AddPolicy("CanUploadMedia", policy =>
            policy.Requirements.Add(new PermissionRequirement("media:upload")));
        options.AddPolicy("CanDeleteMedia", policy =>
            policy.Requirements.Add(new PermissionRequirement("media:delete")));

        // Users
        options.AddPolicy("CanReadUsers", policy =>
            policy.Requirements.Add(new PermissionRequirement("users:read")));
        options.AddPolicy("CanManageUsers", policy =>
            policy.Requirements.Add(new PermissionRequirement("users:manage")));

        // Settings
        options.AddPolicy("CanManageSettings", policy =>
            policy.Requirements.Add(new PermissionRequirement("settings:manage")));
    }
}
