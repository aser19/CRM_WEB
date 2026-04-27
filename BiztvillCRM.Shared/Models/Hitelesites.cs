using BiztvillCRM.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz hitelesítésének adatai.</summary>
public class Hitelesites
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    // Ügyfél
    public int? UgyfelId { get; set; }
    public Ugyfel? Ugyfel { get; set; }

    // Telephely
    public int? TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }

    // Eszköz típus (Kútoszlop, Szintmérő, stb.)
    public int EszkozTipusId { get; set; }
    public EszkozTipus? EszkozTipus { get; set; }

    // Hatóság
    public int? HatosagId { get; set; }
    public Hatosag? Hatosag { get; set; }

    public int Darabszam { get; set; } = 1;
    public DateTime Datum { get; set; }
    
    /// <summary>Általános lejárati dátum (a teljes kútoszlopra/eszközre vonatkozik).</summary>
    public DateTime? LejaratDatum { get; set; }
    
    public HitelesitesStatusz HitelesitesStatusz { get; set; }
    public string? Megjegyzes { get; set; }
    
    /// <summary>
    /// JSON formátumban tárolt lista az egyedi eszközök (pisztolyok) eltérő lejárati dátumairól.
    /// Csak akkor töltjük ki, ha van olyan pisztoly, amelynek lejárata eltér az általánostól.
    /// </summary>
    public string? EgyediLejaratok { get; set; }
    
    /// <summary>
    /// Nem mapped property: az egyedi lejáratok strukturált formában.
    /// </summary>
    [NotMapped]
    public List<HitelesitesReszlet> EgyediLejaratokLista
    {
        get => string.IsNullOrWhiteSpace(EgyediLejaratok) 
            ? new List<HitelesitesReszlet>() 
            : JsonSerializer.Deserialize<List<HitelesitesReszlet>>(EgyediLejaratok) ?? new List<HitelesitesReszlet>();
        set => EgyediLejaratok = value.Any() 
            ? JsonSerializer.Serialize(value) 
            : null;
    }
}
