using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz hitelesítésének adatai.</summary>
public class Hitelesites
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    // Eszköz típus (Kútoszlop, Szintmérő, stb.)
    public int EszkozTipusId { get; set; }
    public EszkozTipus? EszkozTipus { get; set; }

    // Hatóság
    public int? HatosagId { get; set; }
    public Hatosag? Hatosag { get; set; }

    public string? Ugyiratszam { get; set; }
    public int Darabszam { get; set; } = 1;
    public DateTime Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public HitelesitesStatusz HitelesitesStatusz { get; set; }
    public string? Megjegyzes { get; set; }
}
