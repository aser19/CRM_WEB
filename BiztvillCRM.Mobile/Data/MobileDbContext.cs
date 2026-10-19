using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Mobile.Data;

/// <summary>
/// Helyi (eszközön tárolt) SQLite adatbázis a bejelentkezett felhasználó jegyzőkönyveinek
/// és hitelesítéseinek offline megtekintéséhez. Csak olvasási (cache) célt szolgál,
/// a szerver (BiztvillCRM.Data / SQL Server) marad az elsődleges adatforrás.
/// Kijelentkezéskor a teljes fájlt töröljük (lásd OfflineCacheService.TorolCacheAsync).
/// </summary>
public class MobileDbContext : DbContext
{
    public static string GetAdatbazisUtvonal()
        => Path.Combine(FileSystem.AppDataDirectory, "biztvillcrm_offline_cache.db3");

    public DbSet<CachedJegyzokonyv> Jegyzokonyvek => Set<CachedJegyzokonyv>();
    public DbSet<CachedHitelesites> Hitelesitesek => Set<CachedHitelesites>();
    public DbSet<CachedMunkaszam> Munkaszamok => Set<CachedMunkaszam>();
    public DbSet<CachedUgyfel> Ugyfelek => Set<CachedUgyfel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={GetAdatbazisUtvonal()}");
        }
    }
}

public class CachedJegyzokonyv
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public string? MeresTipusNev { get; set; }
    public string MeresStatusz { get; set; } = string.Empty;
    public string? Eredmeny { get; set; }
    public string? Megjegyzes { get; set; }
    public string? JegyzokonyvAdatokJson { get; set; }
    public string? VillamNemNormaAdatokJson { get; set; }

    /// <summary>A mellékletek JSON-szerializálva (egyszerűsített offline tárolás).</summary>
    public string? MellekletekJson { get; set; }
}

public class CachedHitelesites
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public string? EszkozTipusNev { get; set; }
    public string? EszkozAzonosito { get; set; }
    public string HitelesitesStatusz { get; set; } = string.Empty;
    public bool VanMunkalap { get; set; }
    public bool VanBizonyitvany { get; set; }
    public string? Megjegyzes { get; set; }
    public string? HatosagNev { get; set; }
    public int MunkalapFajlokSzama { get; set; }
    public int BizonyitvanyFajlokSzama { get; set; }
}

public class CachedMunkaszam
{
    public int Id { get; set; }
    public string Szam { get; set; } = string.Empty;
    public string? Megnevezes { get; set; }
    public bool Aktiv { get; set; }
    public string? CegNev { get; set; }
}

public class CachedUgyfel
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Adoszam { get; set; }
    public string? Cim { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public bool Aktiv { get; set; }
}
