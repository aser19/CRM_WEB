namespace BiztvillCRM.Shared.Models;

/// <summary>Ügyszám/ügyirat adatai.</summary>
public class Ugyszam
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Szam { get; set; } = string.Empty;
    public string? Targy { get; set; }
    public int? UgyfelId { get; set; }
    public int? HatosagId { get; set; }
    public DateTime? Beerkezett { get; set; }
    public DateTime? Hatarido { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Lezart { get; set; } = false;

    // Navigációs propertyk
    public Ugyfel? Ugyfel { get; set; }
    public Hatosag? Hatosag { get; set; }
}