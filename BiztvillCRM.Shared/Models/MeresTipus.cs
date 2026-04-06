namespace BiztvillCRM.Shared.Models;

public class MeresTipus
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.Now;
    public DateTime? Modositva { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public int? ErvenyessegHonap { get; set; }
    public bool Aktiv { get; set; } = true;
    
    public string? SablonId { get; set; }
    public string JegyzokonyvPrefix { get; set; } = "JKV";
    
    public ICollection<MeresTipusKepzesKovetelemeny> KepzesKovetelemenyei { get; set; } = [];
}
