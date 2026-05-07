using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>Egy adott vizsgálati sablon egy adott tételéhez rendelt alapértelmezett érték.</summary>
public class VizsgalatiSablonTetelErtek
{
    public int Id { get; set; }

    public int SablonId { get; set; }
    public VizsgalatiSablon? Sablon { get; set; }

    public int TetelId { get; set; }
    public JegyzokonyvSablonTetel? Tetel { get; set; }

    /// <summary>Az alapértelmezett érték ebben a sablonban (pl. "MF", "NMF", "NA").</summary>
    [MaxLength(50)]
    public string AlapertelmezettErtek { get; set; } = "MF";
}