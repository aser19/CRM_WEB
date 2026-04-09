namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Üzemanyagtöltő-állomásoknál kötelező hitelesítések törzsadata.
/// Globális adat - minden felhasználó láthatja.
/// </summary>
public class KotelezoHitelesites
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }
    public string Megnevezes { get; set; } = string.Empty;
    public string? JogszabalyiHivatkozas { get; set; }
    public int HitelesitesiIdoszakHonap { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;
}