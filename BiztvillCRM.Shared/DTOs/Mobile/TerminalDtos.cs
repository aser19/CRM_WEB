namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Terminál lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class TerminalListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Azonosito { get; set; }
    public string? IpCim { get; set; }
    public string? TelephelyNev { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; }
}
