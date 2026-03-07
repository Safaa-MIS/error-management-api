using ErrorManagement.Auth.Infrastructure.Persistence;
using ErrorManagement.Auth.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ErrorManagement.Auth.Infrastructure.Seed;

public static class AuthDbSeeder
{
    private static readonly (string Code, string Module, string Description)[] AllPermissions =
    [
        ("dashboard.view",                                "Dashboard",          "View dashboard"),
        ("clinicalattachment.view",                       "ClinicalAttachment", "Access clinical attachment module"),
        ("clinicalattachment.history.view",               "ClinicalAttachment", "View clinical attachment history"),
        ("clinicalattachment.icp.view",                   "ClinicalAttachment", "Check ICP user data"),
        ("clinicalattachment.users-exists.view",          "ClinicalAttachment", "Check users exists"),
        ("clinicalattachment.facilities-speciality.view", "ClinicalAttachment", "Check facilities and speciality"),
        ("patientservices.view",                          "PatientServices",    "Access patient services module"),
        ("medicalreport.view",                            "PatientServices",    "View medical reports"),
        ("sickleave.view",                                "PatientServices",    "View sick leave"),
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();
        var hasher = new PasswordHasher();

        if (!await db.Database.CanConnectAsync())
        {
            logger.LogError("Cannot connect to database. Seeding skipped.");
            return;
        }

        // ── Permissions ───────────────────────────────────────────────────────
        var existingCodes = await db.Permissions
            .AsNoTracking()
            .Select(p => p.Code)
            .ToHashSetAsync();

        var newPerms = AllPermissions
            .Where(p => !existingCodes.Contains(p.Code))
            .Select(p => Permission.Create(p.Code, p.Module, p.Description))
            .ToList();

        if (newPerms.Count > 0)
        {
            db.Permissions.AddRange(newPerms);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} new permissions.", newPerms.Count);
        }

        // ── SupportAdmin Role ─────────────────────────────────────────────────
        var adminRole = await db.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == "SupportAdmin");

        if (adminRole is null)
        {
            adminRole = Role.Create("SupportAdmin");
            db.Roles.Add(adminRole);
            await db.SaveChangesAsync();
            logger.LogInformation("SupportAdmin role created.");
        }

        // Assign any missing permissions — EF handles the join table automatically
        var allPerms = await db.Permissions.ToListAsync();
        var assignedIds = adminRole.Permissions.Select(p => p.Id).ToHashSet();
        var toAssign = allPerms.Where(p => !assignedIds.Contains(p.Id)).ToList();

        foreach (var perm in toAssign)
            adminRole.Permissions.Add(perm);

        if (toAssign.Count > 0)
        {
            await db.SaveChangesAsync();
            logger.LogInformation("Assigned {Count} permissions to SupportAdmin.", toAssign.Count);
        }

        // ── Default admin user ────────────────────────────────────────────────
        var adminExists = await db.Users.AsNoTracking().AnyAsync(u => u.Username == "admin");
        if (!adminExists)
        {
            var adminUser = User.Create("admin", hasher.Hash("Admin@123"), "System Administrator");
            adminUser.Roles.Add(adminRole);
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
            logger.LogInformation("Admin user seeded. username=admin password=Admin@123");
        }
    }
}