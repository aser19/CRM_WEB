namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Képzés lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class KepzesListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string Jogosultsag { get; set; } = string.Empty;
    public string? KepzesTipusNev { get; set; }
    public string? BizonyitvanySzam { get; set; }
    public DateTime? BizonyitvanyKelte { get; set; }
    public DateTime? BizonyitvanyLejarat { get; set; }
    public string? TovabbkepzesSzam { get; set; }
    public DateTime? UtolsoTovabbkepzes { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; }
}
