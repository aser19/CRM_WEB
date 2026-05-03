namespace BiztvillCRM.Shared.Models;

public class JegyzokonyvAdatok
{
    public string JegyzokonyvSzam { get; set; } = "";
    public string CegNev { get; set; } = "";
    public string CegCim { get; set; } = "";
    public string CegWeb { get; set; } = "";
    public string CegTelefon { get; set; } = "";
    public string VizsgalatHelye { get; set; } = "";
    public string VizsgalatTargya { get; set; } = "";
    public string VizsgaltBerendezes { get; set; } = "";
    public string Megrendelo { get; set; } = "";
    public string VizsgalatIdotartama { get; set; } = "";
    public string Eredmeny { get; set; } = "";
    public string Megjegyzes { get; set; } = "";
    public string UzemiKisero { get; set; } = "";
    public string KapcsolatTarto { get; set; } = "";

    // Felelős felülvizsgáló
    public string FelulvizsgaloNev { get; set; } = "";
    public string FelulvizsgaloBizonyitvany { get; set; } = "";
    public string FelulvizsgaloKepzes { get; set; } = "";

    // Segítő felülvizsgáló
    public string SegitoFelulvizsgalo { get; set; } = "";
    public string SegitoBizonyitvany { get; set; } = "";
    public string SegitoKepzes { get; set; } = "";

    // Ellenőr
    public string Ellenor { get; set; } = "";
    public string EllenorBizonyitvany { get; set; } = "";
    public string EllenorKepzes { get; set; } = "";

    // === MINŐSÍTŐ IRAT - 2. OLDAL ===
    public string HibakB { get; set; } = "";
    public string HibakC { get; set; } = "";
    public string HibakD { get; set; } = "";
    public string HibakE { get; set; } = "";
    public string VegsoMinosites { get; set; } = "";
    public string MellekletekSzama { get; set; } = "";
    public string HibavedelmiJkv { get; set; } = "";
    public string AvkJegyzokonyv { get; set; } = "";

    // === MINŐSÍTŐ IRAT - 3. OLDAL - ÚJ MEZŐK ===
    public string MinositasEredmenye { get; set; } = "MEGFELELŐ";
    public string VizsgalatEredmenyMegjegyzes { get; set; } = "NINCS";
    public string TalaltHibak { get; set; } = "Nincsenek";
    public string HibaMellekletSzoveg { get; set; } = "1. melléklet alapján";
    public bool HibaMellekletSzukseges { get; set; } = false;

    // Word export
    public string MINOSITAS_EREDMENY { get; set; } = "MEGFELELŐ";
    public string HIBA_MELLEKLET_X { get; set; } = "☐";

    // === MINŐSÍTŐ IRAT - 3. OLDAL ===
    public bool ErvenyessegMegrendeles { get; set; }
    public bool ErvenyessegBelsoSzabalyzat { get; set; }
    public DateTime? ErvenyessegDatum { get; set; }

    public string KovetkezoFelulvizsgalatTipus { get; set; } = "";
    public string KovetkezoFelulvizsgalatEgyeb { get; set; } = "";
    public DateTime? KovetkezoFelulvizsgalatDatum { get; set; }

    public string HataridoTipus { get; set; } = "";
    public string HataridoEgyeb { get; set; } = "";
    public string MinositoMegjegyzes { get; set; } = "";

    // Word export placeholderek
    public string ERV_MEGRENDELES_X { get; set; } = "☐";
    public string ERV_SZABALYZAT_X { get; set; } = "☐";
    public string ERV_DATUM { get; set; } = "";
    public string KOV_50KW_X { get; set; } = "☐";
    public string KOV_32A_X { get; set; } = "☐";
    public string KOV_VMBSZ_X { get; set; } = "☐";
    public string KOV_RV300_X { get; set; } = "☐";
    public string KOV_EGYEB1_X { get; set; } = "☐";
    public string KOV_EGYEB1_SZOVEG { get; set; } = "";
    public string HAT_3EV_X { get; set; } = "☐";
    public string HAT_3EV_DATUM { get; set; } = "";
    public string HAT_LAKAS_X { get; set; } = "☐";
    public string HAT_RV_X { get; set; } = "☐";
    public string HAT_EGYEB2_X { get; set; } = "☐";
    public string HAT_EGYEB2_SZOVEG { get; set; } = "";
    public string HAT_6EV_DATUM { get; set; } = "";
    public string MINOSITO_MEGJEGYZES { get; set; } = "";

    // === HORDOZHATÓ KÉSZÜLÉK SPECIFIKUS ===
    public string? DolgozoNeve { get; set; }
    public string? ForgalmiRendszam { get; set; }
    public string? Munkaszam { get; set; }
    public string? MatricaSorszamTol { get; set; }
    public string? MatricaSorszamIg { get; set; }

    public bool VanMatricaSorszam =>
        !string.IsNullOrWhiteSpace(MatricaSorszamTol) &&
        !string.IsNullOrWhiteSpace(MatricaSorszamIg);

    // Műszerek
    public string? Muszer1Tipus { get; set; }
    public string? Muszer1GyariSzam { get; set; }
    public string? Muszer1Kalibralas { get; set; }
    public string? Muszer2Tipus { get; set; }
    public string? Muszer2GyariSzam { get; set; }
    public string? Muszer2Kalibralas { get; set; }
    public string? Muszer3Tipus { get; set; }
    public string? Muszer3GyariSzam { get; set; }
    public string? Muszer3Kalibralas { get; set; }

    // Dinamikus eszközlista
    public List<HordozhatoEszkozSor> Eszkozok { get; set; } = new();

    // Dinamikus műszerlista
    public List<MuszerSor> Muszerek { get; set; } = new();

    // Mérési rendszer típusa (TN/TT/IT)
    public string MeresiRendszerTipus { get; set; } = "TN";

    // Dinamikus mérési pont táblázat
    public List<MeresiPontSor> MeresiPontok { get; set; } = new();

    // Következő felülvizsgálat checkboxok
    public bool KovFelulv50kW { get; set; }
    public bool KovFelulv32A { get; set; }
    public bool KovFelulvVMBSZ { get; set; }
    public bool KovFelulvRV300 { get; set; }
    public string KovFelulvTipus { get; set; } = "";

    // Határidő checkboxok
    public bool HataridoHarom { get; set; }
    public bool HataridoHat { get; set; }
    public bool HataridoRV { get; set; }
    public string HataridoTipusRadio { get; set; } = "";

    // === 4. OLDAL – MINŐSÍTÉSI ALAPADATOK ===
    public string NevlegesFeszultseg { get; set; } = "";
    public string NevlegesFeszultsegTipus { get; set; } = "";
    public string FoldelesiTipus { get; set; } = "";
    public string FoldelesiTipusKod { get; set; } = "";
    public string ErintesvedelmiMod { get; set; } = "";

    // Áramütés elleni védelmi módok
    public bool Vedelem404 { get; set; }
    public bool Vedelem405 { get; set; }
    public bool Vedelem406 { get; set; }
}