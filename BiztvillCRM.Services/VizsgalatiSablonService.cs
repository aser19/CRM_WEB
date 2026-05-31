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

    public async Task<List<VizsgalatiSablon>> GetAllAsync(int meresTipusId, int? cegId = null)
    {
        return await context.VizsgalatiSablonok
            .Where(s => s.MeresTipusId == meresTipusId
                     && s.Aktiv
                     && (s.CegId == null || s.CegId == cegId))
            .OrderBy(s => s.CegId == null ? 0 : 1)
            .ThenBy(s => s.Nev)
            .ToListAsync();
    }

    public async Task<VizsgalatiSablon?> GetByIdAsync(int id)
        => await context.VizsgalatiSablonok.FindAsync(id);

    public async Task<VizsgalatiSablon> MentesAsync(VizsgalatiSablon sablon)
    {
        if (sablon.Id == 0)
        {
            sablon.Letrehozva = DateTime.UtcNow;
            context.VizsgalatiSablonok.Add(sablon);
        }
        else
        {
            // Tracked példány lekérése, majd property-k frissítése
            var meglevo = await context.VizsgalatiSablonok.FindAsync(sablon.Id);
            if (meglevo is null)
                throw new InvalidOperationException($"Sablon #{sablon.Id} nem található.");

            meglevo.Nev = sablon.Nev;
            meglevo.Leiras = sablon.Leiras;
            meglevo.AdatokJson = sablon.AdatokJson;
            meglevo.Aktiv = sablon.Aktiv;
            meglevo.CegId = sablon.CegId;
            meglevo.MeresTipusId = sablon.MeresTipusId;
            meglevo.Modositva = DateTime.UtcNow;
        }
        await context.SaveChangesAsync();
        return sablon;
    }

    public async Task TorlesAsync(int id)
    {
        var sablon = await context.VizsgalatiSablonok.FindAsync(id);
        if (sablon is not null)
        {
            context.VizsgalatiSablonok.Remove(sablon);
            await context.SaveChangesAsync();
        }
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