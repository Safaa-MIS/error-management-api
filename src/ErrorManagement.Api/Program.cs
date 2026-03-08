using ErrorManagement.Api;
using ErrorManagement.Api.Middleware;
using ErrorManagement.Auth.Infrastructure;
using ErrorManagement.Auth.Infrastructure.Seed;
using ErrorManagement.Auth.Infrastructure.Services;
using ErrorManagement.ClinicalAttachment.Application.Security;
using ErrorManagement.ClinicalAttachment.Infrastructure;
using ErrorManagement.Dashboard.Application.Security;
using ErrorManagement.Dashboard.Infrastructure;
using ErrorManagement.Navigation.Infrastructure;
using ErrorManagement.PatientServices.Application.Security;
using ErrorManagement.PatientServices.Infrastructure;
using ErrorManagement.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("logs/em-.log",
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 30));

    var jwtOpts = builder.Configuration
        .GetSection(JwtOptions.Section)
        .Get<JwtOptions>()
        ?? throw new InvalidOperationException("Jwt configuration section is missing.");

    jwtOpts.Validate();

    var cookieAuthOpts = builder.Configuration
    .GetSection(CookieAuthOptions.Section)
    .Get<CookieAuthOptions>()
    ?? new CookieAuthOptions();

    // CORS ──────────────────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? throw new InvalidOperationException("AllowedOrigins is not configured.");

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy => policy
      .WithOrigins(allowedOrigins)
          .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
    });

    // JWT Authentication — reads token from httpOnly cookie ─────────────────
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOpts.Issuer,
                ValidAudience = jwtOpts.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwtOpts.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            // Read JWT from httpOnly cookie em_at
            opts.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var cookie = ctx.Request.Cookies[cookieAuthOpts.AccessTokenName];
                    if (!string.IsNullOrEmpty(cookie)) ctx.Token = cookie;
                    return Task.CompletedTask;
                }
            };
        });

   // builder.Services.AddAuthorization();


    // Authorization — FallbackPolicy + one policy per permission ───────────
    // The composition root (Program.cs) is the only place that knows all
    // modules, so policy registration lives here — NOT in Shared.
    builder.Services.AddAuthorization(options =>
    {
        // Any endpoint without [AllowAnonymous] requires authentication.
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        foreach (var permission in DashboardPermissions.All
            .Concat(ClinicalAttachmentPermissions.All)
            .Concat(PatientServicesPermissions.All))
        {
            options.AddPolicy(permission, policy =>
                policy.RequireClaim("permission", permission));
        }
    });

    // Current user ──────────────────────────────────────────────────────────
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();

    // Rate Limiting — global, partitioned per IP ────────────────────────────
    builder.Services.AddRateLimiter(opts =>
    {
        opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 300,
                    QueueLimit = 0
                }));
    });

    // Modules ───────────────────────────────────────────────────────────────
    builder.Services.AddAuthModule(builder.Configuration);
    builder.Services.AddNavigationModule(builder.Configuration);
    builder.Services.AddDashboardModule(builder.Configuration);
    builder.Services.AddClinicalAttachmentModule(builder.Configuration);
    builder.Services.AddPatientServicesModule(builder.Configuration);

    // Controllers + Swagger ─────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Error Management API",
            Version = "v1"
        });

        c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Cookie,
            Name = cookieAuthOpts.AccessTokenName
        });
    });

    var app = builder.Build();

    // Middleware pipeline — ORDER MATTERS ───────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.Use(async (ctx, next) =>
    {
        ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
        ctx.Response.Headers["X-Frame-Options"] = "DENY";
        ctx.Response.Headers["X-XSS-Protection"] = "0";
        ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // HSTS: tell browsers to always use HTTPS for this domain
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseCors("Frontend");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Seed ──────────────────────────────────────────────────────────────────
  //await AuthDbSeeder.SeedAsync(app.Services);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}