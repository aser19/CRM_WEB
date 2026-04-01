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
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .OrderByDescending(h => h.Datum)
            .ToListAsync();

    public async Task<Hitelesites?> GetByIdAsync(int id) =>
        await _context.Hitelesitesek
            .Include(h => h.EszkozTipus)
            .Include(h => h.Hatosag)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<Hitelesites> CreateAsync(Hitelesites hitelesites)
    {
        hitelesites.Letrehozva = DateTime.UtcNow;
        _context.Hitelesitesek.Add(hitelesites);
        await _context.SaveChangesAsync();
        return hitelesites;
    }

    public async Task<Hitelesites> UpdateAsync(Hitelesites hitelesites)
    {
        var existing = await _context.Hitelesitesek.FindAsync(hitelesites.Id)
            ?? throw new InvalidOperationException("Nem található.");
        
        existing.EszkozTipusId = hitelesites.EszkozTipusId;
        existing.HatosagId = hitelesites.HatosagId;
        existing.Ugyiratszam = hitelesites.Ugyiratszam;
        existing.Darabszam = hitelesites.Darabszam;
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