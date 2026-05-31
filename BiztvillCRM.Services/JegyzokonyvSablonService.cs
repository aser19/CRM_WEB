using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class JegyzokonyvSablonService(CrmDbContext context) : IJegyzokonyvSablonService
{
    public async Task<List<JegyzokonyvSablonTetel>> GetTetelek(int meresTipusId, int oldalSzam, int? cegId = null)
    {
        return await context.JegyzokonyvSablonTetelek
            .Where(t => t.MeresTipusId == meresTipusId
                     && t.OldalSzam == oldalSzam
                     && t.Aktiv
                     && (t.CegId == null || t.CegId == cegId))
            .OrderBy(t => t.CegId == null ? 0 : 1) // admin sablonok előre
            .ThenBy(t => t.Kategoria)
            .ThenBy(t => t.Sorrend)
            .ToListAsync();
    }

    public async Task<List<JegyzokonyvSablonTetel>> GetOsszesTetelek(
        int? meresTipusId = null, int? cegId = null, bool adminSablonokIs = true)
    {
        var query = context.JegyzokonyvSablonTetelek
            .Include(t => t.MeresTipus)
            .Include(t => t.Ceg)
            .AsQueryable();

        if (meresTipusId.HasValue)
            query = query.Where(t => t.MeresTipusId == meresTipusId.Value);

        if (adminSablonokIs && cegId.HasValue)
            // Admin sablonok + cég saját sablonjai
            query = query.Where(t => t.CegId == null || t.CegId == cegId.Value);
        else if (!adminSablonokIs && cegId.HasValue)
            // Csak cég saját sablonjai
            query = query.Where(t => t.CegId == cegId.Value);
        else if (adminSablonokIs && !cegId.HasValue)
            // Csak admin sablonok (Admin felület)
            query = query.Where(t => t.CegId == null);
        // ha mindkettő null: összes rekord (szuperadmin nézet)

        return await query
            .OrderBy(t => t.MeresTipusId)
            .ThenBy(t => t.CegId == null ? 0 : 1)
            .ThenBy(t => t.OldalSzam)
            .ThenBy(t => t.Kategoria)
            .ThenBy(t => t.Sorrend)
            .ToListAsync();
    }

    public async Task<JegyzokonyvSablonTetel?> GetByIdAsync(int id)
        => await context.JegyzokonyvSablonTetelek
            .Include(t => t.MeresTipus)
            .Include(t => t.Ceg)
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

    public async Task<List<JegyzokonyvSablonTetel>> KlonozasCegnek(int meresTipusId, int celCegId)
    {
        // Ellenőrzés: van-e már cég-specifikus sablon ehhez a típushoz?
        var letezik = await context.JegyzokonyvSablonTetelek
            .AnyAsync(t => t.MeresTipusId == meresTipusId && t.CegId == celCegId);
        if (letezik)
            throw new InvalidOperationException("Ehhez a mérés típushoz már létezik saját sablon.");

        var adminTetelek = await context.JegyzokonyvSablonTetelek
            .Where(t => t.MeresTipusId == meresTipusId && t.CegId == null)
            .ToListAsync();

        var ujTetelek = adminTetelek.Select(t => new JegyzokonyvSablonTetel
        {
            CegId = celCegId,
            MeresTipusId = t.MeresTipusId,
            OldalSzam = t.OldalSzam,
            Kategoria = t.Kategoria,
            Sorrend = t.Sorrend,
            Felirat = t.Felirat,
            LehetsegesErtekek = t.LehetsegesErtekek,
            AlapertelmezettErtek = t.AlapertelmezettErtek,
            VanMegjegyzesMezo = t.VanMegjegyzesMezo,
            Aktiv = true
        }).ToList();

        context.JegyzokonyvSablonTetelek.AddRange(ujTetelek);
        await context.SaveChangesAsync();
        return ujTetelek;
    }

    public async Task<JegyzokonyvSablonTetel> EgyTetelKlonozasa(int tetelId, int celCegId)
    {
        var forras = await context.JegyzokonyvSablonTetelek.FindAsync(tetelId)
            ?? throw new InvalidOperationException("A forrás tétel nem található.");

        var ujTetel = new JegyzokonyvSablonTetel
        {
            CegId = celCegId,
            MeresTipusId = forras.MeresTipusId,
            OldalSzam = forras.OldalSzam,
            Kategoria = forras.Kategoria,
            Sorrend = forras.Sorrend,
            Felirat = forras.Felirat,
            LehetsegesErtekek = forras.LehetsegesErtekek,
            AlapertelmezettErtek = forras.AlapertelmezettErtek,
            VanMegjegyzesMezo = forras.VanMegjegyzesMezo,
            Aktiv = true
        };

        context.JegyzokonyvSablonTetelek.Add(ujTetel);
        await context.SaveChangesAsync();
        return ujTetel;
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
                Aktiv = true,
                CegId = null // import mindig admin sablon
            };
            context.JegyzokonyvSablonTetelek.Add(tetel);
            eredmeny.Add(tetel);
        }

        await context.SaveChangesAsync();
        return eredmeny;
    }

    /// <summary>
    /// Sablon módban kitöltött értékek (tetelId → érték dictionary)
    /// visszamentése az AlapertelmezettErtek mezőkbe.
    /// </summary>
    public async Task SablonAlapertelmezettekFrissiteseAsync(
        Dictionary<int, string> tetelIdErtekMap, int? cegId)
    {
        var ids = tetelIdErtekMap.Keys.ToList();
        var tetelek = await context.JegyzokonyvSablonTetelek
            .Where(t => ids.Contains(t.Id) && t.CegId == cegId)
            .ToListAsync();

        foreach (var tetel in tetelek)
        {
            if (tetelIdErtekMap.TryGetValue(tetel.Id, out var ujErtek)
                && tetel.ErtekLista.Contains(ujErtek))
            {
                tetel.AlapertelmezettErtek = ujErtek;
                tetel.Modositva = DateTime.UtcNow;
            }
        }
        await context.SaveChangesAsync();
    }
}