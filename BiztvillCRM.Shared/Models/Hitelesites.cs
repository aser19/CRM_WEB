using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz hitelesítésének adatai.</summary>
public class Hitelesites
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public int EszkozId { get; set; }
    public int? HatosagId { get; set; }
    public string? Ugyszam { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public HitelesitesStatusz HitelesitesStatusz { get; set; }
    public string? Megjegyzes { get; set; }

    // Navigációs propertyk
    public Eszkoz? Eszkoz { get; set; }
    public Hatosag? Hatosag { get; set; }
}
