using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class JegyzokonyvSablonService(CrmDbContext context) : IJegyzokonyvSablonService
{
    public async Task<List<JegyzokonyvSablonTetel>> GetTetelek(int meresTipusId, int oldalSzam)
    {
        return await context.JegyzokonyvSablonTetelek
            .Where(t => t.MeresTipusId == meresTipusId && t.OldalSzam == oldalSzam && t.Aktiv)
            .OrderBy(t => t.Kategoria)
            .ThenBy(t => t.Sorrend)
            .ToListAsync();
    }

    public async Task<List<JegyzokonyvSablonTetel>> GetOsszesTetelek(int? meresTipusId = null)
    {
        var query = context.JegyzokonyvSablonTetelek
            .Include(t => t.MeresTipus)
            .AsQueryable();

        if (meresTipusId.HasValue)
            query = query.Where(t => t.MeresTipusId == meresTipusId.Value);

        return await query
            .OrderBy(t => t.MeresTipusId)
            .ThenBy(t => t.OldalSzam)
            .ThenBy(t => t.Kategoria)
            .ThenBy(t => t.Sorrend)
            .ToListAsync();
    }

    public async Task<JegyzokonyvSablonTetel?> GetByIdAsync(int id)
        => await context.JegyzokonyvSablonTetelek
            .Include(t => t.MeresTipus)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<JegyzokonyvSablonTetel> CreateAsync(JegyzokonyvSablonTetel tetel)
    {
        tetel.Letrehozva = DateTime.UtcNow;
        context.JegyzokonyvSablonTetelek.Add(tetel);
        await context.SaveChangesAsync();
        return tetel;
    }

    public async Task<JegyzokonyvSablonTetel> UpdateAsync(JegyzokonyvSablonTetel tetel)
    {
        tetel.Modositva = DateTime.UtcNow;
        context.JegyzokonyvSablonTetelek.Update(tetel);
        await context.SaveChangesAsync();
        return tetel;
    }

    public async Task DeleteAsync(int id)
    {
        var tetel = await context.JegyzokonyvSablonTetelek.FindAsync(id);
        if (tetel is not null)
        {
            context.JegyzokonyvSablonTetelek.Remove(tetel);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<JegyzokonyvSablonTetel>> ImportAlakAsync(
        int meresTipusId, int oldalSzam, string kategoria,
        List<string> feliratok, string ertekek = "MF;NMF;NA")
    {
        var eredmeny = new List<JegyzokonyvSablonTetel>();
        var sorrend = 1;

        foreach (var felirat in feliratok)
        {
            var tetel = new JegyzokonyvSablonTetel
            {
                MeresTipusId = meresTipusId,
                OldalSzam = oldalSzam,
                Kategoria = kategoria,
                Felirat = felirat,
                LehetsegesErtekek = ertekek,
                AlapertelmezettErtek = ertekek.Split(';').FirstOrDefault() ?? "MF",
                Sorrend = sorrend++,
                Aktiv = true
            };
            context.JegyzokonyvSablonTetelek.Add(tetel);
            eredmeny.Add(tetel);
        }

        await context.SaveChangesAsync();
        return eredmeny;
    }
}