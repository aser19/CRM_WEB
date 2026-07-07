using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class UzemeltetoSablonFelhasznaloService : IUzemeltetoSablonFelhasznaloService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public UzemeltetoSablonFelhasznaloService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<UzemeltetoSablonFelhasznalo>> GetBySablonIdAsync(int sablonId)
    {
        var cegId = _tenantService.GetCurrentCegId();

        return await _context.UzemeltetoSablonFelhasznalok
            .Include(usf => usf.Felhasznalo)
            .Include(usf => usf.UzemeltetoSablon)
            .Include(usf => usf.HozzarendeloFelhasznalo)
            .Where(usf => usf.UzemeltetoSablonId == sablonId 
                       && usf.UzemeltetoSablon.CegId == cegId
                       && usf.Aktiv)
            .OrderBy(usf => usf.Felhasznalo.Nev)
            .ToListAsync();
    }

    public async Task<List<UzemeltetoSablonFelhasznalo>> GetByFelhasznaloIdAsync(string felhasznaloId)
    {
        var cegId = _tenantService.GetCurrentCegId();

        return await _context.UzemeltetoSablonFelhasznalok
            .Include(usf => usf.UzemeltetoSablon)
                .ThenInclude(s => s.Mezok)
            .Include(usf => usf.HozzarendeloFelhasznalo)
            .Where(usf => usf.FelhasznaloId == felhasznaloId 
                       && usf.UzemeltetoSablon.CegId == cegId
                       && usf.Aktiv
                       && usf.UzemeltetoSablon.Aktiv)
            .OrderBy(usf => usf.UzemeltetoSablon.Nev)
            .ToListAsync();
    }

    public async Task<UzemeltetoSablonFelhasznalo> HozzarendelAsync(int sablonId, string felhasznaloId)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var currentUserId = _tenantService.GetCurrentUserId();

        // Ellenőrizzük, hogy a sablon a tenant cégjéhez tartozik-e
        var sablon = await _context.UzemeltetoSablonok
            .FirstOrDefaultAsync(s => s.Id == sablonId && s.CegId == cegId);

        if (sablon == null)
            throw new UnauthorizedAccessException("A sablon nem található vagy nincs hozzáférésed.");

        // Ellenőrizzük, hogy már létezik-e hozzárendelés
        var meglevo = await _context.UzemeltetoSablonFelhasznalok
            .FirstOrDefaultAsync(usf => usf.UzemeltetoSablonId == sablonId 
                                     && usf.FelhasznaloId == felhasznaloId);

        if (meglevo != null)
        {
            // Ha már létezik, de inaktív, akkor aktiváljuk
            if (!meglevo.Aktiv)
            {
                meglevo.Aktiv = true;
                await _context.SaveChangesAsync();
            }
            return meglevo;
        }

        // Új hozzárendelés létrehozása
        var hozzarendeles = new UzemeltetoSablonFelhasznalo
        {
            UzemeltetoSablonId = sablonId,
            FelhasznaloId = felhasznaloId,
            HozzarendeloFelhasznaloId = currentUserId,
            Letrehozva = DateTime.UtcNow,
            Aktiv = true
        };

        _context.UzemeltetoSablonFelhasznalok.Add(hozzarendeles);
        await _context.SaveChangesAsync();

        // Frissítjük a navigációs property-ket
        await _context.Entry(hozzarendeles)
            .Reference(h => h.Felhasznalo)
            .LoadAsync();
        await _context.Entry(hozzarendeles)
            .Reference(h => h.UzemeltetoSablon)
            .LoadAsync();
        await _context.Entry(hozzarendeles)
            .Reference(h => h.HozzarendeloFelhasznalo)
            .LoadAsync();

        return hozzarendeles;
    }

    public async Task TorlesAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();

        var hozzarendeles = await _context.UzemeltetoSablonFelhasznalok
            .Include(usf => usf.UzemeltetoSablon)
            .FirstOrDefaultAsync(usf => usf.Id == id && usf.UzemeltetoSablon.CegId == cegId);

        if (hozzarendeles == null)
            throw new UnauthorizedAccessException("A hozzárendelés nem található vagy nincs hozzáférésed.");

        _context.UzemeltetoSablonFelhasznalok.Remove(hozzarendeles);
        await _context.SaveChangesAsync();
    }

    public async Task SetAktivAsync(int id, bool aktiv)
    {
        var cegId = _tenantService.GetCurrentCegId();

        var hozzarendeles = await _context.UzemeltetoSablonFelhasznalok
            .Include(usf => usf.UzemeltetoSablon)
            .FirstOrDefaultAsync(usf => usf.Id == id && usf.UzemeltetoSablon.CegId == cegId);

        if (hozzarendeles == null)
            throw new UnauthorizedAccessException("A hozzárendelés nem található vagy nincs hozzáférésed.");

        hozzarendeles.Aktiv = aktiv;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsHozzarendelveAsync(int sablonId, string felhasznaloId)
    {
        var cegId = _tenantService.GetCurrentCegId();

        return await _context.UzemeltetoSablonFelhasznalok
            .AnyAsync(usf => usf.UzemeltetoSablonId == sablonId 
                          && usf.FelhasznaloId == felhasznaloId
                          && usf.UzemeltetoSablon.CegId == cegId
                          && usf.Aktiv);
    }

    public async Task<List<UzemeltetoSablonFelhasznalo>> GetAllAktivAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();

        return await _context.UzemeltetoSablonFelhasznalok
            .Include(usf => usf.Felhasznalo)
            .Include(usf => usf.UzemeltetoSablon)
            .Where(usf => usf.UzemeltetoSablon.CegId == cegId 
                       && usf.Aktiv 
                       && usf.UzemeltetoSablon.Aktiv)
            .OrderBy(usf => usf.UzemeltetoSablon.Nev)
                .ThenBy(usf => usf.Felhasznalo.Nev)
            .ToListAsync();
    }
}
