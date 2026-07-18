using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Munkaszám törzsadat - céghez rendelt munkaszám/projekt kód, ami a rendszer többi
/// moduljában (ügyfelek, mérések, karbantartások stb.) hozzárendelhető az egyes tételekhez.
/// </summary>
public class Munkaszam
{
    public int Id { get; set; }
    public int CegId { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>A munkaszám kódja/azonosítója (pl. "MSZ-2026-001").</summary>
    [StringLength(50, ErrorMessage = "A munkaszám maximum 50 karakter lehet!")]
    public string Szam { get; set; } = string.Empty;

    /// <summary>A munkaszámhoz tartozó megnevezés/leírás.</summary>
    [StringLength(300, ErrorMessage = "A megnevezés maximum 300 karakter lehet!")]
    public string? Megnevezes { get; set; }

    public bool Aktiv { get; set; } = true;

    public Ceg? Ceg { get; set; }
}
