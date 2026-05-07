using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

public class MellekletJegyzokonyv
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    public int MeresId { get; set; }
    public Meres? Meres { get; set; }

    /// <summary>HVM, AVK, SZI, VVN, VVN-NEM</summary>
    [MaxLength(20)]
    public string Tipus { get; set; } = string.Empty;

    /// <summary>pl. VBF-2024-001/HVM</summary>
    [MaxLength(100)]
    public string Szam { get; set; } = string.Empty;

    /// <summary>Folyamatban / Kesz</summary>
    [MaxLength(20)]
    public string Statusz { get; set; } = MellekletStatusz.Folyamatban;

    /// <summary>A melléklet jgyk. félkész vagy kész adatai JSON-ban.</summary>
    public string? AdatokJson { get; set; }

    /// <summary>A melléklethez tartozó önálló Meres rekord Id-je (opcionális, ha már létrehozták).</summary>
    public int? MellekletMeresId { get; set; }
    public Meres? MellekletMeres { get; set; }

    // Számított
    public bool KeszeE => Statusz == MellekletStatusz.Kesz;

    public static string TipusNev(string tipus) => tipus switch
    {
        "HVM"     => "Hibavédelmi jgyk (Hurok)",
        "AVK"     => "Áramvédő kapcsolók",
        "SZI"     => "Szigetelés ellenállás mérés",
        "VVN"     => "Norma szerinti Villám",
        "VVN-NEM" => "Nem norma szerinti Villám",
        _         => tipus
    };
}

public static class MellekletStatusz
{
    public const string Folyamatban = "Folyamatban";
    public const string Kesz        = "Kesz";
}