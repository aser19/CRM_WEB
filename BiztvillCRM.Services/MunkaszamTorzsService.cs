using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MunkaszamTorzsService : IMunkaszamTorzsService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public MunkaszamTorzsService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Munkaszam>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await context.Munkaszamok
                .Include(m => m.Ceg)
                .OrderByDescending(m => m.Letrehozva)
                .ToListAsync();
        }

        var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
        return await context.Munkaszamok
            .Where(m => cegIds.Contains(m.CegId))
            .OrderByDescending(m => m.Letrehozva)
            .ToListAsync();
    }

    public async Task<Munkaszam?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Munkaszamok.AsQueryable();
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => cegIds.Contains(m.CegId));
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Munkaszam> CreateAsync(Munkaszam munkaszam)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        munkaszam.Letrehozva = DateTime.UtcNow;

        var isAdmin = _tenantService.IsInRole(FelhasznaloSzerepkor.Admin);
        var currentCegId = _tenantService.GetCurrentCegId();

        if (!(isAdmin && munkaszam.CegId > 0))
        {
            munkaszam.CegId = currentCegId;
        }

        munkaszam.Ceg = null;

        context.Munkaszamok.Add(munkaszam);
        await context.SaveChangesAsync();
        return munkaszam;
    }

    public async Task<Munkaszam> UpdateAsync(Munkaszam munkaszam)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.Munkaszamok.FindAsync(munkaszam.Id)
            ?? throw new InvalidOperationException("Munkaszám nem található.");

        existing.Szam = munkaszam.Szam;
        existing.Megnevezes = munkaszam.Megnevezes;
        existing.Aktiv = munkaszam.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.Munkaszamok.FindAsync(id);
        if (existing is not null)
        {
            context.Munkaszamok.Remove(existing);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Munkaszam>> KeresAsync(string kereses, int maxEredmeny = 20)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Munkaszamok.Where(m => m.Aktiv);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => cegIds.Contains(m.CegId));
        }
        else
        {
            var currentCegId = _tenantService.GetCurrentCegId();
            if (currentCegId > 0)
            {
                query = query.Where(m => m.CegId == currentCegId);
            }
        }

        if (!string.IsNullOrWhiteSpace(kereses))
        {
            query = query.Where(m => m.Szam.Contains(kereses) ||
                                      (m.Megnevezes != null && m.Megnevezes.Contains(kereses)));
        }

        return await query
            .OrderBy(m => m.Szam)
            .Take(maxEredmeny)
            .ToListAsync();
    }
}
