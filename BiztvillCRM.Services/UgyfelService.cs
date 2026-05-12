using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class UgyfelService : IUgyfelService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public UgyfelService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Ugyfel>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await context.Ugyfelek
                .Include(u => u.Telephelyek)
                .Include(u => u.Ceg)
                .OrderBy(u => u.Nev)
                .ToListAsync();
        }

        var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
        return await context.Ugyfelek
            .Where(u => cegIds.Contains(u.CegId))
            .Include(u => u.Telephelyek)
            .OrderBy(u => u.Nev)
            .ToListAsync();
    }

    public async Task<Ugyfel?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Ugyfelek.Include(u => u.Telephelyek).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(u => cegIds.Contains(u.CegId));
        }

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Ugyfel> CreateAsync(Ugyfel ugyfel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        ugyfel.Letrehozva = DateTime.UtcNow;
        
        var isAdmin = _tenantService.IsInRole(FelhasznaloSzerepkor.Admin);
        var currentCegId = _tenantService.GetCurrentCegId();

        if (!(isAdmin && ugyfel.CegId > 0))
        {
            ugyfel.CegId = currentCegId;
        }

        ugyfel.Ceg = null!;
        ugyfel.Telephelyek = new List<Telephely>();
        
        context.Ugyfelek.Add(ugyfel);
        await context.SaveChangesAsync();
        return ugyfel;
    }

    public async Task<Ugyfel> UpdateAsync(Ugyfel ugyfel)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.Ugyfelek.FirstOrDefaultAsync(u => u.Id == ugyfel.Id)
            ?? throw new InvalidOperationException("Ügyfél nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél módosításához.");
        }

        existing.Nev = ugyfel.Nev;
        existing.Adoszam = ugyfel.Adoszam;
        existing.Cim = ugyfel.Cim;
        existing.Email = ugyfel.Email;
        existing.Telefon = ugyfel.Telefon;
        existing.UgyfelTipus = ugyfel.UgyfelTipus;
        existing.Tevekenyseg = ugyfel.Tevekenyseg;
        existing.Aktiv = ugyfel.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel.CegId > 0)
            existing.CegId = ugyfel.CegId;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ugyfel = await context.Ugyfelek.FindAsync(id);
        if (ugyfel is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél törléséhez.");
        }

        context.Ugyfelek.Remove(ugyfel);
        await context.SaveChangesAsync();
    }

    public async Task<KapcsolodoAdatok> GetKapcsolodoAdatokAsync(int ugyfelId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var telephelyek = await context.Telephelyek.CountAsync(t => t.UgyfelId == ugyfelId);
        var meresek = await context.Meresek.CountAsync(m => m.UgyfelId == ugyfelId);
        var eszkozok = await context.Eszkozok.CountAsync(e => e.UgyfelId == ugyfelId);
        var tanusitvanyok = await context.Tanusitvanyok.CountAsync(t => t.UgyfelId == ugyfelId);
        var hitelesitesek = await context.Hitelesitesek.CountAsync(h => h.UgyfelId == ugyfelId);
        var oktatasok = await context.MunkavedelmiOktatasok.CountAsync(o => o.UgyfelId == ugyfelId);
        
        // Karbantartások a telephelyeken keresztül
        var telephelyIds = await context.Telephelyek
            .Where(t => t.UgyfelId == ugyfelId)
            .Select(t => t.Id)
            .ToListAsync();
        var karbantartasok = await context.Karbantartasok
            .CountAsync(k => telephelyIds.Contains(k.TelephelyId));

        return new KapcsolodoAdatok(
            telephelyek, 
            meresek, 
            eszkozok, 
            tanusitvanyok, 
            hitelesitesek, 
            oktatasok, 
            karbantartasok
        );
    }

    public async Task DeleteWithRelatedDataAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ugyfel = await context.Ugyfelek.FindAsync(id);
        if (ugyfel is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél törléséhez.");
        }

        // 1. Mérések törlése
        var meresek = await context.Meresek.Where(m => m.UgyfelId == id).ToListAsync();
        context.Meresek.RemoveRange(meresek);

        // 2. Hitelesítések törlése
        var hitelesitesek = await context.Hitelesitesek.Where(h => h.UgyfelId == id).ToListAsync();
        context.Hitelesitesek.RemoveRange(hitelesitesek);

        // 3. Munkavédelmi oktatások és résztvevőik törlése
        var oktatasok = await context.MunkavedelmiOktatasok
            .Include(o => o.Resztvevok)
            .Where(o => o.UgyfelId == id)
            .ToListAsync();
        foreach (var o in oktatasok)
            context.MunkavedelmiOktatasResztvevok.RemoveRange(o.Resztvevok);
        context.MunkavedelmiOktatasok.RemoveRange(oktatasok);

        // 4. Tanúsítványok törlése
        var tanusitvanyok = await context.Tanusitvanyok.Where(t => t.UgyfelId == id).ToListAsync();
        context.Tanusitvanyok.RemoveRange(tanusitvanyok);

        // 5. Eszközök és kalibrációk törlése
        var eszkozok = await context.Eszkozok.Where(e => e.UgyfelId == id).ToListAsync();
        foreach (var eszkoz in eszkozok)
        {
            var kalibraciok = await context.Kalibraciok.Where(k => k.EszkozId == eszkoz.Id).ToListAsync();
            context.Kalibraciok.RemoveRange(kalibraciok);
        }
        context.Eszkozok.RemoveRange(eszkozok);

        // 6. Telephelyek és karbantartások törlése
        var telephelyIds = await context.Telephelyek
            .Where(t => t.UgyfelId == id)
            .Select(t => t.Id)
            .ToListAsync();
        var karbantartasok = await context.Karbantartasok
            .Where(k => telephelyIds.Contains(k.TelephelyId))
            .ToListAsync();
        context.Karbantartasok.RemoveRange(karbantartasok);

        var telephelyek = await context.Telephelyek.Where(t => t.UgyfelId == id).ToListAsync();
        context.Telephelyek.RemoveRange(telephelyek);

        // 7. Végül az ügyfél törlése
        context.Ugyfelek.Remove(ugyfel);
        
        await context.SaveChangesAsync();
    }
}