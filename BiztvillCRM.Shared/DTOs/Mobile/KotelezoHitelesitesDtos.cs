namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Kötelező hitelesítés lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class KotelezoHitelesitesListItemDto
{
    public int Id { get; set; }
    public string Megnevezes { get; set; } = string.Empty;
    public string? JogszabalyiHivatkozas { get; set; }
    public int HitelesitesiIdoszakHonap { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; }
}
