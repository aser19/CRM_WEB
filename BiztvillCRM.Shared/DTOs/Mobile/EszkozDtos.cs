namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Eszköz lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class EszkozListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? GyariSzam { get; set; }
    public string? Tipus { get; set; }
    public string? Kategoria { get; set; }
    public string? GyartoNev { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public bool Aktiv { get; set; }
}
