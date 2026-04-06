namespace BiztvillCRM.Shared.Models;

public class KepzesTipus
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Label { get; set; }  // ← ÚJ
    
    public bool Lejar { get; set; } = false;
    public int? LejaratEvek { get; set; }
    
    public bool TovabbkepzesKotelezo { get; set; } = false;
    public int? TovabbkepzesEvek { get; set; }
    public bool TovabbkepzesCsakFelulvizsgalonak { get; set; } = false;
    
    public string? Leiras { get; set; }
    public bool Aktiv { get; set; } = true;
    
    public ICollection<Kepzes> Kepzesek { get; set; } = [];
    public ICollection<KepzesSzabaly> ForrasSzabalyok { get; set; } = [];
    public ICollection<KepzesSzabaly> CelSzabalyok { get; set; } = [];
}