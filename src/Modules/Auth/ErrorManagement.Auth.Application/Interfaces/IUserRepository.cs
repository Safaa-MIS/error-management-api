
namespace ErrorManagement.Auth.Application.Interfaces;

public interface IUserRepository
{
    Task<IAuthUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<IAuthUser?> GetByIdWithRolesAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}