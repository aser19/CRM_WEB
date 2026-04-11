using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BiztvillCRM.Web.Authorization;

/// <summary>
/// Handler a modul jogosultságok ellenőrzéséhez.
/// </summary>
public class ModulAuthorizationHandler : AuthorizationHandler<ModulRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public ModulAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ModulRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var modulService = scope.ServiceProvider.GetRequiredService<IModulJogosultsagService>();

        // Admin mindig hozzáfér
        if (tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            context.Succeed(requirement);
            return;
        }

        // Ha admin-only, és nem admin -> megtagadva
        if (requirement.AdminOnly)
        {
            return;
        }

        // Modul jogosultság ellenőrzése
        await modulService.InitializeAsync();
        var aktivModulok = modulService.GetAktivModulok();

        if (aktivModulok.HasFlag(requirement.RequiredModule))
        {
            context.Succeed(requirement);
        }
    }
}