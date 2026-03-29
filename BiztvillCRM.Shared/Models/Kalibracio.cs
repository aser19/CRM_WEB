namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz kalibrációs esemény adatai.</summary>
public class Kalibracio
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public int EszkozId { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Bizonyitvany { get; set; }
    public string? Elvegzo { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Sikeres { get; set; } = true;

    // Navigációs property
    public Eszkoz? Eszkoz { get; set; }
}