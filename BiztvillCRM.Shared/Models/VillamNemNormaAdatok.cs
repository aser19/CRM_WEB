namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A "Nem Norma szerinti Villámvédelmi Minősítő Irat" PDF generálásához szükséges adatok.
/// A dokumentum felépítése: 1. Címlap, 2. Főadatok, 3. Tartalomjegyzék, majd a további (mérési) oldalak.
/// </summary>
public class VillamNemNormaAdatok
{
    public string JegyzokonyvSzam { get; set; } = "";

    /// <summary>A kiállító cég azonosítója (bélyegzőkép betöltéséhez).</summary>
    public int CegId { get; set; }

    // === KIÁLLÍTÓ CÉG ADATAI ===
    public string CegNev { get; set; } = "";
    public string CegCim { get; set; } = "";
    public string CegWeb { get; set; } = "";
    public string CegTelefon { get; set; } = "";

    // === MEGRENDELŐ ADATAI (CÍMLAP + FŐADATOK) ===
    public string MegrendeloNev { get; set; } = "";
    public string MegrendeloCim { get; set; } = "";

    /// <summary>A megbízás tárgya (pl. "Kiskereskedelmi üzlet villámvédelmi felülvizsgálata").</summary>
    public string MegbizasTargya { get; set; } = "";

    /// <summary>Megrendelő képviselője (szabad szöveges mező, textbox-ból töltve).</summary>
    public string MegrendeloKepviseloje { get; set; } = "";

    // === FELELŐS FELÜLVIZSGÁLÓ (MÉRÉST VÉGZŐ) ===
    public string FelulvizsgaloNev { get; set; } = "";
    public string FelulvizsgaloBizonyitvany { get; set; } = "";
    public string FelulvizsgaloKepzes { get; set; } = "";

    // === SEGÍTŐ FELÜLVIZSGÁLÓ ===
    public string SegitoFelulvizsgalo { get; set; } = "";
    public string SegitoBizonyitvany { get; set; } = "";
    public string SegitoKepzes { get; set; } = "";

    // === ELLENŐR ===
    public string Ellenor { get; set; } = "";
    public string EllenorBizonyitvany { get; set; } = "";
    public string EllenorKepzes { get; set; } = "";

    // === MÉRÉS / ÉRVÉNYESSÉG ===
    public DateTime? MeresIdeje { get; set; }
    public DateTime? FelulvizsgalatErvenyessegeIg { get; set; }

    // === 4. OLDAL - BEVEZETÉS ===

    /// <summary>Ki készítette a vizsgálati jelentést / minősítő iratot: "Felelos", "Segito" vagy "Ellenor".</summary>
    public string KikeszitetteSzerep { get; set; } = "Felelos";

    /// <summary>A rendelkezésre bocsátott dokumentációk szabad szöveges felsorolása (1.2).</summary>
    public string RendelkezesreBocsatottDokumentaciok { get; set; } = "";

    /// <summary>A vizsgálatnál figyelembe vett rendeletek és szabványok (1.3), a Jogszabaly törzsadatból, "Villámvédelem" taggel szűrve.</summary>
    public List<KijeloltJogszabaly> KijeloltJogszabalyok { get; set; } = new();

    /// <summary>Igaz, ha a létesítmény eredeti műszaki kiírással nem rendelkezik (1.3 alján megjelenő szöveg).</summary>
    public bool NincsEredetiTervdokumentacio { get; set; } = false;

    /// <summary>A létesítmény építésének éve (a "nincs eredeti tervdokumentáció" eset esetén adandó meg).</summary>
    public int? LetesitmenyEpitesEve { get; set; }

    // === 5. OLDAL - A VIZSGÁLAT MÓDSZEREI ===

    /// <summary>A villámvédelmi besorolás 6 szempontja (2.1): Rendeltetés, Magasság, Tető anyaga, Körítő falak anyaga, A környező levegő szennyezettsége, Másodlagos hatás.</summary>
    public List<VillamBesorolasSor> VillamBesorolasSorok { get; set; } = new();

    /// <summary>2.2 - Szemrevételezés bevezető szövege (előtöltött alapszöveggel, szerkeszthető).</summary>
    public string SzemrevetelezesLeiras { get; set; } = "A szemrevételezéses ellenőrzés során megállapítást nyert a villámvédelmi csoportosítás helyessége.";

    /// <summary>Szemrevételezéssel vizsgált tételek (2.2) - checkbox lista.</summary>
    public List<SzemrevetelezesTetel> SzemrevetelezesTetelek { get; set; } = new();

    // === 6. OLDAL - MŰSZERES MÉRÉSEK (2.3) + AZ ÉPÍTMÉNY VIZSGÁLATA (3.1) ===

    /// <summary>A mérés idején az időjárás (pl. "száraz, napos").</summary>
    public string Idojaras { get; set; } = "";

    /// <summary>Az alkalmazott mérőműszerek (max. 2 sor, a JegyzokonyvSzerkeszto mintája szerint választva).</summary>
    public List<MuszerSor> Muszerek { get; set; } = new();

    /// <summary>A mérés körülményeinek rövid leírása (előtöltött alapszöveggel, szerkeszthető).</summary>
    public string MeresKorulmenyeinekLeirasa { get; set; } = "";

    /// <summary>Milyen szabvány szerint mi a mért érték (pl. "A földelési ellenállásmérés eredménye megfelelő, ha az érték kisebb mint 2Ω").</summary>
    public string MertErtekSzabvanySzerint { get; set; } = "";

    /// <summary>A mért érték (pl. "0,97Ω").</summary>
    public string MertErtek { get; set; } = "";

    /// <summary>A mért érték alapján a felülvizsgálat eredménye: "megfelelő" vagy "nem megfelelő".</summary>
    public string MeresEredmenyeAllapot { get; set; } = "megfelelő";

    /// <summary>3.1 - Milyen szabvány szerint történt a szükséges és a meglévő villámvédelmi berendezés összehasonlítása.</summary>
    public string OsszehasonlitasSzabvanySzerint { get; set; } = "";

    /// <summary>3.1 - Tűzveszélyességi osztályba sorolás (pl. „C” nem tűzveszélyes).</summary>
    public string TuzveszelyessegiOsztaly { get; set; } = "";

    /// <summary>
    /// 3.1 - Az építmény villámvédelmi csoportjai alapján történő besorolás eredménye (pl. "R1-M2-T3-K1-S3-H1").
    /// Alapból a VillamBesorolasSorok kódjaiból számítva, de manuálisan felülírható/választható.
    /// </summary>
    public string VillamvedelmiBesorolasEredmenye { get; set; } = "";

    /// <summary>A VillamBesorolasSorok kódjaiból kötőjellel összefűzött, számított besorolás (pl. "R1-M2-T3-K1-S3-H1").</summary>
    public string SzamitottVillamvedelmiBesorolas =>
        string.Join("-", (VillamBesorolasSorok ?? new()).Where(s => !string.IsNullOrWhiteSpace(s.Besorolas)).Select(s => s.Besorolas));

    // === 7. OLDAL - AZ ÉPÍTMÉNY VIZSGÁLATA (FOLYTATÁS) ===

    /// <summary>Az elemenkénti besorolás sorai (Felfogó, Levezető, Földelés, Belső villámvédelem, Méretfokozat).</summary>
    public List<VillamElemBesorolasSor> ElemBesorolasSorok { get; set; } = new();

    /// <summary>A felülvizsgálat összesített értékelése: "megfelelő" vagy "nem megfelelő".</summary>
    public string FelulvizsgalatErtekelese { get; set; } = "megfelelő";

    /// <summary>3.2 - A vizsgálat során tapasztalt hibák, hiányosságok (előtöltött alapszöveggel, szerkeszthető).</summary>
    public string HibakHianyossagok { get; set; } = "";

    /// <summary>3.3 - A felülvizsgálattal kapcsolatos észrevételek, megjegyzések (előtöltött alapszöveggel, szerkeszthető).</summary>
    public string EszrevetelekMegjegyzesek { get; set; } = "";

    // === 8. OLDAL - BELSŐ VILLÁMVÉDELEM (3.4) + ÁLTALÁNOS MINŐSÍTÉS (4.) + ZÁRÓ ADATOK ===

    /// <summary>3.4 - Igaz, ha a megrendelés tartalmazza a belső villámvédelem vizsgálatát.</summary>
    public bool BelsoVillamvedelemTartalmazza { get; set; } = true;

    /// <summary>3.4 - A belső villámvédelem leírása (előtöltött alapszöveggel, szerkeszthető).</summary>
    public string BelsoVillamvedelemLeirasa { get; set; } = "";

    /// <summary>3.4 - A belső villámvédelem állapota: "megfelelő" vagy "nem megfelelő".</summary>
    public string BelsoVillamvedelemAllapot { get; set; } = "megfelelő";

    /// <summary>4. - Az általános minősítés szövege (a korábban kiválasztott szabványokra hivatkozva, előtöltött, szerkeszthető).</summary>
    public string AltalanosMinositesSzovege { get; set; } = "";

    /// <summary>4. - Az általános minősítés eredménye: "megfelelő" vagy "nem megfelelő".</summary>
    public string AltalanosMinositesAllapot { get; set; } = "megfelelő";

    /// <summary>A következő villámvédelmi felülvizsgálat javasolt/kötelező időpontja.</summary>
    public DateTime? KovetkezoFelulvizsgalatIdopontja { get; set; }

    /// <summary>Hol készült a jegyzőkönyv (település neve).</summary>
    public string HolKeszult { get; set; } = "";

    /// <summary>Mikor készült a jegyzőkönyv (kelt dátuma).</summary>
    public DateTime? MikorKeszult { get; set; }
}

/// <summary>Egy sor az épület elemenkénti villámvédelmi besorolásához (7. oldal): pl. Felfogó, Levezető, Földelés, Belső villámvédelem, Méretfokozat.</summary>
public class VillamElemBesorolasSor
{
    /// <summary>Az elem megnevezése (pl. "A felfogó fokozata", "A meglévő felfogó állapota").</summary>
    public string Megnevezes { get; set; } = "";

    /// <summary>A fokozat/kialakítás szabad szöveges megadása (pl. "V2o", "L2o", "F2/r", "B2", "k / e").</summary>
    public string Fokozat { get; set; } = "";

    /// <summary>Az elem állapota: "megfelelő" vagy "nem megfelelő".</summary>
    public string Allapot { get; set; } = "megfelelő";
}

/// <summary>Egy sor a villámvédelmi besorolás táblázatában (2.1).</summary>
public class VillamBesorolasSor
{
    /// <summary>A szempont típusa: "Rendeltetes", "Magassag", "TetoAnyaga", "KoritoFalakAnyaga", "LevegoSzennyezettsege", "MasodlagosHatas".</summary>
    public string SzempontKod { get; set; } = "";

    /// <summary>A szempont megnevezése (pl. "Rendeltetés").</summary>
    public string Megnevezes { get; set; } = "";

    /// <summary>A kiválasztott besorolási kód (pl. "R1", "M2", "T3"...).</summary>
    public string Besorolas { get; set; } = "";

    /// <summary>Az összehasonlítás eredménye: "megfelelő" vagy "nem megfelelő".</summary>
    public string Allapot { get; set; } = "megfelelő";
}

/// <summary>Egy tétel a szemrevételezéssel vizsgált elemek listájában (2.2) - checkbox pipálással.</summary>
public class SzemrevetelezesTetel
{
    public string Megnevezes { get; set; } = "";
    public bool Kivalasztva { get; set; } = true;
}
