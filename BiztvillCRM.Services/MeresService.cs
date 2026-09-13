using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BiztvillCRM.Services;

public class MeresService : IMeresService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;
    private readonly IMunkaszamService _munkaszamService;

    public MeresService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService, IMunkaszamService munkaszamService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
        _munkaszamService = munkaszamService;
    }

    public async Task<List<Meres>> GetAllAsync(bool includeInaktivak = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var mellekletMeresIds = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Include(m => m.Tagek)
            .Where(m => !mellekletMeresIds.Contains(m.Id))
            .AsQueryable();

        if (!includeInaktivak)
            query = query.Where(m => m.Aktiv);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<List<Meres>> GetInaktivakAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var mellekletMeresIds = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Include(m => m.Tagek)
            .Where(m => !mellekletMeresIds.Contains(m.Id) && !m.Aktiv) // Csak az inaktívak
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<Meres?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Include(m => m.Tagek)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Meres?> GetByIdPublicAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Meres> CreateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ugyfel = await context.Ugyfelek.FirstOrDefaultAsync(u => u.Id == meres.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (ugyfel == null || !cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága mérés létrehozásához ennél az ügyfélnél.");
        }

        var telephely = await context.Telephelyek.FirstOrDefaultAsync(t => t.Id == meres.TelephelyId);
        if (telephely?.UgyfelId != meres.UgyfelId)
            throw new InvalidOperationException("A telephely nem tartozik a kiválasztott ügyfélhez.");

        meres.Ugyfel = null!;
        meres.Telephely = null!;
        meres.MeresTipus = null!;
        meres.Letrehozva = DateTime.UtcNow;

        context.Meresek.Add(meres);
        await context.SaveChangesAsync();
        return meres;
    }

    public async Task<Meres> UpdateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == meres.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés módosításához.");
        }

        existing.UgyfelId = meres.UgyfelId;
        existing.TelephelyId = meres.TelephelyId;
        existing.MeresTipusId = meres.MeresTipusId;
        existing.Datum = meres.Datum;
        existing.KovetkezoDatum = meres.KovetkezoDatum;
        existing.Eredmeny = meres.Eredmeny;
        existing.MeresStatusz = meres.MeresStatusz;
        existing.Megjegyzes = meres.Megjegyzes;
        existing.KibocsatoCegId = meres.KibocsatoCegId;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meres is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(meres.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés törléséhez.");
        }

        var mellekletHivatkozas = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId == id)
            .ToListAsync();
        foreach (var m in mellekletHivatkozas)
        {
            m.MellekletMeresId = null;
            m.Modositva = DateTime.UtcNow;
        }

        var mellekletek = await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == id)
            .ToListAsync();
        context.MellekletJegyzokonyvek.RemoveRange(mellekletek);

        context.Meresek.Remove(meres);
        await context.SaveChangesAsync();
    }

    public async Task<JegyzokonyvAdatok?> BetoltJegyzokonyvAdatokAsync(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meres = await context.Meresek.FindAsync(meresId);
        if (meres == null || string.IsNullOrEmpty(meres.JegyzokonyvAdatokJson))
            return null;

        return JsonSerializer.Deserialize<JegyzokonyvAdatok>(meres.JegyzokonyvAdatokJson);
    }

    public async Task MentesJegyzokonyvAdatokkalAsync(int meresId, JegyzokonyvAdatok adatok)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.JegyzokonyvAdatokJson = JsonSerializer.Serialize(adatok);
            await context.SaveChangesAsync();
        }
    }

    public async Task StatuszFrissitesAsync(int meresId, MeresStatusz statusz, string? eredmeny)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.MeresStatusz = statusz;
            meres.Eredmeny = eredmeny;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task MentesJegyzokonyvAdatokkalEsStatuszAsync(
        int meresId, JegyzokonyvAdatok adatok, MeresStatusz statusz, string? eredmeny)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.JegyzokonyvAdatokJson = JsonSerializer.Serialize(adatok);
            meres.MeresStatusz = statusz;
            meres.Eredmeny = eredmeny;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<Meres?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int meresTipusId, DateTime ujMeresDatum)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Where(m => m.UgyfelId == ugyfelId
                        && m.TelephelyId == telephelyId
                        && m.MeresTipusId == meresTipusId
                        && m.Aktiv);

        var regi = await query.FirstOrDefaultAsync();
        if (regi == null) return null;

        // Ellenőrizzük, hogy az új mérés 40 napon belül van-e a régihez képest
        if (regi.KovetkezoDatum.HasValue)
        {
            var kulonbseg = Math.Abs((ujMeresDatum - regi.KovetkezoDatum.Value).TotalDays);
            if (kulonbseg <= 40)
            {
                return regi;
            }
        }

        return null;
    }

    public async Task InaktivvaTesz(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.Aktiv = false;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task AktivAllapotValtasAsync(int meresId, bool aktiv)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.Aktiv = aktiv;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> VanFrissebbAktivAsync(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres == null) return false;

        return await context.Meresek
            .Where(m => m.Id != meresId
                        && m.Aktiv
                        && m.UgyfelId == meres.UgyfelId
                        && m.TelephelyId == meres.TelephelyId
                        && m.MeresTipusId == meres.MeresTipusId
                        && m.Datum > meres.Datum)
            .AnyAsync();
    }

    public async Task<Meres> DuplikalAsync(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var eredeti = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == meresId)
            ?? throw new InvalidOperationException("Nem található a duplikálandó mérés.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(eredeti.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés duplikálásához.");
        }

        // Egyedi jegyzőkönyvszám generálása a másolathoz (a jegyzőkönyv szám egyedi azonosító, nem másolható változatlanul)
        var mellekletek = await context.MellekletJegyzokonyvek
            .Include(m => m.MellekletMeres)
            .Where(m => m.MeresId == meresId)
            .ToListAsync();

        string? ujFoJegyzokonyvSzam = null;
        if (!string.IsNullOrWhiteSpace(eredeti.JegyzokonyvAdatokJson) || mellekletek.Any())
        {
            ujFoJegyzokonyvSzam = await _munkaszamService.GeneralKovetkezoMunkaszamAsync(eredeti.Ugyfel!.CegId, eredeti.MeresTipusId);
        }

        var ujJegyzokonyvAdatokJson = ujFoJegyzokonyvSzam != null
            ? FrissitJegyzokonyvSzam(eredeti.JegyzokonyvAdatokJson, ujFoJegyzokonyvSzam)
            : eredeti.JegyzokonyvAdatokJson;

        var masolat = new Meres
        {
            UgyfelId = eredeti.UgyfelId,
            TelephelyId = eredeti.TelephelyId,
            HelyisegId = eredeti.HelyisegId,
            MeresTipusId = eredeti.MeresTipusId,
            Datum = eredeti.Datum,
            KovetkezoDatum = eredeti.KovetkezoDatum,
            Eredmeny = eredeti.Eredmeny,
            MeresStatusz = eredeti.MeresStatusz,
            Megjegyzes = string.IsNullOrWhiteSpace(eredeti.Megjegyzes)
                ? "- copy"
                : $"{eredeti.Megjegyzes} - copy",
            JegyzokonyvAdatokJson = ujJegyzokonyvAdatokJson,
            Aktiv = true,
            Letrehozva = DateTime.UtcNow
        };

        context.Meresek.Add(masolat);
        await context.SaveChangesAsync();

        // Melléklet jegyzőkönyvek (HVM, AVK, SZI, VVN stb.) duplikálása
        foreach (var melleklet in mellekletek)
        {
            int? ujMellekletMeresId = null;

            // A melléklet száma az "<alapszám>/<TIPUS>" mintát követi (pl. "JK-000001/2026/AVK") — az új, egyedi
            // fő jegyzőkönyvszám alapján állítjuk elő újra, hogy a melléklet sorszáma is egyedi maradjon.
            var ujMellekletSzam = ujFoJegyzokonyvSzam != null
                ? $"{ujFoJegyzokonyvSzam}/{melleklet.Tipus}"
                : melleklet.Szam;

            if (melleklet.MellekletMeres != null)
            {
                var mellekletMeresMasolat = new Meres
                {
                    UgyfelId = melleklet.MellekletMeres.UgyfelId,
                    TelephelyId = melleklet.MellekletMeres.TelephelyId,
                    HelyisegId = melleklet.MellekletMeres.HelyisegId,
                    MeresTipusId = melleklet.MellekletMeres.MeresTipusId,
                    Datum = melleklet.MellekletMeres.Datum,
                    KovetkezoDatum = melleklet.MellekletMeres.KovetkezoDatum,
                    Eredmeny = melleklet.MellekletMeres.Eredmeny,
                    MeresStatusz = melleklet.MellekletMeres.MeresStatusz,
                    Megjegyzes = melleklet.MellekletMeres.Megjegyzes,
                    JegyzokonyvAdatokJson = FrissitJegyzokonyvSzam(melleklet.MellekletMeres.JegyzokonyvAdatokJson, ujMellekletSzam),
                    Aktiv = true,
                    Letrehozva = DateTime.UtcNow
                };

                context.Meresek.Add(mellekletMeresMasolat);
                await context.SaveChangesAsync();
                ujMellekletMeresId = mellekletMeresMasolat.Id;
            }

            var mellekletMasolat = new MellekletJegyzokonyv
            {
                MeresId = masolat.Id,
                Tipus = melleklet.Tipus,
                Szam = ujMellekletSzam,
                Statusz = melleklet.Statusz,
                AdatokJson = melleklet.AdatokJson,
                MellekletMeresId = ujMellekletMeresId,
                Letrehozva = DateTime.UtcNow
            };

            context.MellekletJegyzokonyvek.Add(mellekletMasolat);
        }

        await context.SaveChangesAsync();

        return masolat;
    }

    private static string? FrissitJegyzokonyvSzam(string? jegyzokonyvAdatokJson, string ujSzam)
    {
        if (string.IsNullOrWhiteSpace(jegyzokonyvAdatokJson)) return jegyzokonyvAdatokJson;
        try
        {
            var adatok = JsonSerializer.Deserialize<JegyzokonyvAdatok>(jegyzokonyvAdatokJson);
            if (adatok == null) return jegyzokonyvAdatokJson;

            adatok.JegyzokonyvSzam = ujSzam;
            return JsonSerializer.Serialize(adatok);
        }
        catch
        {
            return jegyzokonyvAdatokJson;
        }
    }

    // --- Tagek ---

    public async Task<List<MeresTag>> GetAllTagekAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTagek
            .AsNoTracking()
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<MeresTag> CreateTagAsync(MeresTag tag)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.MeresTagek.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task UpdateTagAsync(MeresTag tag)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.MeresTagek.FindAsync(tag.Id)
            ?? throw new InvalidOperationException("Tag nem található.");
        existing.Nev = tag.Nev;
        existing.Szin = tag.Szin;
        await context.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var tag = await context.MeresTagek.FindAsync(id);
        if (tag is not null)
        {
            context.MeresTagek.Remove(tag);
            await context.SaveChangesAsync();
        }
    }

    public async Task SetTagekAsync(int meresId, List<int> tagIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var meres = await context.Meresek
            .Include(m => m.Tagek)
            .FirstOrDefaultAsync(m => m.Id == meresId)
            ?? throw new InvalidOperationException("Mérés nem található.");

        meres.Tagek.Clear();

        if (tagIds.Count > 0)
        {
            var tagek = await context.MeresTagek
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();
            foreach (var tag in tagek)
                meres.Tagek.Add(tag);
        }

        await context.SaveChangesAsync();
    }
}