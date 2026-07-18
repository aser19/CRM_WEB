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
            .Include(h => h.Munkaszam)
            .Where(h => h.Aktiv) // Csak az aktívak
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(h => h.Ugyfel != null && cegIds.Contains(h.Ugyfel.CegId));
        }

        return await query.OrderByDescending(h => h.Datum).ToListAsync();
    }

    public async Task<List<Hitelesites>> GetInaktivakAsync()
    {
        var query = _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .Include(h => h.Munkaszam)
            .Where(h => !h.Aktiv) // Csak az inaktívak
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
            .Include(h => h.Munkaszam)
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
        existing.MunkaszamId = hitelesites.MunkaszamId;
        existing.Darabszam = hitelesites.Darabszam;
        existing.Datum = hitelesites.Datum;
        existing.LejaratDatum = hitelesites.LejaratDatum;
        existing.HitelesitesStatusz = hitelesites.HitelesitesStatusz;
        existing.Megjegyzes = hitelesites.Megjegyzes;
        existing.EgyediLejaratok = hitelesites.EgyediLejaratok;
        existing.EszkozAzonosito = hitelesites.EszkozAzonosito;
        existing.CsoportTagLejaratok = hitelesites.CsoportTagLejaratok;
        existing.MunkalapPath = hitelesites.MunkalapPath;
        existing.BizonyitvanyPath = hitelesites.BizonyitvanyPath;
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

    public async Task<Hitelesites?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int eszkozTipusId, string? eszkozAzonosito, DateTime ujHitelesDatum)
    {
        var query = _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Where(h => h.UgyfelId == ugyfelId
                        && h.TelephelyId == telephelyId
                        && h.EszkozTipusId == eszkozTipusId
                        && h.Aktiv);

        // EszkozAzonosito egyezés ellenőrzése (null is számít)
        if (string.IsNullOrWhiteSpace(eszkozAzonosito))
            query = query.Where(h => string.IsNullOrEmpty(h.EszkozAzonosito));
        else
            query = query.Where(h => h.EszkozAzonosito == eszkozAzonosito);

        var regi = await query.FirstOrDefaultAsync();
        if (regi == null) return null;

        // Ellenőrizzük, hogy az új hitelesítés 40 napon belül van-e a régi lejáratához képest
        if (regi.LejaratDatum.HasValue)
        {
            var kulonbseg = Math.Abs((ujHitelesDatum - regi.LejaratDatum.Value).TotalDays);
            if (kulonbseg <= 40)
            {
                return regi;
            }
        }

        return null;
    }

    public async Task InaktivvaTesz(int hitelesitesId)
    {
        var hitelesites = await _context.Hitelesitesek.FindAsync(hitelesitesId);
        if (hitelesites != null)
        {
            hitelesites.Aktiv = false;
            hitelesites.Modositva = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
