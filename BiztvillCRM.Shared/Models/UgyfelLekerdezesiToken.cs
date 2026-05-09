namespace BiztvillCRM.Shared.Models;

public class UgyfelLekerdezesiToken
{
    public int Id { get; set; }
    public int UgyfelId { get; set; }
    public Ugyfel? Ugyfel { get; set; }
    public string Token { get; set; } = "";
    public bool Aktiv { get; set; } = true;
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? UtolsoHasznalat { get; set; }
    public DateTime? LejarDatum { get; set; }

    /// <summary>Token érvényes, ha aktív és nem járt le.</summary>
    public bool ErvenyesE =>
        Aktiv && (LejarDatum == null || LejarDatum.Value > DateTime.UtcNow);
}