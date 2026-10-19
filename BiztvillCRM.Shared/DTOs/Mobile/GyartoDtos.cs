namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Gyártó lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class GyartoListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Orszag { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; }
}
