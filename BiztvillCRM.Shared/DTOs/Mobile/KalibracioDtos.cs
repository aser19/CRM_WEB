namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Kalibráció lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class KalibracioListItemDto
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? EszkozNev { get; set; }
    public string? UgyfelNev { get; set; }
    public string? Bizonyitvany { get; set; }
    public string? Elvegzo { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Sikeres { get; set; }
}
