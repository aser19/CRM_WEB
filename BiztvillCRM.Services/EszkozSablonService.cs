using BiztvillCRM.Data;
using BiztvillCRM.Shared.Models;
using BiztvillCRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozSablonService : IEszkozSablonService
{
    private readonly CrmDbContext _context;

    public EszkozSablonService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<EszkozSablon>> GetAllAsync()
    {
        return await _context.EszkozSablonok
            .Include(e => e.Alkatreszek.OrderBy(a => a.Sorrend))
            .ToListAsync();
    }

    public async Task<EszkozSablon?> GetByIdAsync(int id)
    {
        return await _context.EszkozSablonok
            .Include(e => e.Alkatreszek.OrderBy(a => a.Sorrend))
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<EszkozSablon?> GetByEszkozTipusNevAsync(string eszkozTipusNev)
    {
        return await _context.EszkozSablonok
            .Include(e => e.Alkatreszek.OrderBy(a => a.Sorrend))
            .FirstOrDefaultAsync(e => e.EszkozTipusNev == eszkozTipusNev);
    }

    public async Task<int> CreateAsync(EszkozSablon sablon)
    {
        _context.EszkozSablonok.Add(sablon);
        await _context.SaveChangesAsync();
        return sablon.Id;
    }

    public async Task UpdateAsync(EszkozSablon sablon)
    {
        sablon.UtolsoModositas = DateTime.Now;
        _context.EszkozSablonok.Update(sablon);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sablon = await _context.EszkozSablonok.FindAsync(id);
        if (sablon != null)
        {
            _context.EszkozSablonok.Remove(sablon);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<HordozhatoEszkozSor>> GeneralAlkatreszekAsync(
        string eszkozTipusNev,
        int parentSorsz,
        string parentCsoportNev)
    {
        var sablon = await GetByEszkozTipusNevAsync(eszkozTipusNev);
        if (sablon == null || !sablon.VanAlkatresz)
            return new List<HordozhatoEszkozSor>();

        var alkatreszek = new List<HordozhatoEszkozSor>();
        int sorrend = 1;

        foreach (var alkatreszSablon in sablon.Alkatreszek.OrderBy(a => a.Sorrend))
        {
            for (int i = 0; i < alkatreszSablon.DefaultDarabszam; i++)
            {
                alkatreszek.Add(new HordozhatoEszkozSor
                {
                    Sorsz = 0, // Később frissítjük
                    ParentEszkozId = parentSorsz,
                    CsoportNev = parentCsoportNev,
                    CsoportSorrend = sorrend++,
                    Megnevezes = alkatreszSablon.Nev,
                    VedelmiOsztaly = alkatreszSablon.VedelmiOsztaly,
                    Telj = sablon.AlapTeljesitmeny,
                    Megtekint = "MF"
                });
            }
        }

        return alkatreszek;
    }
}