namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Telephely lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class TelephelyListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Cim { get; set; }
    public string? UgyfelNev { get; set; }
    public string? Kapcsolattarto { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public bool Aktiv { get; set; }
}
