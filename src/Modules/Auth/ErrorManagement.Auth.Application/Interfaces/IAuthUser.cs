namespace ErrorManagement.Auth.Application.Interfaces;

public interface IAuthUser
{
    Guid Id { get; }
    string Username { get; }
    string DisplayName { get; }
    string PasswordHash { get; }
    bool IsActive { get; }
    void RecordLogin();
    IEnumerable<string> GetPermissions();
}