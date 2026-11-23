using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class NyilvanosLekerdezesService : INyilvanosLekerdezesService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public NyilvanosLekerdezesService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UgyfelLekerdezesViewModel?> LekerdezesByTokenAsync(string token, bool tartalmazzaInaktivakat = false)
    {
        await using var ctx = await _contextFactory.CreateDbContextAsync();

        var tokenRek = await ctx.UgyfelLekerdezesiTokenek
            .Include(t => t.Ugyfel)
            .FirstOrDefaultAsync(t => t.Token == token && t.Aktiv);

        if (tokenRek == null || !tokenRek.ErvenyesE)
            return null;

        tokenRek.UtolsoHasznalat = DateTime.UtcNow;
        await ctx.SaveChangesAsync();

        // Ha az ügyfél egy ügyfélcsoport tagja (pl. ugyanaz a tulajdonos több néven/cégen kér
        // jegyzőkönyvet), akkor a csoport összes tagjának adatait összesítve adjuk vissza.
        var csoportId = tokenRek.Ugyfel?.UgyfelCsoportId;
        var csoportTagok = csoportId.HasValue
            ? await ctx.Ugyfelek.Where(u => u.UgyfelCsoportId == csoportId.Value).ToListAsync()
            : new List<Ugyfel> { tokenRek.Ugyfel! };

        var ugyfelIds = csoportTagok.Select(u => u.Id).ToList();
        var ugyfelNevek = csoportTagok.ToDictionary(u => u.Id, u => u.Nev);

        var telephelyek = await ctx.Telephelyek
            .Where(t => ugyfelIds.Contains(t.UgyfelId))
            .OrderBy(t => t.Nev)
            .ToListAsync();

        // A mellékletekhez tartozó "belső" mérés rekordokat (pl. HVM, AVK almérések) ki kell zárni,
        // mert ezek nem önálló mérések, hanem a fő méréshez tartozó melléklet-jegyzőkönyvek adatai.
        var mellekletMeresIds = await ctx.MellekletJegyzokonyvek
            .Where(mj => mj.MellekletMeresId.HasValue)
            .Select(mj => mj.MellekletMeresId!.Value)
            .ToListAsync();

        var meresekQuery = ctx.Meresek
            .Include(m => m.MeresTipus)
            .Where(m => ugyfelIds.Contains(m.UgyfelId) && !mellekletMeresIds.Contains(m.Id));

        if (!tartalmazzaInaktivakat)
        {
            meresekQuery = meresekQuery.Where(m => m.Aktiv);
        }

        var meresek = await meresekQuery
            .OrderByDescending(m => m.Datum)
            .ToListAsync();

        // Hitelesítések lekérdezése: aktív vagy összes az élettörténet kapcsoló alapján
        var hitelesitesekQuery = ctx.Hitelesitesek
            .Include(h => h.EszkozTipus)
            .Where(h => h.UgyfelId.HasValue && ugyfelIds.Contains(h.UgyfelId.Value));

        if (!tartalmazzaInaktivakat)
        {
            hitelesitesekQuery = hitelesitesekQuery.Where(h => h.Aktiv);
        }

        var hitelesitesek = await hitelesitesekQuery
            .OrderByDescending(h => h.Datum)
            .ToListAsync();

        // Hitelesítési csoportok betöltése az eszköztípus-azonosítókhoz
        var eszkozTipusIdk = hitelesitesek.Select(h => h.EszkozTipusId).Distinct().ToList();
        var csoportok = await ctx.HitelesitesCsoportok
            .Include(c => c.Tagok)
                .ThenInclude(t => t.EszkozTipus)
            .Where(c => c.Aktiv && (
                eszkozTipusIdk.Contains(c.FoEszkozTipusId ?? 0) ||
                c.Tagok.Any(t => eszkozTipusIdk.Contains(t.EszkozTipusId))))
            .ToListAsync();

        var karbantartasok = await ctx.Karbantartasok
            .Include(k => k.KarbantartasTipus)
            .Where(k => ugyfelIds.Contains(k.UgyfelId))
            .OrderBy(k => k.KovetkezoDatum)
            .ToListAsync();

        var zonaterkepek = await ctx.Zonaterkepek
            .Where(z => ugyfelIds.Contains(z.UgyfelId) && z.Aktiv)
            .OrderByDescending(z => z.ErvenyessegVege)
            .ToListAsync();

        var kockazatertekelesek = await ctx.Kockazatertekelesek
            .Where(k => ugyfelIds.Contains(k.UgyfelId) && k.Aktiv)
            .OrderByDescending(k => k.ErtekelesDatuma)
            .ToListAsync();

        var result = new UgyfelLekerdezesViewModel
        {
            UgyfelNev = tokenRek.Ugyfel?.Nev ?? "",
            Telephelyek = telephelyek.Select(tp => new TelephelyAdatok
            {
                UgyfelNev = ugyfelNevek.TryGetValue(tp.UgyfelId, out var un) ? un : (tokenRek.Ugyfel?.Nev ?? ""),
                Nev = tp.Nev ?? "",
                Cim = tp.Cim ?? "",
                Meresek = meresek
                    .Where(m => m.TelephelyId == tp.Id)
                    .Select(m => new MeresOsszefoglalo
                    {
                        MeresId = m.Id,
                        Datum = m.Datum,
                        Tipus = m.MeresTipus?.Nev ?? "",
                        Eredmeny = m.Eredmeny,
                        KovetkezoDatum = m.KovetkezoDatum,
                        Statusz = m.MeresStatusz.ToString(),
                        MellekletTipusKod = m.MeresTipus?.MellekletTipusKod,
                        SablonId = m.MeresTipus?.SablonId,
                        Megjegyzes = m.Megjegyzes,
                        VanJegyzokonyv = !string.IsNullOrWhiteSpace(m.JegyzokonyvAdatokJson),
                        KulsoJegyzokonyvPaths = m.KulsoJegyzokonyvPathsLista
                    }).ToList(),
                Hitelesitesek = hitelesitesek
                    .Where(h => h.TelephelyId == tp.Id)
                    .Select(h =>
                    {
                        // Csoport keresése az eszköztípushoz
                        var csoport = csoportok.FirstOrDefault(c =>
                            c.FoEszkozTipusId == h.EszkozTipusId ||
                            c.Tagok.Any(t => t.EszkozTipusId == h.EszkozTipusId));

                        // Mentett egyedi dátumok
                        var mentettDatumok = h.CsoportTagLejaratokLista;

                        var kozbensoVizsgalatok = csoport?.Tagok
                            .OrderBy(t => t.Sorrend)
                            .Select(tag =>
                            {
                                var mentett = mentettDatumok.FirstOrDefault(m => m.EszkozTipusId == tag.EszkozTipusId);
                                var autoLejarat = tag.EszkozTipus != null
                                    ? h.Datum.AddMonths(tag.EszkozTipus.HitelesitesiIdotartamHonap)
                                    : (DateTime?)null;
                                return new CsoportTagLejaratReszlet
                                {
                                    EszkozTipusId = tag.EszkozTipusId,
                                    EszkozTipusNev = tag.EszkozTipus?.Nev ?? "",
                                    LejaratDatum = mentett?.LejaratDatum ?? autoLejarat,
                                    Megjegyzes = tag.Megjegyzes,
                                    NemAlkalmazhato = mentett?.NemAlkalmazhato ?? false,
                                    MunkalapPath = mentett?.MunkalapPath,
                                    BizonyitvanyPath = mentett?.BizonyitvanyPath,
                                    MunkalapPaths = mentett?.MunkalapPaths,
                                    BizonyitvanyPaths = mentett?.BizonyitvanyPaths
                                };
                            }).ToList() ?? new();

                        return new HitelesitesOsszefoglalo
                        {
                            EszkozTipusNev = h.EszkozTipus?.Nev ?? "",
                            EszkozAzonosito = h.EszkozAzonosito,
                            Darabszam = h.Darabszam,
                            Datum = h.Datum,
                            LejaratDatum = h.LejaratDatum,
                            Aktiv = h.Aktiv,
                            MunkalapPath = h.MunkalapPath,
                            BizonyitvanyPath = h.BizonyitvanyPath,
                            KozbensoVizsgalatok = kozbensoVizsgalatok,
                            EgyediLejaratok = h.EgyediLejaratokLista
                        };
                    }).ToList(),
                Karbantartasok = karbantartasok
                    .Where(k => k.TelephelyId == tp.Id)
                    .Select(k => new KarbantartasOsszefoglalo
                    {
                        TipusNev = k.KarbantartasTipus?.Nev ?? "",
                        KovetkezoDatum = k.KovetkezoDatum,
                        Statusz = k.Statusz.ToString()
                    }).ToList(),
                Zonaterkepek = zonaterkepek
                    .Where(z => z.TelephelyId == tp.Id || z.TelephelyId == null)
                    .Select(z => new ZonaterkepOsszefoglalo
                    {
                        Megnevezes = z.Megnevezes,
                        ZonaTipus = z.ZonaTipus.ToString(),
                        ErvenyessegVege = z.ErvenyessegVege,
                        Aktiv = z.Aktiv
                    }).ToList(),
                Kockazatertekelesek = kockazatertekelesek
                    .Where(k => k.TelephelyId == tp.Id || k.TelephelyId == null)
                    .Select(k => new KockazatertekelesOsszefoglalo
                    {
                        Megnevezes = k.Megnevezes,
                        ErtekelesDatuma = k.ErtekelesDatuma,
                        KovetkezoFelulvizsgalat = k.KovetkezoFelulvizsgalat,
                        KockazatiSzint = k.KockazatiSzint.ToString(),
                        Statusz = k.Statusz.ToString()
                    }).ToList(),
            }).ToList()
        };

        return result;
    }

    public async Task<string> UjTokenGeneralasAsync(int ugyfelId)
    {
        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var token = Guid.NewGuid().ToString("N");
        ctx.UgyfelLekerdezesiTokenek.Add(new UgyfelLekerdezesiToken
        {
            UgyfelId = ugyfelId,
            Token = token,
            Letrehozva = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();
        return token;
    }

    public async Task TokenDeaktivalasAsync(int tokenId)
    {
        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var t = await ctx.UgyfelLekerdezesiTokenek.FindAsync(tokenId);
        if (t != null) { t.Aktiv = false; await ctx.SaveChangesAsync(); }
    }

    public async Task<List<UgyfelLekerdezesiToken>> GetTokenekByUgyfelAsync(int ugyfelId)
    {
        await using var ctx = await _contextFactory.CreateDbContextAsync();
        return await ctx.UgyfelLekerdezesiTokenek
            .Where(t => t.UgyfelId == ugyfelId && t.Aktiv)
            .OrderByDescending(t => t.Letrehozva)
            .ToListAsync();
    }
}