namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy felülvizsgáló egy képzése/bizonyítványa
/// </summary>
public class FelulvizsgaloKepzes
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.Now;
    public DateTime? Modositva { get; set; }
    
    // Felülvizsgáló kapcsolat
    public int FelulvizsgaloId { get; set; }
    public Felulvizsgalo? Felulvizsgalo { get; set; }
    
    // Képzés típus
    public int? KepzesTipusId { get; set; }
    public KepzesTipus? KepzesTipus { get; set; }
    
    // Alapbizonyítvány adatok
    public string? BizonyitvanySzam { get; set; }
    public DateTime? BizonyitvanyKelte { get; set; }
    public DateTime? BizonyitvanyLejarat { get; set; }
    
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;
    
    // Továbbképzések gyűjteménye
    public ICollection<KepzesTovabbkepzes> Tovabbkepzesek { get; set; } = [];
    
    // Számított tulajdonságok
    public KepzesTovabbkepzes? UtolsoTovabbkepzes => 
        Tovabbkepzesek.OrderByDescending(t => t.Datum).FirstOrDefault();
    
    public DateTime? KovetkezoTovabbkepzesDatum
    {
        get
        {
            if (KepzesTipus?.TovabbkepzesKotelezo != true || KepzesTipus?.TovabbkepzesEvek == null)
                return null;
            
            var utolso = UtolsoTovabbkepzes?.Datum ?? BizonyitvanyKelte;
            return utolso?.AddYears(KepzesTipus.TovabbkepzesEvek.Value);
        }
    }
    
    public bool TovabbkepzesLejart => KovetkezoTovabbkepzesDatum.HasValue && 
                                       KovetkezoTovabbkepzesDatum.Value < DateTime.Today;
    
    public int? TovabbkepzesHatralevoNap => KovetkezoTovabbkepzesDatum.HasValue 
        ? (int)(KovetkezoTovabbkepzesDatum.Value - DateTime.Today).TotalDays 
        : null;
}