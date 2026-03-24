namespace BiztvillCRM.Shared.Models;

/// <summary>Ügyfélhez kapcsolódó tanúsítvány adatai.</summary>
public class Tanusitvany
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Szam { get; set; }
    public DateTime? KiadoDatum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public int UgyfelId { get; set; }
    public string? Megjegyzes { get; set; }

    // Navigációs property
    public Ugyfel? Ugyfel { get; set; }
}
