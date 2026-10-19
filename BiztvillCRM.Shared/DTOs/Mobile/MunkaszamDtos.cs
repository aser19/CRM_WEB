namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Munkaszám lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class MunkaszamListItemDto
{
    public int Id { get; set; }
    public string Szam { get; set; } = string.Empty;
    public string? Megnevezes { get; set; }
    public bool Aktiv { get; set; }
    public string? CegNev { get; set; }
}
