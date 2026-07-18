using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A súgó oldal egy kategóriáját (pl. "Mérések", "Törzsadatok") reprezentálja.
/// </summary>
public class SugoKategoria
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nev { get; set; } = "";

    /// <summary>
    /// Ikon azonosító (pl. "Business", "Speed"), amit egy megosztott dictionary
    /// képez le a MudBlazor Icons.Material.Filled értékeire.
    /// </summary>
    [MaxLength(50)]
    public string Icon { get; set; } = "Help";

    public int Sorrend { get; set; }

    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    public List<SugoTema> Temak { get; set; } = new();
}
