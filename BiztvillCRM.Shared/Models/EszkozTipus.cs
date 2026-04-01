namespace BiztvillCRM.Shared.Models;

/// <summary>Hitelesítendő eszköz típusa (Admin kezeli).</summary>
public class EszkozTipus
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public bool Aktiv { get; set; } = true;
    public DateTime Letrehozva { get; set; }
}