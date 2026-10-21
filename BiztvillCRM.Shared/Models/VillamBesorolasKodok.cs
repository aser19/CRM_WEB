namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A villámvédelmi besorolás (MSZ 274 szabványsorozat) szempontjaihoz tartozó kódok és rövid
/// tájékoztató jellegű magyarázataik, amelyeket a szerkesztő UI-n a combobox melletti/popup
/// segítségként lehet megjeleníteni.
/// FONTOS: a lenti leírások általános, közkeletű tájékoztatásra szolgálnak, a konkrét besorolást
/// minden esetben a hatályos szabvány/szakértő állapítja meg!
/// </summary>
public static class VillamBesorolasKodok
{
    /// <summary>Rendeltetés szerinti kockázati osztály (R1 = legalacsonyabb kockázat/védelmi igény, R5 = legmagasabb).</summary>
    public static readonly Dictionary<string, string> Rendeltetes = new()
    {
        ["R1"] = "Közönséges épület, építmény. R2..R5 csoportba nem tartozó építmény (ideértve a gyárkéményt, fémtartályt) )",
        ["R2"] = "Kiemelt épület, építmény. R3..R5 csoportba nem tartozó építmény. Napi személyforgalom >1000 fő, óránként >300 fő 5 percenként >50 fő. Nagyüzemi állattartás. Bármelyik tűzszakasz befogadóképessége meghaladja az 500 főt. A tömegtartózkodásra szolgáló építmény. A talajszint feletti nagy forgalmú épület. Földfeletti közműépítmény. Tudományos, történelmi és művészeti értékű épület (szobrok, emlékművek.)",
        ["R3"] = "Tűzveszélyes épület, építmény. C tűzveszélyes osztályú anyagokat előállítanak, feldolgoznak, használnak, tárolnak vagy forgalomba hoznak. A Villamos TvMI alapján „Korlátozott mértékű robbanásveszéllyel” rendelkező épületek, valamint olyan épületek, amelyekben „mérsékelten tűzveszélyes” tűzveszélyességi osztályú anyagokat állítanak elő, dolgoznak fel, használnak.",
        ["R4"] = "Tűz és robbanásveszélyes épület, építmény. B tűzveszélyes osztályú anyagokat előállítanak, feldolgoznak, használnak, tárolnak vagy forgalomba hoznak. A Villamos TvMI alapján „Korlátozott mértékű\r\nrobbanásveszéllyel” rendelkező épületek, valamint olyan épületek, amelyekben „mérsékelten tűzveszélyes” tűzveszélyességi osztályú anyagokat állítanak elő, dolgoznak fel, használnak.",
        ["R5"] = "Fokozottan tűz és robbanásveszélyes épület, építmény.",
    };

    /// <summary>Az épület magassága és környezeti hatása szerinti osztályba sorolás (M ≤ 20 m / 20-35 m / M &gt; 35 m, valamint a becsapási veszélyt csökkentő/fokozó környezet figyelembevételével).</summary>
    public static readonly Dictionary<string, string> Magassag = new()
    {
        ["M1"] = "Becsapási veszélyt csökkentő környezet és M ≤ 20 m",
        ["M2"] = "Nincs környezeti hatás és M ≤ 20 m / Becsapási veszélyt csökkentő környezet és 20 m < M ≤ 35 m",
        ["M3"] = "Nincs környezeti hatás és 20 m < M ≤ 35 m / Becsapási veszélyt csökkentő környezet és M > 35 m / Becsapási veszélyt fokozó környezet és M ≤ 20 m",
        ["M4"] = "Nincs környezeti hatás és M > 35 m / Becsapási veszélyt fokozó környezet és 20 m < M ≤ 35 m vagy M > 35 m",
    };

    /// <summary>A tetőszerkezet anyaga szerinti besorolás (tűz-/villamos szempontból).
    /// A tetőfödém és a tetőszerkezet anyagát leíró pont (T.a / T.b / T.c) és a pont szerinti
    /// tetőfelületű tető besorolása (T.I. / T.II. / T.III. / T.IV.) alapján meghatározott,
    /// a villámvédelmi kockázatelemzéshez használt T1-T5 besorolás.
    /// </summary>
    public static readonly Dictionary<string, string> TetoAnyaga = new()
    {
        ["T1"] = "T.a pont szerinti tetőfödém / tetőszerkezet anyaga, T.I. pont szerinti tetőfelületű tető esetén",
        ["T2"] = "T.a pont szerinti tetőfödém / tetőszerkezet anyaga és T.II. pont szerinti tetőfelület,\r\nvagy T.b pont szerinti tetőfödém/tetőszerkezet anyaga és T.I. vagy T.II. pont szerinti tetőfelület,\r\nvagy T.c pont szerinti tetőfödém / tetőszerkezet anyaga és T.II. pont szerinti tetőfelület esetén",
        ["T3"] = "T.c pont szerinti tetőfödém / tetőszerkezet anyaga, T.I. pont szerinti tetőfelületű tető esetén",
        ["T4"] = "T.a pont szerinti tetőfödém / tetőszerkezet anyaga és T.III. pont szerinti tetőfelület,\r\nvagy T.c pont szerinti tetőfödém / tetőszerkezet anyaga és T.III. pont szerinti tetőfelület esetén",
        ["T5"] = "T.a pont szerinti tetőfödém / tetőszerkezet anyaga és T.IV. pont szerinti tetőfelület,\r\nvagy T.b pont szerinti tetőfödém / tetőszerkezet anyaga és T.III. vagy T.IV. pont szerinti tetőfelület,\r\nvagy T.c pont szerinti tetőfödém / tetőszerkezet anyaga és T.IV. pont szerinti tetőfelület esetén",
    };

    /// <summary>A körítő (homlokzati) falak anyaga szerinti besorolás.</summary>
    public static readonly Dictionary<string, string> KoritoFalakAnyaga = new()
    {
        ["K1"] = "Nem éghető fal",
        ["K2"] = "Fémszerkezetű fal. Az az összefüggően legalább 0.5 mm2 vastag fémlemezzel burkolt körítőfal ami más anyagokból is áll. Acélbetétes vasbetonfal.",
        ["K3"] = "Éghető fal.",
    };

    /// <summary>A környező levegő szennyezettsége (korróziós/ipari terhelés) szerinti besorolás.</summary>
    public static readonly Dictionary<string, string> LevegoSzennyezettsege = new()
    {
        ["S1"] = "Tiszta levegőben lévő épület, építmény.(Falvak)",
        ["S2"] = "Mérsékelten szennyezett levegőben lévő épület, építmény. (Városok)",
        ["S3"] = "Erősen szennyezett légkörben álló épület, építmény. (Vegyi üzemek környékén)",
        ["S4"] = "Erősen agresszív légkörben álló épület, építmény.",
    };

    /// <summary>
    /// Másodlagos hatások elleni védelem fokozata (= belső villámvédelem = túlfeszültség-levezetés).
    /// Az NNV-2020 6. táblázata szerint a rendeltetés szerinti csoport (R1-R4) alapján meghatározott
    /// villámvédelmi potenciálkiegyenlítés fokozata (B0/B2/B3/B4); R2-R4 esetén ezen felül "e" jelű
    /// koordinált túlfeszültségvédelem is szükséges. Az adott körülmények figyelembevételével a
    /// táblázatban meghatározott fokozatnál magasabb fokozat is megállapítható.
    /// </summary>
    /// <summary>A felfogó rendszer lehetséges fokozatai (dropdown-hoz).</summary>
    public static readonly string[] FelfogoFokozatok =
    {
        "V0o", "V1o", "V2a", "V2b", "V2c", "V3a", "V3b", "V3c",
        "V4a", "V4b", "V4c", "V5b", "V5c", "V6c",
    };

    /// <summary>A levezető rendszer lehetséges fokozatai (dropdown-hoz).</summary>
    public static readonly string[] LevezetoFokozatok =
    {
        "L0o", "L1o", "L2a", "L2b", "L3a", "L3b",
        "L4a", "L4b", "L4c", "L5a", "L5b", "L5c", "L6b",
    };

    /// <summary>A földelés lehetséges fokozatai (dropdown-hoz).</summary>
    public static readonly string[] FoldelesFokozatok =
    {
        "F0/x", "F1/x", "F1/r", "F2/x", "F2/r", "F3/r", "F4/r",
    };

    /// <summary>A belső villámvédelem (másodlagos hatások elleni védelem) lehetséges fokozatai (dropdown-hoz).</summary>
    public static readonly string[] BelsoVedelemFokozatok =
    {
        "B0", "B2", "B3", "B4",
    };

    public static readonly Dictionary<string, string> MasodlagosHatas = new()
    {
        ["B0"] = "R1 (nincs külső villámvédelem) esetén: villámvédelmi potenciálkiegyenlítés fokozata B0, koordinált túlfeszültségvédelem nem szükséges. Megjegyzés: az MSZ HD 60364-4-443, MSZ HD 60364-5-534 szabványok ebben az esetben is írhatnak elő potenciálkiegyenlítési intézkedéseket.",
        ["B2"] = "R1 (van külső villámvédelem) esetén: villámvédelmi potenciálkiegyenlítés fokozata B2, koordinált túlfeszültségvédelem nem szükséges. / R2 esetén: villámvédelmi potenciálkiegyenlítés fokozata B2, és koordinált túlfeszültségvédelem (e) is szükséges.",
        ["B3"] = "R3 esetén: villámvédelmi potenciálkiegyenlítés fokozata B3, és koordinált túlfeszültségvédelem (e) is szükséges.",
        ["B4"] = "R4 esetén: villámvédelmi potenciálkiegyenlítés fokozata B4, és koordinált túlfeszültségvédelem (e) is szükséges.",
    };

    /// <summary>Visszaadja az adott szempont kódjaihoz tartozó szótárat a SzempontKod alapján.</summary>
    public static Dictionary<string, string> GetKodok(string szempontKod) => szempontKod switch
    {
        "Rendeltetes" => Rendeltetes,
        "Magassag" => Magassag,
        "TetoAnyaga" => TetoAnyaga,
        "KoritoFalakAnyaga" => KoritoFalakAnyaga,
        "LevegoSzennyezettsege" => LevegoSzennyezettsege,
        "MasodlagosHatas" => MasodlagosHatas,
        _ => new Dictionary<string, string>(),
    };

    /// <summary>Az alapértelmezett 6 soros besorolási táblázat (a 2.1 fejezethez), üres besorolásokkal.</summary>
    public static List<VillamBesorolasSor> AlapertelmezettSorok() => new()
    {        new VillamBesorolasSor { SzempontKod = "Rendeltetes", Megnevezes = "Rendeltetés" },
        new VillamBesorolasSor { SzempontKod = "Magassag", Megnevezes = "Magasság" },
        new VillamBesorolasSor { SzempontKod = "TetoAnyaga", Megnevezes = "Tető anyaga" },
        new VillamBesorolasSor { SzempontKod = "KoritoFalakAnyaga", Megnevezes = "Körítő falak anyaga" },
        new VillamBesorolasSor { SzempontKod = "LevegoSzennyezettsege", Megnevezes = "A környező levegő szennyezettsége" },
        new VillamBesorolasSor { SzempontKod = "MasodlagosHatas", Megnevezes = "Másodlagos hatás" },
    };

    /// <summary>Az alapértelmezett szemrevételezéssel vizsgált tételek listája (2.2).</summary>
    public static List<SzemrevetelezesTetel> AlapertelmezettSzemrevetelezesTetelek() => new()
    {
        new SzemrevetelezesTetel { Megnevezes = "A terület alapterülete, és kerülete" },
        new SzemrevetelezesTetel { Megnevezes = "A felfogó berendezés fokozata" },
        new SzemrevetelezesTetel { Megnevezes = "Látható földelési kötések" },
        new SzemrevetelezesTetel { Megnevezes = "Egyéb szabványok betartása" },
        new SzemrevetelezesTetel { Megnevezes = "(fémtárgyak bekötése, méretfokozat)" },
    };
}

/// <summary>
/// NNV-2020 - 4. táblázat: "A felfogó fokozata" - a védendő építmény rendeltetése (R1-R5), magassága
/// (M1-M4) és a tető anyaga és szerkezete (T1-T5) szerinti csoportosítás alapján meghatározott,
/// a felfogórendszerrel szemben elvárt minimális fokozat (pl. "V2a").
/// A fokozat két részből áll: egy szám (a fokozat nagysága, minél nagyobb annál szigorúbb elvárás)
/// és egy betű (az építményhez viszonyított elhelyezés: "o" &lt; "a" &lt; "b" &lt; "c", magasabb betű helyett
/// mindig használható - a leírás szerint "o", "a-c" fokozat helyett bármelyik magasabb fokozat felhasználható).
/// FONTOS: a "V1o" fokozat a T2 oszlopban csak jelzésértékű - helyette az építmény "R" és "M" csoportjának
/// megfelelő, de a "T3-T5" csoportnak megfelelő, magasabb fokozatú felfogórendszer használható (ezért ilyenkor
/// az elvárt minimum a T3 oszlop értéke).
/// </summary>
public static class FelfogoFokozatTablazat
{
    private static readonly string[] RGroups = { "R1", "R2", "R3", "R4", "R5" };
    private static readonly string[] MGroups = { "M1", "M2", "M3", "M4" };
    private static readonly string[] TGroups = { "T1", "T2", "T3", "T4", "T5" };

    /// <summary>Sorok: R1-R5 × M1-M4 (ebben a sorrendben), oszlopok: T1-T5.</summary>
    private static readonly string[,] Matrix =
    {
        { "V0o", "V0o", "V0o", "V0o", "V0o" }, // R1-M1
        { "V0o", "V1o", "V0o", "V0o", "V2c" }, // R1-M2
        { "V2a", "V1o", "V2b", "V2c", "V2c" }, // R1-M3
        { "V3a", "V1o", "V3b", "V3c", "V3c" }, // R1-M4
        { "V0o", "V1o", "V0o", "V2c", "V2c" }, // R2-M1
        { "V2a", "V1o", "V2b", "V2c", "V3c" }, // R2-M2
        { "V3a", "V1o", "V3b", "V3c", "V3c" }, // R2-M3
        { "V3a", "V1o", "V3b", "V3c", "V3c" }, // R2-M4
        { "V3a", "V1o", "V3b", "V3c", "V3c" }, // R3-M1
        { "V3a", "V1o", "V3b", "V3c", "V4c" }, // R3-M2
        { "V3a", "V1o", "V4b", "V4c", "V4c" }, // R3-M3
        { "V4a", "V1o", "V4b", "V4c", "V4c" }, // R3-M4
        { "V4b", "V1o", "V4c", "V4c", "V4c" }, // R4-M1
        { "V4b", "V1o", "V4c", "V5c", "V5c" }, // R4-M2
        { "V5b", "V1o", "V5c", "V5c", "V5c" }, // R4-M3
        { "V5b", "V1o", "V5c", "V5c", "V6c" }, // R4-M4
        { "V4b", "V1o", "V4c", "V5c", "V5c" }, // R5-M1
        { "V4b", "V1o", "V5c", "V5c", "V6c" }, // R5-M2
        { "V5b", "V1o", "V5c", "V6c", "V6c" }, // R5-M3
        { "V5b", "V1o", "V5c", "V6c", "V6c" }, // R5-M4
    };

    /// <summary>Visszaadja a táblázat szerint közvetlenül előírt kódot (R/M/T alapján), vagy null-t, ha a bemenet nem azonosítható.</summary>
    private static string? NyersElvartKod(string r, string m, string t)
    {
        var rIdx = Array.IndexOf(RGroups, r?.Trim().ToUpperInvariant());
        var mIdx = Array.IndexOf(MGroups, m?.Trim().ToUpperInvariant());
        var tIdx = Array.IndexOf(TGroups, t?.Trim().ToUpperInvariant());
        if (rIdx < 0 || mIdx < 0 || tIdx < 0)
            return null;

        var sor = rIdx * MGroups.Length + mIdx;
        return Matrix[sor, tIdx];
    }

    /// <summary>
    /// Visszaadja az R/M/T besorolás alapján ténylegesen elvárt minimális felfogó fokozatot.
    /// Ha a táblázat szerinti nyers érték "V1o" (ez mindig a T2 oszlopban fordul elő), akkor - mivel ez
    /// a fokozat a gyakorlatban nem alkalmazható közvetlenül - helyette a T3 oszlop szerinti (a T3-T5
    /// csoportnak megfelelő fokozatok közül a legalacsonyabb) értéket adja vissza elvárt minimumként.
    /// </summary>
    public static string? ElvartFokozat(string r, string m, string t)
    {
        var nyers = NyersElvartKod(r, m, t);
        if (nyers == null)
            return null;

        if (string.Equals(nyers, "V1o", StringComparison.OrdinalIgnoreCase))
        {
            // A T2 oszlop helyett a T3 oszlop (a T3-T5 csoportnak megfelelő legalacsonyabb fokozat) az irányadó.
            return NyersElvartKod(r, m, "T3");
        }

        return nyers;
    }

    /// <summary>Egy fokozatkód (pl. "V2a") feldarabolása (szám, betű rangja). Null, ha nem értelmezhető.</summary>
    public static (int Szam, int BetuRang)? ParseFokozat(string? kod)
    {
        if (string.IsNullOrWhiteSpace(kod))
            return null;

        var match = System.Text.RegularExpressions.Regex.Match(
            kod.Trim(), @"^V\s*(\d+)\s*/?\s*([oabc])$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        var szam = int.Parse(match.Groups[1].Value);
        var betuRang = match.Groups[2].Value.ToLowerInvariant() switch
        {
            "o" => 0,
            "a" => 1,
            "b" => 2,
            "c" => 3,
            _ => -1,
        };

        return betuRang < 0 ? null : (szam, betuRang);
    }

    /// <summary>
    /// Megállapítja, hogy a helyszínen talált/beírt felfogó fokozat megfelel-e az R/M/T besorolás alapján
    /// elvárt minimumnak. A szám nagysága szigorúan, a betű ("o" &lt; "a" &lt; "b" &lt; "c") pedig a "magasabb is jó"
    /// elv szerint kerül összevetésre. Null-t ad vissza, ha az elvárt vagy a talált fokozat nem értelmezhető
    /// (ilyenkor a felhasználónak kell manuálisan megállapítania a megfelelőséget).
    /// </summary>
    public static bool? Megfelel(string? elvartKod, string? tenylegesFokozat)
    {
        var elvart = ParseFokozat(elvartKod);
        var tenyleges = ParseFokozat(tenylegesFokozat);
        if (elvart is null || tenyleges is null)
            return null;

        if (tenyleges.Value.Szam != elvart.Value.Szam)
            return tenyleges.Value.Szam > elvart.Value.Szam;

        return tenyleges.Value.BetuRang >= elvart.Value.BetuRang;
    }
}

/// <summary>
/// NNV-2020 - a levezető fokozatát a védendő építmény rendeltetése (R1-R5), magassága (M1-M4), továbbá a
/// körítőfalak anyaga (K1-K3) szerinti csoportosítás alapján, a földelés fokozatát pedig a rendeltetés
/// (R1-R5) és magasság (M1-M4) szerinti csoportosítás alapján kell megállapítani. Egy R/M csoporthoz több
/// alternatív (L/F) sor is tartozhat - ezek mindegyike elfogadható, tájékoztató jelleggel jelennek meg.
/// A földelés fokozatánál az "x" jelzés azt jelenti, hogy a földelési ellenállás értékére nincs követelmény,
/// az "r" jelzés pedig azt, hogy az ellenállásnak méréssel igazolt, szigorúbb követelményt kell kielégítenie
/// (R ≤ 2 Ω, vagy a talaj fajlagos ellenállásától/az építmény alapterületétől függő képlet szerinti érték).
/// </summary>
public static class LevezetoFoldelesFokozatTablazat
{
    /// <summary>Egy lehetséges (alternatív) levezető/földelés fokozat-kombináció egy adott R-M csoporthoz.</summary>
    public record Sor(string R, string M, string? K1, string? K2, string? K3, string? Foldeles1, string? Foldeles2 = null);

    /// <summary>
    /// Az NNV-2020 táblázatához tartozó kiegészítő szabályok, amelyeket a helyszíni megfelelőség megítélésekor
    /// figyelembe kell venni. Ezek tájékoztató jellegűek, a rendszer nem automatizálja ezek alkalmazását.
    /// </summary>
    public static readonly IReadOnlyList<string> Megjegyzesek = new List<string>
    {
        "Az „L2–L4” fokozat helyett a levezetők elhelyezésének rendszere mindig lehet magasabb fokozatú is.",
        "Az „L1o” fokozatú levezetők helyett csak az építmény „R” és „M” csoportjának megfelelő fokozatú, de az eredeti „K2” helyett a „K1” vagy „K3” csoportnak megfelelő levezetőrendszer használható.",
        "A levezető építményhez viszonyított helyzete szempontjából szükséges „o”, „a–c” fokozat helyett bármelyik magasabb fokozat is használható.",
        "„L0o” fokozat csak abban az esetben állapítható meg, ha a felfogó fokozata „V0o”, tehát villámvédelem nincs.",
        "V3 fokozatú felfogórendszerhez kizárólag L3 fokozatú levezetőrendszer tartozhat.",
        "A földelés fokozatának megállapításakor figyelembe kell venni a levezető megállapított fokozatát is, ezért a földelés fokozata csak a levezető fokozatával azonos sorban lévő változatok közül választható.",
        "Az „F2” és „F3” fokozatú földelőrendszer helyett mindig lehet magasabb fokozatú földelőrendszert is használni.",
        "Az „F1/x” vagy az „F1/r” fokozatnak megfelelő természetes földelő és a vagylagosan megadott más fokozatú mesterséges földelő minden esetben helyettesítheti egymást.",
        "Ha a másodlagos hatás elleni védelem fokozata „B2”, „B3” vagy „B4”, akkor a földelés „F0/x”, „F1/x” vagy „F2/x” fokozata nem használható, hanem helyettük legalább „r” fokozatú földelőrendszert kell alkalmazni.",
        "Az építményhez viszonyított helyzet szempontjából az „a–c” fokozatú villámvédelmet mindig helyettesítheti a „d” fokozatú építménytől elszigetelt, vagy építménytől független villámvédelmi rendszer is.",
        "Az építménytől elszigetelt, vagy építménytől független villámvédelem esetén alacsonyabb fokozatú levezető és földelő is alkalmazható. A földelési rendszernek „r” fokozatúnak kell lennie.",
    };

    private static readonly List<Sor> Sorok = new()
    {
        new("R1", "M1", "L0o", "L0o", "L0o", "F0/x"),
        new("R1", "M2", "L0o", "L0o", "L0o", "F0/x"),
        new("R1", "M2", "L2a", null, "L2b", "F2/x", "F1/x"),
        new("R1", "M2", "L3a", "L1o", "L3b", "F3/r", "F1/x"),
        new("R1", "M3", "L2a", null, "L2b", "F2/x", "F1/x"),
        new("R1", "M3", "L3a", "L1o", "L3b", "F3/r", "F1/r"),
        new("R1", "M4", "L4a", "L1o", "L4b", "F3/r", "F1/r"),
        new("R2", "M1", "L0o", "L0o", "L0o", "F0/x"),
        new("R2", "M1", "L2a", null, "L2b", "F2/x", "F1/x"),
        new("R2", "M1", "L3a", "L1o", "L3b", "F3/r", "F1/x"),
        new("R2", "M2", "L2a", null, "L2b", "F2/x", "F1/x"),
        new("R2", "M2", "L3a", "L1o", "L3b", "F3/r", "F1/r"),
        new("R2", "M3", "L4a", "L1o", "L4b", "F3/r", "F1/r"),
        new("R2", "M4", "L5a", "L1o", "L5b", "F3/r", "F1/r"),
        new("R3", "M1", "L3a", "L1o", "L3b", "F3/r", "F1/r"),
        new("R3", "M2", "L3a", "L1o", "L4b", "F3/r", "F1/r"),
        new("R3", "M3", "L4a", "L1o", "L5b", "F4/r", "F1/r"),
        new("R3", "M4", "L5a", "L1o", "L6b", "F4/r", "F1/r"),
        new("R4", "M1", "L4b", "L1o", "L4b", "F4/r"),
        new("R4", "M2", "L4b", "L1o", "L4b", "F4/r"),
        new("R4", "M3", "L5b", "L1o", "L5b", "F4/r"),
        new("R4", "M4", "L5b", "L1o", "L5b", "F4/r"),
        new("R5", "M1", "L4b", "L1o", "L4c", "F4/r"),
        new("R5", "M2", "L4b", "L1o", "L4c", "F4/r"),
        new("R5", "M3", "L5b", "L1o", "L5c", "F4/r"),
        new("R5", "M4", "L5b", "L1o", "L5c", "F4/r"),
    };

    /// <summary>Visszaadja az adott R/M csoporthoz tartozó összes alternatív sort (tájékoztató megjelenítéshez).</summary>
    public static List<Sor> GetAlternativak(string? r, string? m)
    {
        if (string.IsNullOrWhiteSpace(r) || string.IsNullOrWhiteSpace(m))
            return new List<Sor>();

        return Sorok
            .Where(s => string.Equals(s.R, r.Trim(), StringComparison.OrdinalIgnoreCase)
                        && string.Equals(s.M, m.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>Egy levezető fokozatkód (pl. "L2b") feldarabolása (szám, betű rangja). Null, ha nem értelmezhető.</summary>
    private static (int Szam, int BetuRang)? ParseLevezetoFokozat(string? kod)
    {
        if (string.IsNullOrWhiteSpace(kod))
            return null;

        var match = System.Text.RegularExpressions.Regex.Match(
            kod.Trim(), @"^L\s*(\d+)\s*/?\s*([oabc])$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        var szam = int.Parse(match.Groups[1].Value);
        var betuRang = match.Groups[2].Value.ToLowerInvariant() switch
        {
            "o" => 0,
            "a" => 1,
            "b" => 2,
            "c" => 3,
            _ => -1,
        };

        return betuRang < 0 ? null : (szam, betuRang);
    }

    /// <summary>
    /// Megállapítja, hogy a helyszínen talált/beírt levezető fokozat megfelel-e a 2.1 pontban megadott R/M
    /// besorolás alapján elvárt (bármelyik K1/K2/K3 oszlop szerinti alternatívának megfelelő) minimumnak.
    /// A szám nagysága szigorúan, a betű ("o" &lt; "a" &lt; "b" &lt; "c") pedig a "magasabb is jó" elv szerint
    /// kerül összevetésre. Null-t ad vissza, ha az R/M besorolás vagy a talált fokozat nem értelmezhető
    /// (ilyenkor a felhasználónak kell manuálisan megállapítania a megfelelőséget).
    /// FONTOS: a K2 oszlopban szereplő "L1o" fokozat önmagában nem valódi, használható minimum - a kiegészítő
    /// szabályok szerint helyette az adott sor K1 vagy K3 oszlopának megfelelő (magasabb) fokozatú levezetőrendszer
    /// használható, ezért ez a kiértékelés nem veszi figyelembe önálló elvárt célértékként.
    /// </summary>
    public static bool? Megfelel(string? r, string? m, string? tenylegesFokozat)
    {
        var tenyleges = ParseLevezetoFokozat(tenylegesFokozat);
        if (tenyleges is null)
            return null;

        var alternativak = GetAlternativak(r, m);
        if (!alternativak.Any())
            return null;

        var elvartKodok = alternativak
            .SelectMany(a => new[] { a.K1, a.K3 })
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(ParseLevezetoFokozat)
            .Where(p => p is not null)
            .Select(p => p!.Value)
            .ToList();

        if (!elvartKodok.Any())
            return null;

        return elvartKodok.Any(elvart =>
            tenyleges.Value.Szam != elvart.Szam
                ? tenyleges.Value.Szam > elvart.Szam
                : tenyleges.Value.BetuRang >= elvart.BetuRang);
    }
}
