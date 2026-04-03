namespace BiztvillCRM.Shared.Models;

public class EszkozTipus
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public bool Aktiv { get; set; } = true;
    public DateTime Letrehozva { get; set; }
    
    /// <summary>Hitelesítési időtartam hónapokban (pl. 12 = 1 év, 24 = 2 év)</summary>
    public int HitelesitesiIdotartamHonap { get; set; } = 12;
}