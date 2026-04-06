namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Jegyzőkönyv típushoz szükséges képzés követelmények
/// </summary>
public class MeresTipusKepzesKovetelemeny
{
    public int Id { get; set; }
    
    /// <summary>Melyik jegyzőkönyv típushoz tartozik</summary>
    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }
    
    /// <summary>Melyik képzéstípus szükséges</summary>
    public int KepzesTipusId { get; set; }
    public KepzesTipus? KepzesTipus { get; set; }
    
    /// <summary>Kötelező-e (true) vagy csak ajánlott (false)</summary>
    public bool Kotelezo { get; set; } = true;
    
    /// <summary>Alternatíva csoport - azonos számú követelmények VAGY kapcsolatban vannak</summary>
    /// <remarks>Pl. AlternativaCsoport=1: ÉBF VAGY VBF megléte elegendő</remarks>
    public int AlternativaCsoport { get; set; } = 0;
    
    /// <summary>Prioritás a megjelenítéshez (bizonyítvány szöveg sorrendje)</summary>
    public int Prioritas { get; set; } = 0;
    
    /// <summary>Egyedi label a sablonban (pl. "ÉV", "EBF", "VBF")</summary>
    public string? SablonLabel { get; set; }
}