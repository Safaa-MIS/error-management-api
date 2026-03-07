using System;

namespace ErrorManagement.Auth.Infrastructure.Persistence;

public partial class Role
{
    public static Role Create(string name)
        => new() { Id = Guid.NewGuid(), Name = name };
}