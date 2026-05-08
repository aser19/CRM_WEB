using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MellekletJegyzokonyvService(IDbContextFactory<CrmDbContext> contextFactory) : IMellekletJegyzokonyvService
{
    // context helyett factory — minden metódus saját context-et hoz létre
    public async Task<List<MellekletJegyzokonyv>> GetByMeresIdAsync(int meresId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == meresId)
            .OrderBy(m => m.Letrehozva)
            .ToListAsync();
    }

    public async Task<MellekletJegyzokonyv?> GetByIdAsync(int id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.MellekletJegyzokonyvek.FindAsync(id);
    }

    public async Task<MellekletJegyzokonyv> LetrehozVagyFrissitAsync(int meresId, string tipus, string szam)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var meglevo = await context.MellekletJegyzokonyvek
            .FirstOrDefaultAsync(m => m.MeresId == meresId && m.Tipus == tipus);

        if (meglevo is not null)
        {
            meglevo.Szam = szam;
            meglevo.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return meglevo;
        }

        var uj = new MellekletJegyzokonyv
        {
            MeresId = meresId,
            Tipus = tipus,
            Szam = szam,
            Statusz = MellekletStatusz.Folyamatban
        };
        context.MellekletJegyzokonyvek.Add(uj);
        await context.SaveChangesAsync();
        return uj;
    }

    public async Task MentAdatokAsync(int id, string adatokJson, bool kesz = false)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var m = await context.MellekletJegyzokonyvek.FindAsync(id);
        if (m is null) return;

        m.AdatokJson = adatokJson;
        m.Statusz = kesz ? MellekletStatusz.Kesz : MellekletStatusz.Folyamatban;
        m.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task<bool> MindenKeszeE(int meresId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var tetelek = await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == meresId)
            .ToListAsync();
        return tetelek.Count > 0 && tetelek.All(m => m.KeszeE);
    }

    public async Task<int> MellekletMeresLetrehozAsync(
        int mellekletId, int meresTipusId, JegyzokonyvAdatok foAdatok)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var melleklet = await context.MellekletJegyzokonyvek
            .Include(m => m.Meres)
            .FirstOrDefaultAsync(m => m.Id == mellekletId)
            ?? throw new Exception("Melléklet nem található.");

        if (melleklet.MellekletMeresId.HasValue)
            return melleklet.MellekletMeresId.Value;

        var foMeres = melleklet.Meres
            ?? throw new Exception("Főjegyőkönyv mérése nem található.");

        // ✅ CSAK alapadatok — NEM a főjgyk wizard adatai
        var ujAdatok = new JegyzokonyvAdatok
        {
            JegyzokonyvSzam = melleklet.Szam,
            AvkSorok = new List<AvkSor>()
        };

        var ujMeres = new Meres
        {
            UgyfelId     = foMeres.UgyfelId,
            TelephelyId  = foMeres.TelephelyId,
            MeresTipusId = meresTipusId,
            Datum        = DateTime.Today,
            MeresStatusz = MeresStatusz.Folyamatban,
            Megjegyzes   = $"Melléklet: {melleklet.Szam}",
            JegyzokonyvAdatokJson = System.Text.Json.JsonSerializer.Serialize(ujAdatok)
        };

        context.Meresek.Add(ujMeres);
        await context.SaveChangesAsync();

        melleklet.MellekletMeresId = ujMeres.Id;
        melleklet.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return ujMeres.Id;
    }

    public async Task MentAvkAdatokAsync(int mellekletId, JegyzokonyvAdatok adatok)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var melleklet = await context.MellekletJegyzokonyvek
            .FirstOrDefaultAsync(m => m.Id == mellekletId)
            ?? throw new Exception("Melléklet nem található.");

        if (!melleklet.MellekletMeresId.HasValue)
            throw new Exception("Melléklet mérés még nincs létrehozva.");

        var meres = await context.Meresek.FindAsync(melleklet.MellekletMeresId.Value)
            ?? throw new Exception("Melléklet mérés nem található.");

        meres.JegyzokonyvAdatokJson = System.Text.Json.JsonSerializer.Serialize(adatok);
        await context.SaveChangesAsync();
    }

    public async Task<HashSet<int>> GetMellekletMeresIdsAsync()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var ids = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync();
        return ids.ToHashSet();
    }

    public async Task<HashSet<int>> GetMeresIdsWithMellekletAsync()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var ids = await context.MellekletJegyzokonyvek
            .Select(m => m.MeresId)
            .Distinct()
            .ToListAsync();
        return ids.ToHashSet();
    }

    public async Task StatuszFrissitesAsync(int mellekletId, string ujStatusz)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var melleklet = await context.MellekletJegyzokonyvek.FindAsync(mellekletId);
        if (melleklet is null) return;
        melleklet.Statusz = ujStatusz;
        melleklet.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
}