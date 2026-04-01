using System.Security.Claims;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Web.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentCegId()
    {
        var cegIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("CegId")?.Value;
        var result = int.TryParse(cegIdClaim, out var cegId) ? cegId : 0;
        
        // DEBUG
        Console.WriteLine($"[TenantService.GetCurrentCegId] cegIdClaim: '{cegIdClaim}', result: {result}");
        
        return result;
    }

    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    }

    public bool IsInRole(FelhasznaloSzerepkor role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role.ToString()) ?? false;
    }
}