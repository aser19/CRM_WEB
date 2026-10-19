namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Hatóság lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class HatosagListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Rovidites { get; set; }
    public string? Cim { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; }
}
