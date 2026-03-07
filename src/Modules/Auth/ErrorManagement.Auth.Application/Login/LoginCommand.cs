using MediatR;

namespace ErrorManagement.Auth.Application.Login;

public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;