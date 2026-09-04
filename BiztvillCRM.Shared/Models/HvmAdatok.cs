namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Hibavédelmi mérési jegyzőkönyv (HVM) adatai – melléklet, nem önálló mérés.
/// Első oldal: fejlap. Többi oldal: MeresiPontSor-ok (egymérés mintájára).
/// </summary>
public class HvmAdatok
{
    // === FEJLAP (1. oldal) ===

    /// <summary>Jelentés típusa (pl. "Használatba vételt megelőző")</summary>
    public string JelentesTipus { get; set; } = "Használatba vételt megelőző";

    /// <summary>Munkaszám (prefixelt, pl. BVP-MNB-0210)</summary>
    public string Munkaszam { get; set; } = "";

    /// <summary>Lapszám (pl. "1/4")</summary>
    public string Lapszam { get; set; } = "1";

    /// <summary>Összes lapszám (pl. 4)</summary>
    public int OsszesLap { get; set; } = 1;

    /// <summary>Mérés helye (pl. Magyar Nemzeti Bank, Bp., Szabadság tér 9.)</summary>
    public string MeresHelye { get; set; } = "";

    /// <summary>Mérés ideje (dátum)</summary>
    public DateTime MeresIdeje { get; set; } = DateTime.Today;

    /// <summary>Készítés dátuma</summary>
    public DateTime KeszitesDatum { get; set; } = DateTime.Today;

    // Felelős mérést végző
    public string FelelosNev { get; set; } = "";
    public string FelelosVegzettseg { get; set; } = "felülvizsgáló";
    public string FelelosBizonyitvany { get; set; } = "";

    /// <summary>Kiállító cég neve (az "Egy mérés" jegyzőkönyv stílusához, a fejlécben jelenik meg).</summary>
    public string CegNev { get; set; } = "";

    /// <summary>Kiállító cég címe (az "Egy mérés" jegyzőkönyv stílusához, a fejlécben jelenik meg).</summary>
    public string CegCim { get; set; } = "";

    /// <summary>Kapcsolattartó neve (a főjegyzőkönyvből átvéve).</summary>
    public string KapcsolatTarto { get; set; } = "";

    /// <summary>Belső azonosító szám (a főjegyzőkönyvből átvéve).</summary>
    public string UzemiKisero { get; set; } = "";

    /// <summary>Hálózat típusa (pl. TN, TT, IT) – a főjegyzőkönyvből átvéve.</summary>
    public string MeresiRendszerTipus { get; set; } = "TN";

    /// <summary>Felelős felülvizsgáló neve (aláírás/bélyegző kereséséhez az IFelulvizsgaloService-ben).</summary>
    public string FelulvizsgaloNev { get; set; } = "";

    // Segítő
    public string SegitoNev { get; set; } = "";
    public string SegitoVegzettseg { get; set; } = "";
    public string SegitoBizonyitvany { get; set; } = "";

    // Műszer adatok
    public string MuszerTipus { get; set; } = "";
    public string MuszerGyariSzam { get; set; } = "";
    public DateTime? MuszerKalibralasDatum { get; set; }

    // Műszer – legördülőből kiválasztva
    public int? MuszerEszkozId { get; set; }
    public string MuszerKalibralasStr { get; set; } = "";  // szöveges kalibrálás dátum

    // Régi egyszeres mezők HELYETT dinamikus lista (főjgyk mintájára):
    public List<MuszerSor> Muszerek { get; set; } = new();

    // Régi mezők – visszafelé kompatibilitáshoz megtarthatók, de a UI már a listát használja:
    // public string MuszerTipus ...
    // public string MuszerGyariSzam ...
    // public string MuszerKalibralasStr ...
    // public int? MuszerEszkozId ...

    // === MÉRÉSI ADATOK (2–N. oldal) – egymérés mintájára ===
    public List<MeresiPontSor> MeresiPontok { get; set; } = new();
}