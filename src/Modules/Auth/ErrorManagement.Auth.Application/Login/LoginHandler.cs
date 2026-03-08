using ErrorManagement.Auth.Application.Interfaces;
using ErrorManagement.Shared.Exceptions;
using MediatR;

namespace ErrorManagement.Auth.Application.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginHandler(IUserRepository users, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByUsernameAsync(
            request.Username.ToLowerInvariant().Trim(), ct);

        // Same error for wrong username AND wrong password — prevents user enumeration attacks
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        if (!user.IsActive)
            throw new ForbiddenException("Account is inactive. Contact your administrator.");

        user.RecordLogin();
        await _users.SaveChangesAsync(ct);

        return new LoginResponse(
            DisplayName: user.DisplayName,
            Permissions: user.GetPermissions().ToList(),
            AccessToken: _jwt.GenerateAccessToken(user),
            ExpiresInSeconds: _jwt.AccessTokenExpiresInSeconds);
    }
}