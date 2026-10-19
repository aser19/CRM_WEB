namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Jogszabály lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class JogszabalyListItemDto
{
    public int Id { get; set; }
    public string Szam { get; set; } = string.Empty;
    public string Cim { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public string Tipus { get; set; } = string.Empty;
    public DateTime? HatalyosKezdet { get; set; }
    public DateTime? HatalyosVege { get; set; }
    public string? Url { get; set; }
    public string? Megjegyzes { get; set; }
    public bool JelenlegHatalyos { get; set; }
    public bool Aktiv { get; set; }
}
