using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Összetartozó méréstípusok csoportja.
/// Pl. "Tartály 15 éves hitelesítési ciklus" = hitelesítés + tömörségi + lyukadásjelző
/// </summary>
public class MeresCsoport
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Nev { get; set; } = "";

    [MaxLength(500)]
    public string? Leiras { get; set; }

    /// <summary>A "főhitelesítés" méréstípusa (pl. 15 éves tartály hitelesítés)</summary>
    public int? FoMeresTipusId { get; set; }
    public MeresTipus? FoMeresTipus { get; set; }

    public bool Aktiv { get; set; } = true;
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    public List<MeresCsoportTag> Tagok { get; set; } = new();
}