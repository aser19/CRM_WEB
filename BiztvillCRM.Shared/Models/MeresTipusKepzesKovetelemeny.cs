using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Kapcsolótábla a MeresTipus és KepzesTipus között
/// + Extra mezők a sablon generáláshoz
/// </summary>
public class MeresTipusKepzesKovetelemeny
{
    public int Id { get; set; }
    
    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }
    
    public int KepzesTipusId { get; set; }
    public KepzesTipus? KepzesTipus { get; set; }
    
    /// <summary>Sablon label (pl. "ÉBF", "VBF")</summary>
    [MaxLength(50)]
    public string? SablonLabel { get; set; }
    
    /// <summary>Kötelező képzés-e (true = kötelező, false = opcionális/alternatíva)</summary>
    public bool Kotelezo { get; set; } = true;
    
    /// <summary>
    /// Alternatíva csoport: 
    /// 0 = nincs alternatíva (kötelező vagy sima opcionális)
    /// Azonos pozitív szám = VAGY kapcsolat (legalább egy kell a csoportból)
    /// </summary>
    public int AlternativaCsoport { get; set; } = 0;
    
    /// <summary>Megjelenítési sorrend</summary>
    public int Prioritas { get; set; }
}