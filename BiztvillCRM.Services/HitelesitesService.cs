using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HitelesitesService : IHitelesitesService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public HitelesitesService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Hitelesites>> GetAllAsync()
    {
        var query = _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(h => h.Ugyfel != null && cegIds.Contains(h.Ugyfel.CegId));
        }

        return await query.OrderByDescending(h => h.Datum).ToListAsync();
    }

    public async Task<Hitelesites?> GetByIdAsync(int id)
    {
        var query = _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(h => h.Ugyfel != null && cegIds.Contains(h.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hitelesites> CreateAsync(Hitelesites hitelesites)
    {
        hitelesites.Letrehozva = DateTime.UtcNow;
        
        // Lejárat dátum automatikus számítása az eszköztípus alapján
        await SzamolLejaratDatumAsync(hitelesites);
        
        _context.Hitelesitesek.Add(hitelesites);
        await _context.SaveChangesAsync();
        return hitelesites;
    }

    public async Task<Hitelesites> UpdateAsync(Hitelesites hitelesites)
    {
        var existing = await _context.Hitelesitesek.FindAsync(hitelesites.Id)
            ?? throw new InvalidOperationException("Nem található.");
        
        existing.UgyfelId = hitelesites.UgyfelId;
        existing.TelephelyId = hitelesites.TelephelyId;
        existing.EszkozTipusId = hitelesites.EszkozTipusId;
        existing.HatosagId = hitelesites.HatosagId;
        existing.Darabszam = hitelesites.Darabszam;
        existing.Datum = hitelesites.Datum;
        existing.LejaratDatum = hitelesites.LejaratDatum;
        existing.HitelesitesStatusz = hitelesites.HitelesitesStatusz;
        existing.Megjegyzes = hitelesites.Megjegyzes;
        existing.EgyediLejaratok = hitelesites.EgyediLejaratok;
        existing.EszkozAzonosito = hitelesites.EszkozAzonosito;        // ← ez hiányzott
        existing.CsoportTagLejaratok = hitelesites.CsoportTagLejaratok; // ← ez hiányzott
        existing.Modositva = DateTime.UtcNow;
        
        await SzamolLejaratDatumAsync(existing);
        
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var hitelesites = await _context.Hitelesitesek.FindAsync(id);
        if (hitelesites is not null)
        {
            _context.Hitelesitesek.Remove(hitelesites);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Automatikusan kiszámolja a lejárat dátumát az eszköztípus hitelesítési időtartama alapján.
    /// Csak akkor számolja, ha nincs már megadott lejárati dátum (manuális felülírás védelme).
    /// </summary>
    private async Task SzamolLejaratDatumAsync(Hitelesites hitelesites)
    {
        // Ha már van lejárati dátum (manuálisan állította be a felhasználó), nem írjuk felül
        if (hitelesites.LejaratDatum.HasValue) return;

        var eszkozTipus = await _context.EszkozTipusok.FindAsync(hitelesites.EszkozTipusId);
        if (eszkozTipus != null && eszkozTipus.HitelesitesiIdotartamHonap > 0)
        {
            hitelesites.LejaratDatum = hitelesites.Datum.AddMonths(eszkozTipus.HitelesitesiIdotartamHonap);
        }
    }
}