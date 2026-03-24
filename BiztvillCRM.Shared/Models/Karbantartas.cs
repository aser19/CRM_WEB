namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz karbantartási esemény adatai.</summary>
public class Karbantartas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public int EszkozId { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Leiras { get; set; }
    public string? Elvegzo { get; set; }

    // Navigációs property
    public Eszkoz? Eszkoz { get; set; }
}
