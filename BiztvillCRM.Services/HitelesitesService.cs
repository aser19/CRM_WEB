using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HitelesitesService : IHitelesitesService
{
    private readonly CrmDbContext _context;

    public HitelesitesService(CrmDbContext context) => _context = context;

    public async Task<List<Hitelesites>> GetAllAsync() =>
        await _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .OrderByDescending(h => h.Datum)
            .ToListAsync();

    public async Task<Hitelesites?> GetByIdAsync(int id) =>
        await _context.Hitelesitesek
            .Include(h => h.Ugyfel)
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .FirstOrDefaultAsync(h => h.Id == id);

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
        existing.Modositva = DateTime.UtcNow;
        
        // Lejárat dátum automatikus számítása az eszköztípus alapján
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
    /// </summary>
    private async Task SzamolLejaratDatumAsync(Hitelesites hitelesites)
    {
        var eszkozTipus = await _context.EszkozTipusok.FindAsync(hitelesites.EszkozTipusId);
        
        if (eszkozTipus is not null && eszkozTipus.HitelesitesiIdotartamHonap > 0)
        {
            hitelesites.LejaratDatum = hitelesites.Datum.AddMonths(eszkozTipus.HitelesitesiIdotartamHonap);
        }
    }
}