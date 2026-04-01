using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

/// <summary>
/// Ügyfél CRUD műveletek implementációja EF Core segítségével.
/// Multi-tenant: csak a bejelentkezett felhasználó cégéhez tartozó ügyfeleket kezeli.
/// </summary>
public class UgyfelService : IUgyfelService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public UgyfelService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    /// <inheritdoc/>
    public async Task<List<Ugyfel>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        // Admin látja az összeset
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await _context.Ugyfelek
                .Include(u => u.Telephelyek)
                .Include(u => u.Ceg)
                .OrderBy(u => u.Nev)
                .ToListAsync();
        }

        return await _context.Ugyfelek
            .Where(u => u.CegId == cegId)
            .Include(u => u.Telephelyek)
            .OrderBy(u => u.Nev)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Ugyfel?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Ugyfelek
            .Include(u => u.Telephelyek)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(u => u.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Ugyfel> CreateAsync(Ugyfel ugyfel)
    {
        ugyfel.Letrehozva = DateTime.UtcNow;
        
        var isAdmin = _tenantService.IsInRole(FelhasznaloSzerepkor.Admin);
        var currentCegId = _tenantService.GetCurrentCegId();
        
        // DEBUG - ezt később eltávolíthatod
        Console.WriteLine($"[CreateAsync] isAdmin: {isAdmin}, currentCegId: {currentCegId}, ugyfel.CegId: {ugyfel.CegId}");
        
        // Admin esetén: ha explicit megadtak CegId-t, azt használjuk; egyébként a sajátját
        // Nem-Admin esetén: mindig a saját cégét használjuk (biztonsági okokból)
        if (isAdmin && ugyfel.CegId > 0)
        {
            // Admin explicit megadta a céget - megtartjuk
        }
        else
        {
            // Nem-Admin vagy nincs megadva cég: a bejelentkezett felhasználó cégét használjuk
            if (currentCegId == 0)
            {
                throw new InvalidOperationException("Nincs érvényes cég hozzárendelve a felhasználóhoz!");
            }
            ugyfel.CegId = currentCegId;
        }
        
        Console.WriteLine($"[CreateAsync] VÉGSŐ ugyfel.CegId: {ugyfel.CegId}");
        
        _context.Ugyfelek.Add(ugyfel);
        await _context.SaveChangesAsync();
        
        // Ellenőrizzük, hogy valóban mentettük-e
        Console.WriteLine($"[CreateAsync] Mentés után - ugyfel.Id: {ugyfel.Id}, ugyfel.CegId: {ugyfel.CegId}");
        
        return ugyfel;
    }

    /// <inheritdoc/>
    public async Task<Ugyfel> UpdateAsync(Ugyfel ugyfel)
    {
        Console.WriteLine($"[UpdateAsync] START - Id: {ugyfel.Id}, CegId: {ugyfel.CegId}");
        
        var cegId = _tenantService.GetCurrentCegId();
        var isAdmin = _tenantService.IsInRole(FelhasznaloSzerepkor.Admin);
        
        var existing = await _context.Ugyfelek.FindAsync(ugyfel.Id)
            ?? throw new InvalidOperationException("Nem található.");

        Console.WriteLine($"[UpdateAsync] existing.CegId: {existing.CegId}, isAdmin: {isAdmin}");

        // Ellenőrzés: csak saját cég ügyfele módosítható (nem admin esetén)
        if (!isAdmin && existing.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél módosításához.");
        }

        existing.Nev = ugyfel.Nev;
        existing.Adoszam = ugyfel.Adoszam;
        existing.Cim = ugyfel.Cim;
        existing.Email = ugyfel.Email;
        existing.Telefon = ugyfel.Telefon;
        existing.UgyfelTipus = ugyfel.UgyfelTipus;
        existing.Aktiv = ugyfel.Aktiv;
        existing.Modositva = DateTime.UtcNow;
        
        // FONTOS: Admin módosíthatja a CegId-t!
        if (isAdmin)
        {
            Console.WriteLine($"[UpdateAsync] Admin - CegId módosítása: {existing.CegId} -> {ugyfel.CegId}");
            existing.CegId = ugyfel.CegId;
        }

        await _context.SaveChangesAsync();
        
        Console.WriteLine($"[UpdateAsync] KÉSZ - Id: {existing.Id}, CegId: {existing.CegId}");
        return existing;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        
        if (ugyfel is not null)
        {
            // Ellenőrzés: csak saját cég ügyfele törölhető
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél törléséhez.");
            }

            _context.Ugyfelek.Remove(ugyfel);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<KapcsolodoAdatok> GetKapcsolodoAdatokAsync(int ugyfelId)
    {
        var telephelyek = await _context.Telephelyek.CountAsync(t => t.UgyfelId == ugyfelId);
        var meresek = await _context.Meresek.CountAsync(m => m.UgyfelId == ugyfelId);
        var eszkozok = await _context.Eszkozok.CountAsync(e => e.UgyfelId == ugyfelId);
        var tanusitvanyok = await _context.Tanusitvanyok.CountAsync(t => t.UgyfelId == ugyfelId);

        return new KapcsolodoAdatok(telephelyek, meresek, eszkozok, tanusitvanyok);
    }

    public async Task DeleteWithRelatedDataAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(id);

        if (ugyfel is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél törléséhez.");
        }

        // Kapcsolódó adatok törlése sorrendben (idegen kulcs függőségek miatt)
        // 1. Mérések (UgyfelId + TelephelyId)
        var meresek = await _context.Meresek.Where(m => m.UgyfelId == id).ToListAsync();
        _context.Meresek.RemoveRange(meresek);

        // 2. Tanúsítványok
        var tanusitvanyok = await _context.Tanusitvanyok.Where(t => t.UgyfelId == id).ToListAsync();
        _context.Tanusitvanyok.RemoveRange(tanusitvanyok);

        // 3. Eszközök (karbantartások és kalibrációk kapcsolódhatnak)
        var eszkozok = await _context.Eszkozok.Where(e => e.UgyfelId == id).ToListAsync();
        foreach (var eszkoz in eszkozok)
        {
            // Hitelesitesek már NEM kapcsolódnak közvetlenül az eszközhöz (EszkozTipusId-t használnak)
            // Ezért itt nem kell hitelesítéseket törölni

            var karbantartasok = await _context.Karbantartasok.Where(k => k.EszkozId == eszkoz.Id).ToListAsync();
            _context.Karbantartasok.RemoveRange(karbantartasok);

            var kalibraciok = await _context.Kalibraciok.Where(k => k.EszkozId == eszkoz.Id).ToListAsync();
            _context.Kalibraciok.RemoveRange(kalibraciok);
        }
        _context.Eszkozok.RemoveRange(eszkozok);

        // 4. Telephelyek
        var telephelyek = await _context.Telephelyek.Where(t => t.UgyfelId == id).ToListAsync();
        _context.Telephelyek.RemoveRange(telephelyek);

        // 5. Végül az ügyfél
        _context.Ugyfelek.Remove(ugyfel);

        await _context.SaveChangesAsync();
    }
}