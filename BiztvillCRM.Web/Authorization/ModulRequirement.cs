using BiztvillCRM.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BiztvillCRM.Web.Authorization;

/// <summary>
/// Authorization requirement egy adott modulhoz.
/// </summary>
public class ModulRequirement : IAuthorizationRequirement
{
    public ModulJogosultsag RequiredModule { get; }
    public bool AdminOnly { get; }

    public ModulRequirement(ModulJogosultsag requiredModule, bool adminOnly = false)
    {
        RequiredModule = requiredModule;
        AdminOnly = adminOnly;
    }
}