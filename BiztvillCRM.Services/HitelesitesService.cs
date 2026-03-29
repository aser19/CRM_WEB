using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HitelesitesService : IHitelesitesService
{
    private readonly CrmDbContext _context;

    public HitelesitesService(CrmDbContext context) => _context = context;

    // ✅ AsNoTracking() - gyorsabb, nincs tracking konfliktus
    public async Task<List<Hitelesites>> GetAllAsync() =>
        await _context.Hitelesitesek
            .AsNoTracking()
            .Include(h => h.Eszkoz)
            .Include(h => h.Hatosag)
            .OrderByDescending(h => h.Datum)
            .ToListAsync();

    public async Task<Hitelesites?> GetByIdAsync(int id) =>
        await _context.Hitelesitesek
            .AsNoTracking()
            .Include(h => h.Eszkoz)
            .Include(h => h.Hatosag)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<Hitelesites> CreateAsync(Hitelesites hitelesites)
    {
        hitelesites.Letrehozva = DateTime.UtcNow;
        _context.Hitelesitesek.Add(hitelesites);
        await _context.SaveChangesAsync();
        return hitelesites;
    }

    // ✅ Fetch + Update minta - biztonságos, nincs tracking konfliktus
    public async Task<Hitelesites> UpdateAsync(Hitelesites hitelesites)
    {
        var existing = await _context.Hitelesitesek.FindAsync(hitelesites.Id) 
            ?? throw new InvalidOperationException("Nem található a hitelesítés.");

        // Csak a módosítható mezők frissítése
        existing.EszkozId = hitelesites.EszkozId;
        existing.HatosagId = hitelesites.HatosagId;
        existing.Ugyszam = hitelesites.Ugyszam;
        existing.Datum = hitelesites.Datum;
        existing.LejaratDatum = hitelesites.LejaratDatum;
        existing.HitelesitesStatusz = hitelesites.HitelesitesStatusz;
        existing.Megjegyzes = hitelesites.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

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
}