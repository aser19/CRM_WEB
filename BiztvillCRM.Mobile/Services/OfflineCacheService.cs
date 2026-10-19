using System.Text.Json;
using BiztvillCRM.Mobile.Data;
using BiztvillCRM.Shared.Dtos.Mobile;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Mobile.Services;

/// <summary>
/// Offline cache (szinkronizáció és megtekintés) a jegyzőkönyvekhez és hitelesítésekhez.
/// Csak pull-irányú szinkronizáció (szerver -> eszköz), mert a mobil app read-only.
/// </summary>
public class OfflineCacheService
{
    private readonly ApiClient _apiClient;

    public OfflineCacheService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>Bejelentkezés után (vagy manuális frissítéskor) letölti a szerver adatait a helyi cache-be.</summary>
    public async Task SzinkronizalAsync()
    {
        await using var db = new MobileDbContext();
        await db.Database.EnsureCreatedAsync();

        var jegyzokonyvek = await _apiClient.GetJegyzokonyvekAsync();
        db.Jegyzokonyvek.RemoveRange(db.Jegyzokonyvek);
        foreach (var jk in jegyzokonyvek)
        {
            db.Jegyzokonyvek.Add(new CachedJegyzokonyv
            {
                Id = jk.Id,
                Datum = jk.Datum,
                KovetkezoDatum = jk.KovetkezoDatum,
                UgyfelNev = jk.UgyfelNev,
                TelephelyNev = jk.TelephelyNev,
                MeresTipusNev = jk.MeresTipusNev,
                MeresStatusz = jk.MeresStatusz,
                Eredmeny = jk.Eredmeny
            });
        }

        var hitelesitesek = await _apiClient.GetHitelesitesekAsync();
        db.Hitelesitesek.RemoveRange(db.Hitelesitesek);
        foreach (var h in hitelesitesek)
        {
            db.Hitelesitesek.Add(new CachedHitelesites
            {
                Id = h.Id,
                Datum = h.Datum,
                LejaratDatum = h.LejaratDatum,
                UgyfelNev = h.UgyfelNev,
                TelephelyNev = h.TelephelyNev,
                EszkozTipusNev = h.EszkozTipusNev,
                EszkozAzonosito = h.EszkozAzonosito,
                HitelesitesStatusz = h.HitelesitesStatusz,
                VanMunkalap = h.VanMunkalap,
                VanBizonyitvany = h.VanBizonyitvany
            });
        }

        await db.SaveChangesAsync();

        var munkaszamok = await _apiClient.GetMunkaszamokAsync();
        db.Munkaszamok.RemoveRange(db.Munkaszamok);
        foreach (var mu in munkaszamok)
        {
            db.Munkaszamok.Add(new CachedMunkaszam
            {
                Id = mu.Id,
                Szam = mu.Szam,
                Megnevezes = mu.Megnevezes,
                Aktiv = mu.Aktiv,
                CegNev = mu.CegNev
            });
        }

        await db.SaveChangesAsync();

        var ugyfelek = await _apiClient.GetUgyfelekAsync();
        db.Ugyfelek.RemoveRange(db.Ugyfelek);
        foreach (var u in ugyfelek)
        {
            db.Ugyfelek.Add(new CachedUgyfel
            {
                Id = u.Id,
                Nev = u.Nev,
                Adoszam = u.Adoszam,
                Cim = u.Cim,
                Email = u.Email,
                Telefon = u.Telefon,
                Aktiv = u.Aktiv
            });
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Jegyzőkönyv részletek letöltése és cache-be mentése (lapozott / egyedi megtekintéskor).</summary>
    public async Task<JegyzokonyvDetailDto?> GetJegyzokonyvReszletekAsync(int id, bool probaljOnline = true)
    {
        if (probaljOnline)
        {
            try
            {
                var detail = await _apiClient.GetJegyzokonyvAsync(id);
                if (detail is not null)
                {
                    await using var db = new MobileDbContext();
                    var entitas = await db.Jegyzokonyvek.FirstOrDefaultAsync(x => x.Id == id);
                    if (entitas is not null)
                    {
                        entitas.Megjegyzes = detail.Megjegyzes;
                        entitas.JegyzokonyvAdatokJson = detail.JegyzokonyvAdatokJson;
                        entitas.VillamNemNormaAdatokJson = detail.VillamNemNormaAdatokJson;
                        entitas.MellekletekJson = JsonSerializer.Serialize(detail.Mellekletek);
                        await db.SaveChangesAsync();
                    }
                    return detail;
                }
            }
            catch
            {
                // Nincs hálózat / szerver elérhetetlen -> offline cache-ből szolgáljuk ki
            }
        }

        await using var offlineDb = new MobileDbContext();
        var cache = await offlineDb.Jegyzokonyvek.FirstOrDefaultAsync(x => x.Id == id);
        if (cache is null)
            return null;

        return new JegyzokonyvDetailDto
        {
            Id = cache.Id,
            Datum = cache.Datum,
            KovetkezoDatum = cache.KovetkezoDatum,
            UgyfelNev = cache.UgyfelNev,
            TelephelyNev = cache.TelephelyNev,
            MeresTipusNev = cache.MeresTipusNev,
            MeresStatusz = cache.MeresStatusz,
            Eredmeny = cache.Eredmeny,
            Megjegyzes = cache.Megjegyzes,
            JegyzokonyvAdatokJson = cache.JegyzokonyvAdatokJson,
            VillamNemNormaAdatokJson = cache.VillamNemNormaAdatokJson,
            Mellekletek = string.IsNullOrWhiteSpace(cache.MellekletekJson)
                ? new List<JegyzokonyvMellekletDto>()
                : JsonSerializer.Deserialize<List<JegyzokonyvMellekletDto>>(cache.MellekletekJson) ?? new()
        };
    }

    public async Task<List<CachedJegyzokonyv>> GetJegyzokonyvekCacheAsync()
    {
        await using var db = new MobileDbContext();
        await db.Database.EnsureCreatedAsync();
        return await db.Jegyzokonyvek.OrderByDescending(x => x.Datum).ToListAsync();
    }

    public async Task<List<CachedHitelesites>> GetHitelesitesekCacheAsync()
    {
        await using var db = new MobileDbContext();
        await db.Database.EnsureCreatedAsync();
        return await db.Hitelesitesek.OrderByDescending(x => x.Datum).ToListAsync();
    }

    public async Task<CachedHitelesites?> GetHitelesitesReszletekAsync(int id)
    {
        await using var db = new MobileDbContext();
        return await db.Hitelesitesek.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<CachedMunkaszam>> GetMunkaszamokCacheAsync()
    {
        await using var db = new MobileDbContext();
        await db.Database.EnsureCreatedAsync();
        return await db.Munkaszamok.OrderBy(x => x.Szam).ToListAsync();
    }

    public async Task<CachedMunkaszam?> GetMunkaszamReszletekAsync(int id)
    {
        await using var db = new MobileDbContext();
        return await db.Munkaszamok.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<CachedUgyfel>> GetUgyfelekCacheAsync()
    {
        await using var db = new MobileDbContext();
        await db.Database.EnsureCreatedAsync();
        return await db.Ugyfelek.OrderBy(x => x.Nev).ToListAsync();
    }

    public async Task<CachedUgyfel?> GetUgyfelReszletekAsync(int id)
    {
        await using var db = new MobileDbContext();
        return await db.Ugyfelek.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Kijelentkezéskor hívandó: törli a teljes helyi SQLite fájlt, hogy a kijelentkezett
    /// felhasználóhoz tartozó adatokból semmi ne maradjon a készüléken.
    /// </summary>
    public async Task TorolCacheAsync()
    {
        await using (var db = new MobileDbContext())
        {
            await db.Database.EnsureDeletedAsync();
        }

        var utvonal = MobileDbContext.GetAdatbazisUtvonal();
        if (File.Exists(utvonal))
            File.Delete(utvonal);
    }
}
