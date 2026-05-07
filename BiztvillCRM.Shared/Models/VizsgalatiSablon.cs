using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>Vizsgálati sablon (pl. "Családi ház", "Iroda"), amely
/// az 5-7. oldal combo box értékeit előre kitölti.</summary>
public class VizsgalatiSablon
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }

    [MaxLength(200)]
    public string Nev { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Leiras { get; set; }

    public bool Aktiv { get; set; } = true;

    public List<VizsgalatiSablonTetelErtek> TetelErtekek { get; set; } = [];
}