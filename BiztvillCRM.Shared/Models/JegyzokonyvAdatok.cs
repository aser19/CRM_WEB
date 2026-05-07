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

    // === MELLÉKLETEK – CHECKBOXOK ===
    public bool MellekletHibavedelem { get; set; } = false;   // Hibavédelmi jgyk (Hurok)
    public bool MellekletAvk { get; set; } = false;            // Áramvédő kapcsolók
    public bool MellekletSzigeteles { get; set; } = false;     // Szigetelés ellenállás mérés
    public bool MellekletVillam { get; set; } = false;         // Norma szerinti Villám
    public bool MellekletVillamNem { get; set; } = false;      // Nem norma szerinti Villám

    // Automatikusan generált melléklet-számok (csak olvasásra a UI-ban)
    public string HibavedelmiJkv { get; set; } = "";           // meglévő – megtartva
    public string AvkJegyzokonyv { get; set; } = "";           // meglévő – megtartva
    public string SzigetelesiJkv { get; set; } = "";
    public string VillamJkv { get; set; } = "";
    public string VillamNemJkv { get; set; } = "";
    public string MellekletekSzama
    {
        get
        {
            int count = 0;
            if (MellekletHibavedelem) count++;
            if (MellekletAvk) count++;
            if (MellekletSzigeteles) count++;
            if (MellekletVillam) count++;
            if (MellekletVillamNem) count++;
            return count.ToString();
        }
    }

    // === MINŐSÍTŐ IRAT - 3. OLDAL - ÚJ MEZŐK ===
    public string MinositasEredmenye { get; set; } = "MEGFELELŐ";
    public string VizsgalatEredmenyMegjegyzes { get; set; } = "NINCS";
    public string TalaltHibak { get; set; } = "Nincsenek";
    public string HibaMellekletSzoveg { get; set; } = "1. melléklet alapján";
    public bool HibaMellekletSzukseges { get; set; } = false;

    // Word export
    public string MINOSITAS_EREDMENY { get; set; } = "MEGFELELŐ";
    public string HIBA_MELLEKLET_X { get; set; } = "☐";
    
    // === MINŐSÍTŐ IRAT - 3. OLDAL (2/2) - HIÁNYZÓ PROPERTY-K ===
    public bool ErvenyessegMegrendeles { get; set; }
    public bool ErvenyessegBelsoSzabalyzat { get; set; }
    public DateTime? ErvenyessegDatum { get; set; }
    
    public string? KovetkezoFelulvizsgalatTipus { get; set; } = null;
    public string KovetkezoFelulvizsgalatEgyeb { get; set; } = "";
    public DateTime? KovetkezoFelulvizsgalatDatum { get; set; }
    
    public string? HataridoTipus { get; set; } = null;
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

    // Következő felülvizsgálat checkboxok
    public bool KovFelulv50kW { get; set; }
    public bool KovFelulv32A { get; set; }
    public bool KovFelulvVMBSZ { get; set; }
    public bool KovFelulvRV300 { get; set; }

    // Határidő checkboxok
    public bool HataridoHarom { get; set; }
    public bool HataridoHat { get; set; }
    public bool HataridoRV { get; set; }
    public string MinositoIratMegjegyzes { get; set; } = "";

    // === 4. OLDAL – IDŐSZAKOS VBF ===
    // I. Névleges feszültség
    public string NevlegesFeszultseg { get; set; } = "230V";
    public string NevlegesFeszultsegTipus { get; set; } = "1fazis";

    // II. Földelési típus
    public string FoldelesiTipus { get; set; } = "A";
    public string FoldelesiTipusKod { get; set; } = "szonda";

    // III. Alapvető érintésvédelmi mód
    public string ErintesvedelmiMod { get; set; } = "TN-C-S";

    // === 4. OLDAL – Áramütés-elleni védelmi módok ===
    public bool Vedelem404 { get; set; } // A táplálás önműködő lekapcsolása (TN-/TT-/IT-rendszer)
    public bool Vedelem405 { get; set; } // Kettős vagy megerősített szigetelés
    public bool Vedelem406 { get; set; } // Villamos elválasztás
    public bool Vedelem407 { get; set; } // SELV/PELV törpefeszültség
    public bool Vedelem408 { get; set; } // Védő egyenpotenciáli összekötés, védővezetők, védőösszekötő-vezetők
    public bool Vedelem409 { get; set; } // Védelem földeletlen helyi egyenpotenciáli összekötéssel

    // === 4. OLDAL – Betáplálás és dokumentáció ===
    public string Betaplalas { get; set; } = "Nem része a felülvizsgálatnak"; // "Légvezeték", "Földkábel", "Nem része a felülvizsgálatnak"
    public string TartalekEnergia { get; set; } = "Nincs";                    // "Van", "Nincs"
    public string LegutolsoFelujitas { get; set; } = "Ismeretlen";            // év (pl. "2018") vagy "Ismeretlen"
    public string Dokumentaciok { get; set; } = "";                           // max 150 karakter

    // === JOGSZABÁLYOK (4. oldal) ===
    public List<KijeloltJogszabaly> KijeloltJogszabalyok { get; set; } = new();

    // === 5. OLDAL – MSZ HD 60364-6 ELLENŐRZÉSEK ===
    // Rögzített villamos berendezés szerkezetei
    public string Ellen5_Sz_A { get; set; } = "MF";
    public string Ellen5_Sz_B { get; set; } = "MF";
    public string Ellen5_Sz_C { get; set; } = "MF";

    // Megtekintéses ellenőrzések
    public string Ellen5_Me_A { get; set; } = "MF";
    public string Ellen5_Me_B { get; set; } = "MF";
    public string Ellen5_Me_C { get; set; } = "MF";
    public string Ellen5_Me_D { get; set; } = "MF";
    public string Ellen5_Me_E { get; set; } = "MF";
    public string Ellen5_Me_F { get; set; } = "MF";
    public string Ellen5_Me_G { get; set; } = "MF";
    public string Ellen5_Me_H { get; set; } = "MF";
    public string Ellen5_Me_I { get; set; } = "MF";
    public string Ellen5_Me_J { get; set; } = "MF";
    public string Ellen5_Me_K { get; set; } = "MF";
    public string Ellen5_Me_L { get; set; } = "MF";
    public string Ellen5_Me_M { get; set; } = "MF";
    public string Ellen5_Me_N { get; set; } = "MF";
    public string Ellen5_Me_O { get; set; } = "MF";
    public string Ellen5_Me_P { get; set; } = "MF";

    // Megjegyzések
    // === 5. OLDAL – MEGJEGYZÉSEK ===
    public string Ellen5_Sz_A_M { get; set; } = "";
    public string Ellen5_Sz_B_M { get; set; } = "";
    public string Ellen5_Sz_C_M { get; set; } = "";
    public string Ellen5_Me_A_M { get; set; } = "";
    public string Ellen5_Me_B_M { get; set; } = "";
    public string Ellen5_Me_C_M { get; set; } = "";
    public string Ellen5_Me_D_M { get; set; } = "";
    public string Ellen5_Me_E_M { get; set; } = "";
    public string Ellen5_Me_F_M { get; set; } = "";
    public string Ellen5_Me_G_M { get; set; } = "";
    public string Ellen5_Me_H_M { get; set; } = "";
    public string Ellen5_Me_I_M { get; set; } = "";
    public string Ellen5_Me_J_M { get; set; } = "";
    public string Ellen5_Me_K_M { get; set; } = "";
    public string Ellen5_Me_L_M { get; set; } = "";
    public string Ellen5_Me_M_M { get; set; } = "";
    public string Ellen5_Me_N_M { get; set; } = "";
    public string Ellen5_Me_O_M { get; set; } = "";
    public string Ellen5_Me_P_M { get; set; } = "";
    public string Ellen5_Megjegyzes { get; set; } = "";
    // === 5. OLDAL – MÉRÉSEK ===
    public string Ellen5_Mr_A { get; set; } = "MF";
    public string Ellen5_Mr_B { get; set; } = "MF";
    public string Ellen5_Mr_C { get; set; } = "MF";
    public string Ellen5_Mr_D { get; set; } = "MF";
    public string Ellen5_Mr_E { get; set; } = "MF";
    public string Ellen5_Mr_F { get; set; } = "MF";
    public string Ellen5_Mr_G { get; set; } = "MF";
    public string Ellen5_Mr_H { get; set; } = "MF";
    public string Ellen5_Mr_I { get; set; } = "MF";
    public string Ellen5_Mr_J { get; set; } = "MF";
    public string Ellen5_Mr_A_M { get; set; } = "";
    public string Ellen5_Mr_B_M { get; set; } = "";
    public string Ellen5_Mr_C_M { get; set; } = "";
    public string Ellen5_Mr_D_M { get; set; } = "";
    public string Ellen5_Mr_E_M { get; set; } = "";
    public string Ellen5_Mr_F_M { get; set; } = "";
    public string Ellen5_Mr_G_M { get; set; } = "";
    public string Ellen5_Mr_H_M { get; set; } = "";
    public string Ellen5_Mr_I_M { get; set; } = "";
    public string Ellen5_Mr_J_M { get; set; } = "";

    // === 6. OLDAL – OTSZ ELLENŐRZÉSEK ===
    public string Ellen6_A { get; set; } = "MF";
    public string Ellen6_B { get; set; } = "MF";
    public string Ellen6_C { get; set; } = "MF";
    public string Ellen6_D { get; set; } = "MF";
    public string Ellen6_E { get; set; } = "MF";
    public string Ellen6_F { get; set; } = "MF";
    public string Ellen6_G { get; set; } = "MF";
    public string Ellen6_H { get; set; } = "MF";
    public string Ellen6_I { get; set; } = "MF";
    public string Ellen6_J { get; set; } = "MF";
    public string Ellen6_K { get; set; } = "MF";
    public string Ellen6_L { get; set; } = "MF";
    public string Ellen6_M { get; set; } = "MF";
    public string Ellen6_N { get; set; } = "MF";
    public string Ellen6_O { get; set; } = "MF";
    public string Ellen6_P { get; set; } = "MF";
    public string Ellen6_A_M { get; set; } = "";
    public string Ellen6_B_M { get; set; } = "";
    public string Ellen6_C_M { get; set; } = "";
    public string Ellen6_D_M { get; set; } = "";
    public string Ellen6_E_M { get; set; } = "";
    public string Ellen6_F_M { get; set; } = "";
    public string Ellen6_G_M { get; set; } = "";
    public string Ellen6_H_M { get; set; } = "";
    public string Ellen6_I_M { get; set; } = "";
    public string Ellen6_J_M { get; set; } = "";
    public string Ellen6_K_M { get; set; } = "";
    public string Ellen6_L_M { get; set; } = "";
    public string Ellen6_M_M { get; set; } = "";
    public string Ellen6_N_M { get; set; } = "";
    public string Ellen6_O_M { get; set; } = "";
    public string Ellen6_P_M { get; set; } = "";
    public string Ellen6_Megjegyzes { get; set; } = "";

    // === 6. OLDAL – VMBSZ ELLENŐRZÉSEK ===
    public string Ellen6V_01 { get; set; } = "MF";
    public string Ellen6V_02 { get; set; } = "MF";
    public string Ellen6V_03 { get; set; } = "MF";
    public string Ellen6V_04 { get; set; } = "MF";
    public string Ellen6V_05 { get; set; } = "MF";
    public string Ellen6V_06 { get; set; } = "MF";
    public string Ellen6V_07 { get; set; } = "MF";
    public string Ellen6V_08 { get; set; } = "MF";
    public string Ellen6V_09 { get; set; } = "MF";
    public string Ellen6V_10 { get; set; } = "MF";
    public string Ellen6V_11 { get; set; } = "MF";
    public string Ellen6V_12 { get; set; } = "MF";
    public string Ellen6V_13 { get; set; } = "MF";
    public string Ellen6V_14 { get; set; } = "MF";
    public string Ellen6V_15 { get; set; } = "MF";
    public string Ellen6V_16 { get; set; } = "MF";
    public string Ellen6V_17 { get; set; } = "MF";
    public string Ellen6V_01_M { get; set; } = "";
    public string Ellen6V_02_M { get; set; } = "";
    public string Ellen6V_03_M { get; set; } = "";
    public string Ellen6V_04_M { get; set; } = "";
    public string Ellen6V_05_M { get; set; } = "";
    public string Ellen6V_06_M { get; set; } = "";
    public string Ellen6V_07_M { get; set; } = "";
    public string Ellen6V_08_M { get; set; } = "";
    public string Ellen6V_09_M { get; set; } = "";
    public string Ellen6V_10_M { get; set; } = "";
    public string Ellen6V_11_M { get; set; } = "";
    public string Ellen6V_12_M { get; set; } = "";
    public string Ellen6V_13_M { get; set; } = "";
    public string Ellen6V_14_M { get; set; } = "";
    public string Ellen6V_15_M { get; set; } = "";
    public string Ellen6V_16_M { get; set; } = "";
    public string Ellen6V_17_M { get; set; } = "";
    public string Ellen6V_Megjegyzes { get; set; } = "";
    public string Ellen6V_18 { get; set; } = "MF";
    public string Ellen6V_19 { get; set; } = "MF";
    public string Ellen6V_20 { get; set; } = "MF";
    public string Ellen6V_21 { get; set; } = "MF";
    public string Ellen6V_18_M { get; set; } = "";
    public string Ellen6V_19_M { get; set; } = "";
    public string Ellen6V_20_M { get; set; } = "";
    public string Ellen6V_21_M { get; set; } = "";

    // === 7. OLDAL – A VÉDELMEK ELLENŐRZÉSE ===
    public string Ellen7_Balesetvédelem { get; set; } = "";
    public string Ellen7_TulaAramvedelem { get; set; } = "";
    public string Ellen7_AramutesElleni { get; set; } = "";
    public string Ellen7_Villamvedelem { get; set; } = "";
    public string Ellen7_Tulfeszultseg { get; set; } = "";
    public string Ellen7_Feszultsegcsokkenes { get; set; } = "";
    public string Ellen7_Elektrosztatikus { get; set; } = "";
    public string Ellen7_Megjegyzes { get; set; } = "";
    public string Ellen7_AtfogoErtekeles { get; set; } = "";

    // === 7. OLDAL – ÁRAMKÖRÖK LEÍRÁSA HELYISÉGENKÉNT ===
    public List<AramkorSor> Aramkorok { get; set; } = new();
    public string Ellen7_AltalánosEszrevételek { get; set; } = "";
    public string Ellen7_MegjegyzesekEszrevételek { get; set; } = "";

} // <-- JegyzokonyvAdatok osztály lezárása

public class KijeloltJogszabaly
{
    public int JogszabalyId { get; set; }
    public string Szam { get; set; } = "";
    public string Cim { get; set; } = "";
    public bool Kivalasztva { get; set; } = true;
    public bool IsSzabvany { get; set; } = false; // ÚJ
}

public class AramkorSor
{
    public string HelyisegNev { get; set; } = "";
    public string Leiras { get; set; } = "";
}


