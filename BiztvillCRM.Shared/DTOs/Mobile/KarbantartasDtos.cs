namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Karbantartás lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class KarbantartasListItemDto
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public string? KarbantartasTipusNev { get; set; }
    public string? Leiras { get; set; }
    public string? Elvegzo { get; set; }
    public string Statusz { get; set; } = string.Empty;
}
