namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Az automatikus email értesítések futásának naplózására szolgál.
/// </summary>
public class AutomatikaFutasNaplo
{
    public int Id { get; set; }

    /// <summary>A futás UTC időpontja.</summary>
    public DateTime FutasiIdo { get; set; }

    /// <summary>Sikeres volt-e a feldolgozás.</summary>
    public bool Sikeres { get; set; }

    /// <summary>Feldolgozott hitelesítések száma.</summary>
    public int FeldolgozottHitelesitesek { get; set; }

    /// <summary>Feldolgozott mérések száma.</summary>
    public int FeldolgozottMeresek { get; set; }

    /// <summary>Sikeresen elküldött emailek száma.</summary>
    public int KuldottEmailek { get; set; }

    /// <summary>Sikertelen email küldések száma.</summary>
    public int SikertelenEmailek { get; set; }

    /// <summary>Hibaüzenet, ha volt.</summary>
    public string? Hiba { get; set; }
}