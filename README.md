# error-management-api
ASP.NET Core 10 modular monolith — JWT cookie auth, per-endpoint RBAC, MediatR CQRS, SQL Server DB-first, Serilog, HTTP resilience

Backend API for the Error Management internal support portal.  
Built with **ASP.NET Core 10** following a **modular monolith** architecture.

---

## Architecture
```
ErrorManagement.Api (host)          ← Composition root. Wires everything.
│
├── ErrorManagement.Shared          ← No dependencies. Exceptions, ICurrentUser, HTTP resilience.
│
└── Modules/
    ├── Auth/
    │   ├── Auth.Application        ← Login command, IUserRepository, IJwtTokenService
    │   ├── Auth.Infrastructure     ← EF Core (DB-first), JWT, PBKDF2 hasher, cookie service
    │   └── Auth.Api                ← AuthController (login / logout)
    │
    ├── Navigation/
    │   ├── Navigation.Application  ← GetMyNavigation query, per-user IMemoryCache
    │   └── Navigation.Api          ← NavigationController
    │
    ├── Dashboard/
    │   ├── Dashboard.Application   ← GetDashboardCards, permission-filtered
    │   └── Dashboard.Api           ← DashboardController
    │
    ├── ClinicalAttachment/
    │   ├── ClinicalAttachment.Application   ← Queries, IClinicalAttachmentApiClient
    │   ├── ClinicalAttachment.Infrastructure ← Typed HttpClient + resilience pipeline
    │   └── ClinicalAttachment.Api            ← ClinicalAttachmentController
    │
    └── PatientServices/
        ├── PatientServices.Application      ← Search, MedicalReport, SickLeave queries
        ├── PatientServices.Infrastructure   ← Typed HttpClient + resilience pipeline
        └── PatientServices.Api              ← PatientServicesController
```

### Dependency rule
```
Shared  ←  Application  ←  Infrastructure
                        ←  Api (classlib)
                                ↑
                         Api host (only)
```

No module references another module. The host is the only project that knows about everything.

---

## Key Design Decisions

| Concern | Decision | Reason |
|---|---|---|
| Architecture | Modular monolith | Single SQL Server, single deploy, internal tool — microservices overhead unjustified |
| Auth | JWT in httpOnly cookie | Prevents JS token theft; no Authorization header used |
| Refresh token | None | Internal tool, single shift = 8h session, no Redis available |
| Password hashing | PBKDF2-SHA256, 100k iterations | OWASP-compliant, constant-time comparison |
| ORM | EF Core DB-first (scaffolded) | Schema owned by DBA; partial classes add domain logic without touching generated files |
| CQRS | MediatR | Thin controllers, testable handlers, ValidationBehaviour pipeline |
| Permissions | Per-endpoint `[Authorize(Policy = "...")]` | No endpoint left at bare `[Authorize]`; constants owned by each module's Application layer |
| HTTP resilience | `Microsoft.Extensions.Http.Resilience` | Retry(3, exponential) + AttemptTimeout(8s) on all external API clients |
| Logging | Serilog → Console + rolling file | Structured logs; 401/403 logged at Warning with user identity; 500s at Error |

---

## Modules

### Auth
Handles login and logout. Validates credentials, issues a signed JWT stored in an httpOnly cookie. Permissions are embedded as claims in the token — no DB call on subsequent requests.

### Navigation
Returns the sidebar structure filtered to the current user's permissions. Result is cached per user (5-minute TTL) in `IMemoryCache`.

### Dashboard
Returns the dashboard cards the current user is allowed to see, filtered by permission claims.

### ClinicalAttachment
Proxies three endpoints on the external ClinicalAttachment API: ICP user lookup, user existence check, and facilities/speciality check. Each endpoint requires its own specific permission.

### PatientServices
Proxies the external PatientServices API: patient search, medical report retrieval, and sick leave retrieval. Each endpoint requires its own specific permission.

---

## Permission System

Permissions are stored in SQL Server (`auth.Permissions`) and assigned to roles (`auth.Roles`) via a join table. Users have roles. On login the full permission set is embedded as `"permission"` claims in the JWT.

Each module owns its permission string constants:
```csharp
// ClinicalAttachment.Application/Security/ClinicalAttachmentPermissions.cs
public static class ClinicalAttachmentPermissions
{
    public const string IcpView = "clinicalattachment.icp.view";
    // ...
    public static IReadOnlyCollection<string> All { get; } = [ IcpView, ... ];
}
```

Policies are registered at startup by iterating each module's `All` collection. Controllers use:
```csharp
[Authorize(Policy = ClinicalAttachmentPermissions.IcpView)]
```

---

## Technology Stack

| | |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 (DB-first) |
| Database | SQL Server |
| Mediator | MediatR 14 |
| Validation | FluentValidation 12 |
| Logging | Serilog (Console + File) |
| Auth | JWT Bearer + httpOnly cookie |
| HTTP Resilience | Microsoft.Extensions.Http.Resilience (Polly v8) |
| API Docs | Swashbuckle / Swagger |

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (local or remote)
- The schema must already exist (DB-first — no EF migrations)

### Configuration

The app requires a secret key that must **not** be in `appsettings.json`. Use user-secrets locally:
```bash
dotnet user-secrets set "Jwt:SecretKey" "your-secret-key-minimum-32-characters" \
  --project src/ErrorManagement.Api
```

Update `appsettings.json` with your connection string and allowed origins. The development override file (`appsettings.Development.json`) sets `SecureOnly: false` and `SameSiteStrict: false` for local HTTP.

### Run
```bash
cd src/ErrorManagement.Api
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` in Development.

### Seed

To seed the initial admin user and permissions, uncomment the seeder call in `Program.cs`:
```csharp
await AuthDbSeeder.SeedAsync(app.Services);
```

Run once, then comment it out again. The seeder is idempotent — safe to run multiple times.

---

## Project Structure
```
src/
├── ErrorManagement.Api/              # Host — Program.cs, middleware, CurrentUser
├── ErrorManagement.Shared/           # Shared abstractions — no dependencies
│   ├── Exceptions/                   # AppException hierarchy
│   ├── Http/                         # HttpResilienceExtensions
│   └── interfaces/                   # ICurrentUser
└── Modules/
    ├── Auth/
    │   ├── ErrorManagement.Auth.Application/
    │   │   ├── Behaviours/           # ValidationBehaviour (MediatR pipeline)
    │   │   ├── Interfaces/           # IAuthUser, ICookieTokenService, IJwtTokenService...
    │   │   └── Login/                # LoginCommand, LoginHandler, LoginValidator, LoginResponse
    │   ├── ErrorManagement.Auth.Infrastructure/
    │   │   ├── Persistence/
    │   │   │   └── Generated/        # EF scaffolded entities + .Partial.cs domain extensions
    │   │   ├── Repositories/
    │   │   ├── Seed/                 # AuthDbSeeder
    │   │   └── Services/             # JwtTokenService, PasswordHasher, CookieTokenService
    │   └── ErrorManagement.Auth.Api/
    ├── Navigation/...
    ├── Dashboard/...
    ├── ClinicalAttachment/...
    └── PatientServices/...
```

---

## Security Notes

- JWT secret must be set via user-secrets or environment variable — never committed
- `SecureOnly: true` by default — cookies sent over HTTPS only
- All endpoints require a specific permission — no bare `[Authorize]` on action methods
- Login returns the same error for wrong username and wrong password (prevents user enumeration)
- Security headers set on every response: `X-Content-Type-Options`, `X-Frame-Options: DENY`, `Referrer-Policy`
- Rate limiting: 300 requests/minute per IP

---

## License

Internal use only.
