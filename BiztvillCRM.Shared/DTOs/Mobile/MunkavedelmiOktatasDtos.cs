namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Munkavédelmi oktatás lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class MunkavedelmiOktatasListItemDto
{
    public int Id { get; set; }
    public string Megnevezes { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public DateTime OktatasDatuma { get; set; }
    public DateTime? KovetkezoOktatas { get; set; }
    public string? OktatoNeve { get; set; }
    public string? Megjegyzes { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public bool Aktiv { get; set; }
}
