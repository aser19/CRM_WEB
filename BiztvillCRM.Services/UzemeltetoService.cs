using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

/// <summary>
/// Üzemeltető modul service implementáció - admin által létrehozott sablonok és üzemeltető által rögzített adatok kezelése.
/// </summary>
public class UzemeltetoService : IUzemeltetoService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public UzemeltetoService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    // --- Sablonok (admin műveletei) ---

    public async Task<List<UzemeltetoSablon>> GetSablonokAsync()
    {
        var query = _context.UzemeltetoSablonok
            .Include(s => s.Ceg)
            .Include(s => s.LetrehozoFelhasznalo)
            .Include(s => s.Mezok.OrderBy(m => m.Sorrend))
            .Where(s => s.Aktiv)
            .AsQueryable();

        // Admin mindent lát, nincs szűrés
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            // Admin felhasználó minden aktív sablont lát
        }
        // Üzemeltető szerepkör esetén csak a hozzárendelt sablonokat láthatja
        else if (_tenantService.IsInRole(FelhasznaloSzerepkor.Uzemelteto))
        {
            var userId = _tenantService.GetCurrentUserId();
            var hozzarendeltSablonIds = await _context.UzemeltetoSablonFelhasznalok
                .Where(sf => sf.FelhasznaloId == userId && sf.Aktiv)
                .Select(sf => sf.UzemeltetoSablonId)
                .ToListAsync();

            query = query.Where(s => hozzarendeltSablonIds.Contains(s.Id));
        }
        else
        {
            // Egyéb szerepkörök (pl. Felhasználó, Cégadmin) csak a céghez tartozó sablonokat láthatják
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(s => cegIds.Contains(s.CegId));
        }

        return await query.OrderBy(s => s.Nev).ToListAsync();
    }

    public async Task<List<UzemeltetoSablon>> GetInaktivSablonokAsync()
    {
        var query = _context.UzemeltetoSablonok
            .Include(s => s.Ceg)
            .Include(s => s.LetrehozoFelhasznalo)
            .Include(s => s.Mezok.OrderBy(m => m.Sorrend))
            .Where(s => !s.Aktiv)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(s => cegIds.Contains(s.CegId));
        }

        return await query.OrderBy(s => s.Nev).ToListAsync();
    }

    public async Task<UzemeltetoSablon?> GetSablonByIdAsync(int id)
    {
        var query = _context.UzemeltetoSablonok
            .Include(s => s.Ceg)
            .Include(s => s.LetrehozoFelhasznalo)
            .Include(s => s.Mezok.OrderBy(m => m.Sorrend))
            .AsQueryable();

        // Admin mindent lát
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            // Nincs szűrés
        }
        // Üzemeltető csak a hozzárendelt sablonokat látja
        else if (_tenantService.IsInRole(FelhasznaloSzerepkor.Uzemelteto))
        {
            var userId = _tenantService.GetCurrentUserId();
            var hozzarendeltSablonIds = await _context.UzemeltetoSablonFelhasznalok
                .Where(sf => sf.FelhasznaloId == userId && sf.Aktiv)
                .Select(sf => sf.UzemeltetoSablonId)
                .ToListAsync();

            query = query.Where(s => hozzarendeltSablonIds.Contains(s.Id));
        }
        else
        {
            // Egyéb szerepkörök csak a céghez tartozó sablonokat látják
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(s => cegIds.Contains(s.CegId));
        }

        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<UzemeltetoSablon> CreateSablonAsync(UzemeltetoSablon sablon)
    {
        sablon.Letrehozva = DateTime.UtcNow;
        sablon.CegId = _tenantService.GetCurrentCegId();
        sablon.LetrehozoFelhasznaloId = _tenantService.GetCurrentUserId();

        _context.UzemeltetoSablonok.Add(sablon);
        await _context.SaveChangesAsync();

        return sablon;
    }

    public async Task<UzemeltetoSablon> UpdateSablonAsync(UzemeltetoSablon sablon)
    {
        var existing = await _context.UzemeltetoSablonok.FindAsync(sablon.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Sablon nem található: {sablon.Id}");
        }

        // Tenant ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez a sablonhoz!");
            }
        }

        existing.Nev = sablon.Nev;
        existing.Leiras = sablon.Leiras;
        existing.JogszabalyiHivatkozas = sablon.JogszabalyiHivatkozas;
        existing.EllenorzesiIdoszakHonap = sablon.EllenorzesiIdoszakHonap;
        existing.Aktiv = sablon.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteSablonAsync(int id)
    {
        var sablon = await _context.UzemeltetoSablonok.FindAsync(id);
        if (sablon == null)
        {
            throw new InvalidOperationException($"Sablon nem található: {id}");
        }

        // Tenant ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(sablon.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez a sablonhoz!");
            }
        }

        sablon.Aktiv = false;
        sablon.Modositva = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<UzemeltetoSablonMezo> AddSablonMezoAsync(UzemeltetoSablonMezo mezo)
    {
        // Ellenőrzés: létezik-e a sablon
        var sablon = await _context.UzemeltetoSablonok.FindAsync(mezo.UzemeltetoSablonId);
        if (sablon == null)
        {
            throw new InvalidOperationException($"Sablon nem található: {mezo.UzemeltetoSablonId}");
        }

        // Tenant ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(sablon.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez a sablonhoz!");
            }
        }

        _context.UzemeltetoSablonMezok.Add(mezo);
        await _context.SaveChangesAsync();

        return mezo;
    }

    public async Task<UzemeltetoSablonMezo> UpdateSablonMezoAsync(UzemeltetoSablonMezo mezo)
    {
        var existing = await _context.UzemeltetoSablonMezok
            .Include(m => m.UzemeltetoSablon)
            .FirstOrDefaultAsync(m => m.Id == mezo.Id);

        if (existing == null)
        {
            throw new InvalidOperationException($"Sablon mező nem található: {mezo.Id}");
        }

        // Tenant ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.UzemeltetoSablon.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez a sablonhoz!");
            }
        }

        existing.MezoNev = mezo.MezoNev;
        existing.MezoTipus = mezo.MezoTipus;
        existing.Kotelezo = mezo.Kotelezo;
        existing.Sorrend = mezo.Sorrend;
        existing.AlapErtek = mezo.AlapErtek;
        existing.Sugo = mezo.Sugo;
        existing.ValidaciosSzabaly = mezo.ValidaciosSzabaly;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteSablonMezoAsync(int mezoId)
    {
        var mezo = await _context.UzemeltetoSablonMezok
            .Include(m => m.UzemeltetoSablon)
            .FirstOrDefaultAsync(m => m.Id == mezoId);

        if (mezo == null)
        {
            throw new InvalidOperationException($"Sablon mező nem található: {mezoId}");
        }

        // Tenant ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(mezo.UzemeltetoSablon.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez a sablonhoz!");
            }
        }

        _context.UzemeltetoSablonMezok.Remove(mezo);
        await _context.SaveChangesAsync();
    }

    // --- Adatok (üzemeltető műveletei) ---

    public async Task<List<UzemeltetoAdat>> GetAdatokAsync()
    {
        var query = _context.UzemeltetoAdatok
            .Include(a => a.UzemeltetoSablon)
            .Include(a => a.Ceg)
            .Include(a => a.RogzitoFelhasznalo)
            .Where(a => a.Aktiv)
            .AsQueryable();

        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Uzemelteto))
        {
            // Üzemeltető csak a saját adatait láthatja
            var userId = _tenantService.GetCurrentUserId();
            query = query.Where(a => a.RogzitoFelhasznaloId == userId);
        }
        else if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            // Egyéb szerepkörök a céghez tartozó összes adatot láthatják
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(a => cegIds.Contains(a.CegId));
        }

        return await query.OrderByDescending(a => a.RogzitesDatum).ToListAsync();
    }

    public async Task<List<UzemeltetoAdat>> GetAdatokBySablonIdAsync(int sablonId)
    {
        var query = _context.UzemeltetoAdatok
            .Include(a => a.UzemeltetoSablon)
            .Include(a => a.Ceg)
            .Include(a => a.RogzitoFelhasznalo)
            .Where(a => a.UzemeltetoSablonId == sablonId && a.Aktiv)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(a => cegIds.Contains(a.CegId));
        }

        return await query.OrderByDescending(a => a.RogzitesDatum).ToListAsync();
    }

    public async Task<UzemeltetoAdat?> GetAdatByIdAsync(int id)
    {
        var query = _context.UzemeltetoAdatok
            .Include(a => a.UzemeltetoSablon)
                .ThenInclude(s => s.Mezok.OrderBy(m => m.Sorrend))
            .Include(a => a.Ceg)
            .Include(a => a.RogzitoFelhasznalo)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(a => cegIds.Contains(a.CegId));
        }

        return await query.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<UzemeltetoAdat> CreateAdatAsync(UzemeltetoAdat adat)
    {
        adat.Letrehozva = DateTime.UtcNow;
        adat.CegId = _tenantService.GetCurrentCegId();
        adat.RogzitoFelhasznaloId = _tenantService.GetCurrentUserId();

        // Következő esedékesség számítása (ha van ellenőrzési időszak)
        if (adat.KovetkezoEsedekesseg == null)
        {
            var sablon = await _context.UzemeltetoSablonok.FindAsync(adat.UzemeltetoSablonId);
            if (sablon?.EllenorzesiIdoszakHonap != null)
            {
                adat.KovetkezoEsedekesseg = adat.RogzitesDatum.AddMonths(sablon.EllenorzesiIdoszakHonap.Value);
            }
        }

        _context.UzemeltetoAdatok.Add(adat);
        await _context.SaveChangesAsync();

        return adat;
    }

    public async Task<UzemeltetoAdat> UpdateAdatAsync(UzemeltetoAdat adat)
    {
        var existing = await _context.UzemeltetoAdatok.FindAsync(adat.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Adat nem található: {adat.Id}");
        }

        // Tenant és üzemeltető ellenőrzés
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Uzemelteto))
        {
            // Üzemeltető csak a saját adatait módosíthatja
            var userId = _tenantService.GetCurrentUserId();
            if (existing.RogzitoFelhasznaloId != userId)
            {
                throw new UnauthorizedAccessException("Csak a saját adatait módosíthatja!");
            }
        }
        else if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez az adathoz!");
            }
        }

        existing.RogzitesDatum = adat.RogzitesDatum;
        existing.KovetkezoEsedekesseg = adat.KovetkezoEsedekesseg;
        existing.Statusz = adat.Statusz;
        existing.MezoErtekekJson = adat.MezoErtekekJson;
        existing.Megjegyzes = adat.Megjegyzes;
        existing.Aktiv = adat.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAdatAsync(int id)
    {
        var adat = await _context.UzemeltetoAdatok.FindAsync(id);
        if (adat == null)
        {
            throw new InvalidOperationException($"Adat nem található: {id}");
        }

        // Tenant és üzemeltető ellenőrzés
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Uzemelteto))
        {
            // Üzemeltető csak a saját adatait törölheti
            var userId = _tenantService.GetCurrentUserId();
            if (adat.RogzitoFelhasznaloId != userId)
            {
                throw new UnauthorizedAccessException("Csak a saját adatait törölheti!");
            }
        }
        else if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(adat.CegId))
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága ehhez az adathoz!");
            }
        }

        adat.Aktiv = false;
        adat.Modositva = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<UzemeltetoAdat>> GetLejaroAdatokAsync(int napok = 30)
    {
        var hatarDatum = DateTime.Today.AddDays(napok);

        var query = _context.UzemeltetoAdatok
            .Include(a => a.UzemeltetoSablon)
            .Include(a => a.Ceg)
            .Include(a => a.RogzitoFelhasznalo)
            .Where(a => a.Aktiv &&
                        a.KovetkezoEsedekesseg != null &&
                        a.KovetkezoEsedekesseg <= hatarDatum)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(a => cegIds.Contains(a.CegId));
        }

        return await query.OrderBy(a => a.KovetkezoEsedekesseg).ToListAsync();
    }
}
