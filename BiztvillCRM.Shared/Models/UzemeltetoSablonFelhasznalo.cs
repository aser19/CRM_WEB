namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Many-to-many kapcsolat az üzemeltetői sablonok és a felhasználók között.
/// Egy üzemeltető felhasználó több sablonhoz is hozzárendelhető, és egy sablonhoz több üzemeltető is tartozhat.
/// </summary>
public class UzemeltetoSablonFelhasznalo
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }

    /// <summary>Sablon azonosító</summary>
    public int UzemeltetoSablonId { get; set; }
    public UzemeltetoSablon UzemeltetoSablon { get; set; } = null!;

    /// <summary>Üzemeltető felhasználó azonosító</summary>
    public string FelhasznaloId { get; set; } = string.Empty;
    public Felhasznalo Felhasznalo { get; set; } = null!;

    /// <summary>Hozzárendelés létrehozója (admin/cégadmin)</summary>
    public string HozzarendeloFelhasznaloId { get; set; } = string.Empty;
    public Felhasznalo? HozzarendeloFelhasznalo { get; set; }

    /// <summary>Aktív-e a hozzárendelés</summary>
    public bool Aktiv { get; set; } = true;
}
