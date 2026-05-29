using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class VizsgalatiSablonService(CrmDbContext context) : IVizsgalatiSablonService
{
    public async Task<List<VizsgalatiSablon>> GetByMeresTipusIdAsync(int meresTipusId)
    {
        return await context.VizsgalatiSablonok
            .Where(s => s.MeresTipusId == meresTipusId && s.Aktiv)
            .OrderBy(s => s.Nev)
            .ToListAsync();
    }

    public async Task<List<string>> GetKategoriak(int meresTipusId)
    {
        return await context.JegyzokonyvSablonTetelek
            .Where(t => t.MeresTipusId == meresTipusId && t.Aktiv && !string.IsNullOrEmpty(t.Kategoria))
            .Select(t => t.Kategoria)
            .Distinct()
            .OrderBy(k => k)
            .ToListAsync();
    }
}