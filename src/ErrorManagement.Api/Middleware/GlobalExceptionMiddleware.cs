using ErrorManagement.Shared.Exceptions;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ErrorManagement.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, message) = ex switch
        {
            ValidationException ve => (400, string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedException => (401, ex.Message),
            ForbiddenException => (403, ex.Message),
            NotFoundException => (404, ex.Message),
            ConflictException => (409, ex.Message),
            AppException ae => (ae.StatusCode, ae.Message),
            _ => (500, "An unexpected error occurred.")
        };

        // Log based on status code
        switch (status)
        {
            case >= 500:
                _logger.LogError(ex,
                    "Unhandled exception {Status} at {Method} {Path}",
                    status, ctx.Request.Method, ctx.Request.Path);
                break;

            case 401:
                // Warning — could indicate brute force, expired tokens, or misconfiguration
                _logger.LogWarning(
                    "Unauthorized {Status} at {Method} {Path} — {Message}",
                    status, ctx.Request.Method, ctx.Request.Path, ex.Message);
                break;

            case 403:
                _logger.LogWarning(
                    "Forbidden {Status} at {Method} {Path} — User={User} Message={Message}",
                    status, ctx.Request.Method, ctx.Request.Path,
                    ctx.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? "anonymous",
                    ex.Message);                
                break;
        }
        // 400, 404, 409 are not logged — normal business validation, no noise

        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(
            new { message, traceId = ctx.TraceIdentifier },
            cancellationToken: ctx.RequestAborted);
    }
}