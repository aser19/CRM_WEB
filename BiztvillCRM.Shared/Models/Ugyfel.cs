namespace BiztvillCRM.Shared.Models;

/// <summary>Ügyfél törzsadatai.</summary>
public class Ugyfel
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Adoszam { get; set; }
    public string? Cim { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public bool Aktiv { get; set; } = true;

    // Céghez tartozás (tenant) - az ügyfél melyik céghez tartozik
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>
    /// Ügyfélcsoport azonosítója: ha ugyanaz a valós ügyfél több névre (cégre) is kér jegyzőkönyvet
    /// (pl. "Kiskutya Kft" és "FGR Kft"), ugyanazt a csoportot kell beállítani náluk. Ekkor a tokenes
    /// lekérdezés a csoport összes tagjának adatait egy listában mutatja.
    /// </summary>
    public int? UgyfelCsoportId { get; set; }
    public UgyfelCsoport? UgyfelCsoport { get; set; }

    // Navigációs property
    public List<Telephely> Telephelyek { get; set; } = new();

    /// <summary>Sekély másolatot készít az ügyfélről (navigációs propertyk nélkül).</summary>
    public Ugyfel SekélyMásolat() => new()
    {
        Id = Id,
        Nev = Nev,
        Adoszam = Adoszam,
        Cim = Cim,
        Email = Email,
        Telefon = Telefon,
        Aktiv = Aktiv,
        CegId = CegId,
        UgyfelCsoportId = UgyfelCsoportId,
        Letrehozva = Letrehozva,
        Modositva = Modositva
    };
}
