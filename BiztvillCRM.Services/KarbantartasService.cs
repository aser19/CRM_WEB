using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KarbantartasService : IKarbantartasService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public KarbantartasService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Karbantartas>> GetAllAsync()
    {
        var query = _context.Karbantartasok
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .Where(k => k.Aktiv)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(k => cegIds.Contains(k.CegId));
        }

        return await query.OrderByDescending(k => k.Datum).ToListAsync();
    }

    public async Task<Karbantartas?> GetByIdAsync(int id)
    {
        var query = _context.Karbantartasok
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(k => cegIds.Contains(k.CegId));
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Karbantartas> CreateAsync(Karbantartas karbantartas)
    {
        // CegId az ügyfél alapján kerül meghatározásra, nem a bejelentkezett felhasználó elsődleges cége alapján
        var ugyfel = await _context.Ugyfelek.FindAsync(karbantartas.UgyfelId)
            ?? throw new InvalidOperationException("Az ügyfél nem található.");

        karbantartas.CegId = ugyfel.CegId;
        karbantartas.Letrehozva = DateTime.UtcNow;
        await SzamolKovetkezoDatumAsync(karbantartas);
        _context.Karbantartasok.Add(karbantartas);
        await _context.SaveChangesAsync();
        return karbantartas;
    }

    public async Task<Karbantartas> UpdateAsync(Karbantartas karbantartas)
    {
        var existing = await _context.Karbantartasok.FirstOrDefaultAsync(k => k.Id == karbantartas.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága.");
        }

        existing.UgyfelId = karbantartas.UgyfelId;
        existing.TelephelyId = karbantartas.TelephelyId;
        existing.KarbantartasTipusId = karbantartas.KarbantartasTipusId;
        existing.Datum = karbantartas.Datum;
        existing.KovetkezoDatum = karbantartas.KovetkezoDatum;
        existing.Leiras = karbantartas.Leiras;
        existing.Elvegzo = karbantartas.Elvegzo;
        existing.Elvegezve = karbantartas.Elvegezve;
        existing.Modositva = DateTime.UtcNow;
        await SzamolKovetkezoDatumAsync(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var karbantartas = await _context.Karbantartasok.FirstOrDefaultAsync(k => k.Id == id);
        if (karbantartas is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(karbantartas.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága.");
        }

        _context.Karbantartasok.Remove(karbantartas);
        await _context.SaveChangesAsync();
    }

    private async Task SzamolKovetkezoDatumAsync(Karbantartas karbantartas)
    {
        var tipus = await _context.KarbantartasTipusok.FindAsync(karbantartas.KarbantartasTipusId);
        
        if (tipus is null || tipus.Eseti)
        {
            // Eseti karbantartás - nincs következő dátum
            karbantartas.KovetkezoDatum = null;
        }
        else
        {
            // Ismétlődő karbantartás - számoljuk a következő dátumot
            karbantartas.KovetkezoDatum = karbantartas.Datum.AddMonths(tipus.IsmetlodesHonap);
        }
    }

    public async Task<List<Karbantartas>> GetInaktivakAsync()
    {
        var query = _context.Karbantartasok
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .Where(k => !k.Aktiv)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(k => cegIds.Contains(k.CegId));
        }

        return await query.OrderByDescending(k => k.Datum).ToListAsync();
    }

    public async Task<Karbantartas?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int karbantartasTipusId, DateTime ujDatum)
    {
        var cegIds = _tenantService.IsInRole(FelhasznaloSzerepkor.Admin)
            ? null
            : await _tenantService.GetElerhhetoCegIdsAsync();

        var query = _context.Karbantartasok
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .Where(k => k.Aktiv
                        && k.UgyfelId == ugyfelId
                        && k.TelephelyId == telephelyId
                        && k.KarbantartasTipusId == karbantartasTipusId);

        if (cegIds != null)
            query = query.Where(k => cegIds.Contains(k.CegId));

        var regi = await query.FirstOrDefaultAsync();
        if (regi == null || regi.KovetkezoDatum == null)
            return null;

        var kulonbseg = Math.Abs((ujDatum - regi.KovetkezoDatum.Value).TotalDays);
        return kulonbseg <= 40 ? regi : null;
    }

    public async Task InaktivvaTesz(int karbantartasId)
    {
        var karbantartas = await _context.Karbantartasok.FindAsync(karbantartasId);
        if (karbantartas != null)
        {
            karbantartas.Aktiv = false;
            await _context.SaveChangesAsync();
        }
    }
}
