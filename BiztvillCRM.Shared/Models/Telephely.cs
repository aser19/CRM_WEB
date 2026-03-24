namespace BiztvillCRM.Shared.Models;

/// <summary>Ügyfélhez tartozó telephely adatai.</summary>
public class Telephely
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Cim { get; set; }
    public int UgyfelId { get; set; }
    public string? Kapcsolattarto { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public bool Aktiv { get; set; } = true;

    // Navigációs property
    public Ugyfel? Ugyfel { get; set; }
}
