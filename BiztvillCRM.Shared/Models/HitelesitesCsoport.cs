using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Összetartozó hitelesítési kötelezettségek csoportja.
/// Pl. "Tartály 15 éves hitelesítési ciklus" = 15 éves hitelesítés
/// + 5 évente tömörségi vizsgálat + évente lyukadásjelző ellenőrzés.
/// </summary>
public class HitelesitesCsoport
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Nev { get; set; } = "";

    [MaxLength(500)]
    public string? Leiras { get; set; }

    /// <summary>A "főhitelesítés" eszköztípusa (pl. Tartály – 180 hónap)</summary>
    public int? FoEszkozTipusId { get; set; }
    public EszkozTipus? FoEszkozTipus { get; set; }

    public bool Aktiv { get; set; } = true;
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    public List<HitelesitesCsoportTag> Tagok { get; set; } = new();
}