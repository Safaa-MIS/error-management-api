using System;

namespace ErrorManagement.Auth.Infrastructure.Persistence;

public partial class Permission
{
    public static Permission Create(string code, string module, string description)
        => new() { Id = Guid.NewGuid(), Code = code, Module = module, Description = description };
}