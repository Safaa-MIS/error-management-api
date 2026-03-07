using System.Collections.Generic;
using System;

namespace ErrorManagement.Shared.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Username { get; }
    string DisplayName { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permission);
    IEnumerable<string> GetPermissions();
}
