using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class CegService : ICegService
{
    private readonly CrmDbContext _context;

    public CegService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ceg>> GetAllAsync(bool csakAktiv = true)
    {
        var query = _context.Cegek.AsQueryable();
        
        if (csakAktiv)
            query = query.Where(c => c.Aktiv);
            
        return await query.OrderBy(c => c.Nev).ToListAsync();
    }

    public async Task<Ceg?> GetByIdAsync(int id)
    {
        return await _context.Cegek.FindAsync(id);
    }

    public async Task<Ceg> CreateAsync(Ceg ceg)
    {
        ceg.Letrehozva = DateTime.Now;
        ceg.Aktiv = true;
        
        _context.Cegek.Add(ceg);
        await _context.SaveChangesAsync();
        
        return ceg;
    }

    public async Task<Ceg> UpdateAsync(Ceg ceg)
    {
        var existing = await _context.Cegek.FindAsync(ceg.Id);
        if (existing == null)
            throw new InvalidOperationException("Cég nem található.");

        // Ellenőrizzük, hogy van-e downgrade a tevékenységekben
        var regiTevekenyseg = existing.Tevekenyseg;
        var ujTevekenyseg = ceg.Tevekenyseg;
        
        // Downgrade: azok a címkék, amik korábban megvoltak, de most már nincsenek
        var eltavolitottCimkek = regiTevekenyseg & ~ujTevekenyseg;
        
        // Ha van eltávolított címke, az ügyfelektől is el kell távolítani
        if (eltavolitottCimkek != TevekenysegTipus.Nincs)
        {
            await UgyfelekTevekenysegDowngradeAsync(ceg.Id, eltavolitottCimkek);
        }

        // Cég adatainak frissítése
        existing.Nev = ceg.Nev;
        existing.Adoszam = ceg.Adoszam;
        existing.Cim = ceg.Cim;
        existing.Email = ceg.Email;
        existing.Telefon = ceg.Telefon;
        existing.Weboldal = ceg.Weboldal;
        existing.Tevekenyseg = ceg.Tevekenyseg;  // <-- Tevekenyseg mentése
        existing.Aktiv = ceg.Aktiv;
        existing.Modositva = DateTime.Now;

        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Eltávolítja az ügyfelektől azokat a címkéket, amiket a cég is elvesztett (downgrade).
    /// </summary>
    private async Task UgyfelekTevekenysegDowngradeAsync(int cegId, TevekenysegTipus eltavolitandoCimkek)
    {
        // Lekérjük a céghez tartozó ügyfeleket, akiknek van érintett címkéjük
        var ugyfelek = await _context.Ugyfelek
            .Where(u => u.CegId == cegId)
            .ToListAsync();

        foreach (var ugyfel in ugyfelek)
        {
            // Csak akkor módosítjuk, ha az ügyfélnek van olyan címkéje, amit el kell távolítani
            if ((ugyfel.Tevekenyseg & eltavolitandoCimkek) != TevekenysegTipus.Nincs)
            {
                // Eltávolítjuk az érintett címkéket
                ugyfel.Tevekenyseg &= ~eltavolitandoCimkek;
                ugyfel.Modositva = DateTime.Now;
            }
        }

        // A SaveChangesAsync a hívó metódusban történik
    }

    public async Task<bool> SetAktivAsync(int id, bool aktiv)
    {
        var ceg = await _context.Cegek.FindAsync(id);
        if (ceg == null)
            return false;

        ceg.Aktiv = aktiv;
        ceg.Modositva = DateTime.Now;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<int> GetFelhasznalokSzamaAsync(int cegId)
    {
        return await _context.Users.CountAsync(f => f.CegId == cegId);
    }

    public async Task<int> GetUgyfelekSzamaAsync(int cegId)
    {
        return await _context.Ugyfelek.CountAsync(u => u.CegId == cegId);
    }
}