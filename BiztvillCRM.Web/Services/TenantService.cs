using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiztvillCRM.Web.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CrmDbContext _db;

    public TenantService(IHttpContextAccessor httpContextAccessor, CrmDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
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
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetCurrentUserName()
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public bool IsInRole(FelhasznaloSzerepkor role)
        => _httpContextAccessor.HttpContext?.User?.IsInRole(role.ToString()) ?? false;

    public async Task<List<int>> GetElerhhetoCegIdsAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return new();

        // Elsődleges cég
        var elsodleges = GetCurrentCegId();

        // FelhasznaloCegek táblából a többi
        var tobbi = await _db.FelhasznaloCegek
            .Where(fc => fc.FelhasznaloId == userId)
            .Select(fc => fc.CegId)
            .ToListAsync();

        // Összesítés, duplikátum nélkül
        return tobbi.Append(elsodleges).Distinct().Where(id => id > 0).ToList();
    }
}