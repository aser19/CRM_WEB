namespace BiztvillCRM.Shared.Models;

/// <summary>Mérőeszköz / berendezés törzsadatai.</summary>
public class Eszkoz
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? GyariSzam { get; set; }
    public string? Tipus { get; set; }
    public int GyartoId { get; set; }
    public int UgyfelId { get; set; }
    public int? TelephelyId { get; set; }
    public bool Aktiv { get; set; } = true;

    // Navigációs propertyk
    public Gyarto? Gyarto { get; set; }
    public Ugyfel? Ugyfel { get; set; }
    public Telephely? Telephely { get; set; }
}
