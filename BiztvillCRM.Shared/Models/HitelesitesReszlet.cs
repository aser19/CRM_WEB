using System.ComponentModel.DataAnnotations.Schema;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy hitelesítéshez tartozó egyedi eszköz (pisztoly, átfolyásmérő, stb.) részletei,
/// ha annak lejárata eltér a hitelesítés általános lejárati dátumától.
/// </summary>
public class HitelesitesReszlet
{
    /// <summary>Eszköz azonosítója/neve (pl. "2-es benzin pisztoly", "1-es dízel nozzle").</summary>
    public string EszkozNev { get; set; } = string.Empty;
    
    /// <summary>Egyedi lejárati dátum erre az eszközre.</summary>
    public DateTime LejaratDatum { get; set; }
    
    /// <summary>Megjegyzés (pl. "javítás után újrahitelesítve").</summary>
    public string? Megjegyzes { get; set; }
    
    /// <summary>Helper property a MudDatePicker számára (nullable DateTime).</summary>
    [NotMapped]
    public DateTime? LejaratDatumNullable
    {
        get => LejaratDatum;
        set => LejaratDatum = value ?? DateTime.Today;
    }
}