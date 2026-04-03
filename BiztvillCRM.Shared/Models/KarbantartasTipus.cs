namespace BiztvillCRM.Shared.Models;

/// <summary>Karbantartás típusának törzsadatai (pl. féléves karbantartás, éves karbantartás).</summary>
public class KarbantartasTipus
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    
    /// <summary>Ismétlődés időtartama hónapokban. 0 = eseti (nem ismétlődő).</summary>
    public int IsmetlodesHonap { get; set; }
    
    /// <summary>Ha true, akkor eseti karbantartás - nem számol következő dátumot.</summary>
    public bool Eseti => IsmetlodesHonap == 0;
    
    public bool Aktiv { get; set; } = true;
}