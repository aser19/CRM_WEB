namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Ügyfélcsoport: ugyanazon valós személy/tulajdonos több "Ugyfel" (jegyzőkönyvezési név) rekordját
/// fogja össze, pl. amikor egy ügyfél hol "Kiskutya Kft", hol "FGR Kft" nevére kéri a jegyzőkönyvet.
/// A tokenes lekérdezésnél a csoport minden tagjának adatai egy listában jelennek meg.
/// </summary>
public class UgyfelCsoport
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;

    /// <summary>Megjegyzés/elnevezés az adminisztrátorok számára (pl. "Kovács János").</summary>
    public string Nev { get; set; } = string.Empty;

    public List<Ugyfel> Tagok { get; set; } = new();
}
