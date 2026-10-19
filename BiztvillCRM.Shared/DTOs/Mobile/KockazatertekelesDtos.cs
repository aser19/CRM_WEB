namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Kockázatértékelés lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class KockazatertekelesListItemDto
{
    public int Id { get; set; }
    public string Megnevezes { get; set; } = string.Empty;
    public DateTime ErtekelesDatuma { get; set; }
    public DateTime? KovetkezoFelulvizsgalat { get; set; }
    public string KockazatiSzint { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public string? Intezkedesek { get; set; }
    public string? FelelosNeve { get; set; }
    public string Statusz { get; set; } = string.Empty;
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public bool Aktiv { get; set; }
}
