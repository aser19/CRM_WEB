using BiztvillCRM.Shared.Enums;

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
    public UgyfelTipus UgyfelTipus { get; set; }
    public bool Aktiv { get; set; } = true;

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
        UgyfelTipus = UgyfelTipus,
        Aktiv = Aktiv,
        Letrehozva = Letrehozva,
        Modositva = Modositva
    };
}
