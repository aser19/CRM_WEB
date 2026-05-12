using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Felhasználó–Cég kapcsolótábla (many-to-many), szerepkörrel.</summary>
public class FelhasznaloCeg
{
    public string FelhasznaloId { get; set; } = string.Empty;
    public Felhasznalo Felhasznalo { get; set; } = null!;

    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>Opcionális: felhasználó szerepköre az adott cégnél.</summary>
    public string? Szerep { get; set; }

    public DateTime Hozzaadva { get; set; } = DateTime.UtcNow;
}