using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TelephelyService : ITelephelyService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public TelephelyService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Telephely>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Telephelyek.Include(t => t.Ugyfel).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(t => t.Ugyfel != null && cegIds.Contains(t.Ugyfel.CegId));
        }

        return await query.OrderBy(t => t.Nev).ToListAsync();
    }

    public async Task<Telephely?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Telephelyek.Include(t => t.Ugyfel).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(t => t.Ugyfel != null && cegIds.Contains(t.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Telephely> CreateAsync(Telephely telephely)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ugyfel = await context.Ugyfelek.FindAsync(telephely.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (ugyfel == null || !cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága telephely létrehozásához ennél az ügyfélnél.");
        }

        telephely.Ugyfel = null!;
        telephely.Letrehozva = DateTime.UtcNow;
        context.Telephelyek.Add(telephely);
        await context.SaveChangesAsync();
        return telephely;
    }

    public async Task<Telephely> UpdateAsync(Telephely telephely)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.Telephelyek.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == telephely.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a telephely módosításához.");
        }

        existing.Nev = telephely.Nev;
        existing.Cim = telephely.Cim;
        existing.Kapcsolattarto = telephely.Kapcsolattarto;
        existing.Telefon = telephely.Telefon;
        existing.Email = telephely.Email;
        existing.Aktiv = telephely.Aktiv;
        existing.UgyfelId = telephely.UgyfelId;
        existing.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var telephely = await context.Telephelyek.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == id);
        if (telephely is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(telephely.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a telephely törléséhez.");
        }

        context.Telephelyek.Remove(telephely);
        await context.SaveChangesAsync();
    }

    public async Task<List<Telephely>> GetByUgyfelIdAsync(int ugyfelId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Telephelyek
            .Where(t => t.UgyfelId == ugyfelId)
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }
}