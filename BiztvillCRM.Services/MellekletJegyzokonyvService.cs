using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MellekletJegyzokonyvService(CrmDbContext context) : IMellekletJegyzokonyvService
{
    public async Task<List<MellekletJegyzokonyv>> GetByMeresIdAsync(int meresId)
        => await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == meresId)
            .OrderBy(m => m.Letrehozva)
            .ToListAsync();

    public async Task<MellekletJegyzokonyv?> GetByIdAsync(int id)
        => await context.MellekletJegyzokonyvek.FindAsync(id);

    public async Task<MellekletJegyzokonyv> LetrehozVagyFrissitAsync(int meresId, string tipus, string szam)
    {
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
        var m = await context.MellekletJegyzokonyvek.FindAsync(id);
        if (m is null) return;

        m.AdatokJson = adatokJson;
        m.Statusz = kesz ? MellekletStatusz.Kesz : MellekletStatusz.Folyamatban;
        m.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task<bool> MindenKeszeE(int meresId)
    {
        var tetelek = await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == meresId)
            .ToListAsync();
        return tetelek.Count > 0 && tetelek.All(m => m.KeszeE);
    }

    public async Task<int> MellekletMeresLetrehozAsync(
        int mellekletId, int meresTipusId, JegyzokonyvAdatok foAdatok)
    {
        var melleklet = await context.MellekletJegyzokonyvek
            .Include(m => m.Meres)
            .FirstOrDefaultAsync(m => m.Id == mellekletId)
            ?? throw new Exception("Melléklet nem található.");

        // Ha már van melléklet meres, visszaadjuk
        if (melleklet.MellekletMeresId.HasValue)
            return melleklet.MellekletMeresId.Value;

        var foMeres = melleklet.Meres
            ?? throw new Exception("Főjegyőkönyv mérése nem található.");

        var elotoltottAdatok = new JegyzokonyvAdatok
        {
            JegyzokonyvSzam  = melleklet.Szam,
            VizsgalatHelye   = foAdatok.VizsgalatHelye,
            FelulvizsgaloNev = foAdatok.FelulvizsgaloNev,
            Megrendelo       = foAdatok.Megrendelo,
            CegNev           = foAdatok.CegNev,
            CegCim           = foAdatok.CegCim,
            CegWeb           = foAdatok.CegWeb,
            CegTelefon       = foAdatok.CegTelefon,
        };

        var ujMeres = new Meres
        {
            UgyfelId     = foMeres.UgyfelId,
            TelephelyId  = foMeres.TelephelyId,
            MeresTipusId = meresTipusId,
            Datum        = DateTime.Today,
            MeresStatusz = MeresStatusz.Folyamatban,
            Megjegyzes   = $"Melléklet: {melleklet.Szam}",
            JegyzokonyvAdatokJson = System.Text.Json.JsonSerializer.Serialize(elotoltottAdatok)
        };

        context.Meresek.Add(ujMeres);
        await context.SaveChangesAsync();

        melleklet.MellekletMeresId = ujMeres.Id;
        melleklet.Modositva = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return ujMeres.Id;
    }
}