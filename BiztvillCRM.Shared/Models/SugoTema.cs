using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A súgó egy kategórián belüli témáját (cím, leírás, opcionális videó link) reprezentálja.
/// </summary>
public class SugoTema
{
    public int Id { get; set; }

    public int SugoKategoriaId { get; set; }
    public SugoKategoria? SugoKategoria { get; set; }

    [Required]
    [MaxLength(200)]
    public string Cim { get; set; } = "";

    [Required]
    public string Leiras { get; set; } = "";

    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    public int Sorrend { get; set; }

    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }
}
