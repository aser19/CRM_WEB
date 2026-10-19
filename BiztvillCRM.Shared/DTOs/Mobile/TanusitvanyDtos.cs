namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Tanúsítvány lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class TanusitvanyListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Szam { get; set; }
    public DateTime? KiadoDatum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? Megjegyzes { get; set; }
}
