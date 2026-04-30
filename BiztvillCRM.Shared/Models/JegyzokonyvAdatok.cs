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

    // === MINŐSÍTŐ IRAT - 3. OLDAL (2/2) - HIÁNYZÓ PROPERTY-K ===
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
    
    // ÚJ: Mérési rendszer típusa (TN/TT/IT) - egyszer választják ki az egész jegyzőkönyvre
    public string MeresiRendszerTipus { get; set; } = "TN";
    
    // ÚJ: Dinamikus mérési pont táblázat (ID=5 sablon 3. oldalán)
    public List<MeresiPontSor> MeresiPontok { get; set; } = new();
}