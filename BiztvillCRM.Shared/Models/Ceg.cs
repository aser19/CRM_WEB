namespace BiztvillCRM.Shared.Models;

/// <summary>Cég (tenant) törzsadatai - multi-tenant rendszer alapja.</summary>
public class Ceg
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Adoszam { get; set; }
    public string? Cim { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; } = true;

    // Navigációs property-k
    public List<Felhasznalo> Felhasznalok { get; set; } = new();
    public List<Ugyfel> Ugyfelek { get; set; } = new();
}