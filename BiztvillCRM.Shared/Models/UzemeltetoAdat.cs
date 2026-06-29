namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Üzemeltető által rögzített konkrét adat (egy ellenőrzés/felülvizsgálat/hitelesítés/képzés).
/// Minden adat egy sablonhoz tartozik, és csak a sablonban meghatározott mezőket tartalmazza.
/// </summary>
public class UzemeltetoAdat
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>Melyik sablonhoz tartozik az adat</summary>
    public int UzemeltetoSablonId { get; set; }
    public UzemeltetoSablon UzemeltetoSablon { get; set; } = null!;

    /// <summary>Az adat rögzítése dátuma</summary>
    public DateTime RogzitesDatum { get; set; }

    /// <summary>Következő ellenőrzés esedékessége (opcionális)</summary>
    public DateTime? KovetkezoEsedekesseg { get; set; }

    /// <summary>Státusz (Tervezett, Elvégezve, Lejárt, stb.)</summary>
    public string Statusz { get; set; } = "Tervezett";

    /// <summary>Melyik céghez tartozik az adat (tenant izolációhoz)</summary>
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>Ki rögzítette az adatot (üzemeltető felhasználó)</summary>
    public string RogzitoFelhasznaloId { get; set; } = string.Empty;
    public Felhasznalo RogzitoFelhasznalo { get; set; } = null!;

    /// <summary>A rögzített mezők értékei (JSON formátumban)</summary>
    public string MezoErtekekJson { get; set; } = "{}";

    /// <summary>Megjegyzés (opcionális)</summary>
    public string? Megjegyzes { get; set; }

    /// <summary>Aktív-e az adat</summary>
    public bool Aktiv { get; set; } = true;
}
