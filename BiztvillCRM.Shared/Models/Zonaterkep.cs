namespace BiztvillCRM.Shared.Models;

public class Zonaterkep
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Megnevezes { get; set; } = string.Empty;
    public ZonaTipus ZonaTipus { get; set; } = ZonaTipus.Zone1;
    public string? Leiras { get; set; }
    public string? FajlNev { get; set; }
    public string? FajlUtvonal { get; set; }
    public DateTime? ErvenyessegKezdete { get; set; }
    public DateTime? ErvenyessegVege { get; set; }
    public bool Aktiv { get; set; } = true;

    // Ügyfél, Telephely, Cég
    public int UgyfelId { get; set; }
    public Ugyfel Ugyfel { get; set; } = null!;
    
    public int? TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }
    
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;
}

public enum ZonaTipus
{
    Zone0 = 0,
    Zone1 = 1,
    Zone2 = 2,
    Zone20 = 20,
    Zone21 = 21,
    Zone22 = 22
}