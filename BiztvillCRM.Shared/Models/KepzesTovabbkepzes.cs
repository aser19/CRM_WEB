namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy képzés továbbképzési eseménye
/// </summary>
public class KepzesTovabbkepzes
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.Now;
    
    // Képzés kapcsolat
    public int FelulvizsgaloKepzesId { get; set; }
    public FelulvizsgaloKepzes? FelulvizsgaloKepzes { get; set; }
    
    /// <summary>A továbbképzés dátuma</summary>
    public DateTime Datum { get; set; }
    
    /// <summary>Továbbképzési bizonyítvány száma</summary>
    public string? BizonyitvanySzam { get; set; }
    
    /// <summary>Továbbképzés helye/szervezője</summary>
    public string? Hely { get; set; }
    
    public string? Megjegyzes { get; set; }
}