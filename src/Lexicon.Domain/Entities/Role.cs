using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Permissions stored as comma-separated values
    // e.g., "posts:read,posts:write,posts:delete,posts:publish"
    public string Permissions { get; set; } = string.Empty;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();

    // Helper methods
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrEmpty(Permissions)) return false;
        var perms = Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return perms.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public void AddPermission(string permission)
    {
        var perms = string.IsNullOrEmpty(Permissions)
            ? new List<string>()
            : Permissions.Split(',').ToList();

        if (!perms.Contains(permission, StringComparer.OrdinalIgnoreCase))
        {
            perms.Add(permission);
            Permissions = string.Join(",", perms);
        }
    }
}

// Predefined roles
public static class Roles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";
    public const string Author = "Author";
    public const string Reader = "Reader";
}

// Predefined permissions
public static class Permissions
{
    // Posts
    public const string PostsRead = "posts:read";
    public const string PostsCreate = "posts:create";
    public const string PostsUpdate = "posts:update";
    public const string PostsDelete = "posts:delete";
    public const string PostsPublish = "posts:publish";

    // Categories
    public const string CategoriesRead = "categories:read";
    public const string CategoriesManage = "categories:manage";

    // Tags
    public const string TagsRead = "tags:read";
    public const string TagsManage = "tags:manage";

    // Comments
    public const string CommentsRead = "comments:read";
    public const string CommentsCreate = "comments:create";
    public const string CommentsModerate = "comments:moderate";

    // Media
    public const string MediaRead = "media:read";
    public const string MediaUpload = "media:upload";
    public const string MediaDelete = "media:delete";

    // Users
    public const string UsersRead = "users:read";
    public const string UsersManage = "users:manage";

    // Settings
    public const string SettingsManage = "settings:manage";
}
