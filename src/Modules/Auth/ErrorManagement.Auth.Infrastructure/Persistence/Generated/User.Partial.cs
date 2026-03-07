using ErrorManagement.Auth.Application.Interfaces;

namespace ErrorManagement.Auth.Infrastructure.Persistence; 

public partial class User : IAuthUser
{
    Guid IAuthUser.Id => Id;
    string IAuthUser.Username => Username;
    string IAuthUser.DisplayName => DisplayName;
    string IAuthUser.PasswordHash => PasswordHash;
    bool IAuthUser.IsActive => IsActive;

    public static User Create(string username, string passwordHash, string displayName)
        => new()
        {
            Id = Guid.NewGuid(),
            Username = username.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            DisplayName = displayName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public IEnumerable<string> GetPermissions()
        => Roles  // ← scaffolded navigation property is "Roles" not "UserRoles"
            .SelectMany(r => r.Permissions)
            .Select(p => p.Code)
            .Distinct()
            .OrderBy(p => p);
}