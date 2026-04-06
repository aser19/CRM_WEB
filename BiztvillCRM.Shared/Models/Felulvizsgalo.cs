using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Felülvizsgáló személy (a cég dolgozója, aki felülvizsgálatokat végez)
/// </summary>
public class Felulvizsgalo
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.Now;
    public DateTime? Modositva { get; set; }
    
    /// <summary>A felülvizsgáló neve</summary>
    public string Nev { get; set; } = string.Empty;
    
    /// <summary>Legmagasabb jogosultsági szint</summary>
    public FelulvizsgaloJogosultsag Jogosultsag { get; set; } = FelulvizsgaloJogosultsag.Segito;
    
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;
    
    // Cég kapcsolat
    public int CegId { get; set; }
    public Ceg? Ceg { get; set; }
    
    // Képzések gyűjteménye
    public ICollection<FelulvizsgaloKepzes> Kepzesek { get; set; } = [];
    
    // Számított tulajdonságok
    public bool LehetFelelosFelulvizsgalo => Jogosultsag >= FelulvizsgaloJogosultsag.Felelos;
    public bool LehetSegito => true;
    public bool LehetEllenor => Jogosultsag >= FelulvizsgaloJogosultsag.Ellenor;
    
    /// <summary>Az összes képzéstípus ID-ja (szabályellenőrzéshez)</summary>
    public IEnumerable<int> OsszesKepzesTipusId => 
        Kepzesek.Where(k => k.KepzesTipusId.HasValue).Select(k => k.KepzesTipusId!.Value);
}