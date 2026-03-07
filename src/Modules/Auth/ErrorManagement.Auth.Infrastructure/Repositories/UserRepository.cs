using ErrorManagement.Auth.Application.Interfaces;
using ErrorManagement.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErrorManagement.Auth.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;
    public UserRepository(AuthDbContext db) => _db = db;

    public async Task<IAuthUser?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<IAuthUser?> GetByIdWithRolesAsync(Guid id, CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}