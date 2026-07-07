namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Üzemeltetői sablon - admin által meghatározott kötelező ellenőrzések/felülvizsgálatok/hitelesítések/képzések.
/// Az üzemeltető felhasználók csak a sablonban meghatározott adatokat tudják rögzíteni.
/// </summary>
public class UzemeltetoSablon
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>A sablon neve (pl. "Benzinkút ellenőrzések")</summary>
    public string Nev { get; set; } = string.Empty;

    /// <summary>A sablon részletes leírása</summary>
    public string? Leiras { get; set; }

    /// <summary>Jogszabályi hivatkozás (opcionális)</summary>
    public string? JogszabalyiHivatkozas { get; set; }

    /// <summary>Ellenőrzési gyakoriság hónapokban (pl. 12 = évente)</summary>
    public int? EllenorzesiIdoszakHonap { get; set; }

    /// <summary>Aktív-e a sablon</summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>Melyik céghez tartozik a sablon (tenant izolációhoz)</summary>
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>Sablon létrehozója (admin felhasználó)</summary>
    public string LetrehozoFelhasznaloId { get; set; } = string.Empty;
    public Felhasznalo LetrehozoFelhasznalo { get; set; } = null!;

    /// <summary>Sablon mezők (milyen adatokat kell rögzíteni)</summary>
    public List<UzemeltetoSablonMezo> Mezok { get; set; } = new();

    /// <summary>Üzemeltetők által rögzített adatok</summary>
    public List<UzemeltetoAdat> Adatok { get; set; } = new();

    /// <summary>Sablonhoz rendelt üzemeltető felhasználók</summary>
    public List<UzemeltetoSablonFelhasznalo> Uzemeltetok { get; set; } = new();
}
