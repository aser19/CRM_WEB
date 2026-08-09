namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy Rb (robbanásbiztos) berendezés sora az Excel importból / kézi bevitelből,
/// kiegészítve az "Egyedi felülvizsgálati lap" checklist eredményeivel.
/// Egy sor = egy legenerálandó PDF oldal.
/// </summary>
public class RbSor
{
    // === Excel import mezők (2. képen látható táblázat oszlopai) ===
    public int Sorsz { get; set; }
    public string Elhelyezes { get; set; } = "";
    public string Megnevezes { get; set; } = "";
    public string AramkoriJel { get; set; } = "";
    public string Gyarto { get; set; } = "";
    public string Tipus { get; set; } = "";
    private string _gyariSzam = "";
    /// <summary>A berendezés gyári száma. Beállításakor a korábbi duplikáció-nyugtázás érvényét veszti.</summary>
    public string GyariSzam
    {
        get => _gyariSzam;
        set
        {
            if (_gyariSzam == value) return;
            _gyariSzam = value;
            GyariSzamDuplikaciotElfogadta = false;
        }
    }
    public string IpVedelem { get; set; } = "";

    /// <summary>
    /// Igaz, ha a felhasználó nyugtázta, hogy a gyári szám valóban egyezik egy másik sorral
    /// (pl. tudatosan ugyanazt a berendezést rögzítette kétszer), így a duplikáció-jelzés elrejthető.
    /// </summary>
    public bool GyariSzamDuplikaciotElfogadta { get; set; }

    private string _vedelmiMod = "";
    /// <summary>A Rb védelmi mód jele. Beállításakor a hozzá kapcsolódó (regex-elemzésből származó) gyorsítótárazott mezők érvénytelenítődnek.</summary>
    public string VedelmiMod
    {
        get => _vedelmiMod;
        set
        {
            if (_vedelmiMod == value) return;
            _vedelmiMod = value;
            ErvenytelenitiVedelmiModCache();
        }
    }

    public string EngSzam { get; set; } = "";
    public string Minositas { get; set; } = "megfelelő";


    // === "Egyedi felülvizsgálati lap" fejléc-specifikus mezők (1. kép) ===
    public string Tervjel { get; set; } = "";
    public string Objektum { get; set; } = "";
    public string CimkeSorszam { get; set; } = "";

    public string TuzveszOsztaly { get; set; } = "";

    private string _zonaBesorolas = "";
    /// <summary>A Zóna besorolás szövege. Beállításakor a gyorsítótárazott zóna-megfelelőség érvénytelenítődik.</summary>
    public string ZonaBesorolas
    {
        get => _zonaBesorolas;
        set
        {
            if (_zonaBesorolas == value) return;
            _zonaBesorolas = value;
            _zonaMegfeleloCache = null;
            _zonaMegfeleloSzamitva = false;
        }
    }
    public string AlkalmazasiCsoport { get; set; } = "";
    public string HomersOsztaly { get; set; } = "";

    /// <summary>Az év mód meglétele (szemrevételezéssel).</summary>
    public bool EvModMeglete { get; set; } = true;

    // === Checklist szekciók (soronként Megfelelt/NemMegfelelt + megjegyzés) ===
    public List<RbCheckTetel> Kornyezeti { get; set; } = RbChecklistSablon.UjKornyezeti();
    public List<RbCheckTetel> KeszulekAllapota { get; set; } = RbChecklistSablon.UjKeszulekAllapota();
    public List<RbCheckTetel> ExI { get; set; } = RbChecklistSablon.UjExI();
    public List<RbCheckTetel> ExD { get; set; } = RbChecklistSablon.UjExD();
    public List<RbCheckTetel> ExM { get; set; } = RbChecklistSablon.UjExM();
    public List<RbCheckTetel> ExE { get; set; } = RbChecklistSablon.UjExE();
    public List<RbCheckTetel> ExP { get; set; } = RbChecklistSablon.UjExP();

    /// <summary>Végső minősítés ("megfelelt" / "nem felelt meg").</summary>
    public string VegsoMinosites { get; set; } = "megfelelt";

    /// <summary>A vizsgálatot végezte (alapértelmezetten a bejelentkezett felhasználó neve).</summary>
    public string VizsgalatotVegezte { get; set; } = "";

    /// <summary>
    /// A "Védelmi mód" oszlop alapján meghatározott releváns Ex védelmi módok (i, d, m, p, e),
    /// amelyek szerinti checklist szekciókat meg kell jeleníteni. Gyorsítótárazott: a regex-elemzés
    /// csak akkor fut le újra, ha a Védelmi mód szövege ténylegesen megváltozott (lásd <see cref="VedelmiMod"/> setter).
    /// </summary>
    public HashSet<char> RelevansVedelmiModok
    {
        get
        {
            _relevansVedelmiModokCache ??= RbVedelmiModHelper.MeghatarozRelevansModok(VedelmiMod);
            return _relevansVedelmiModokCache;
        }
    }
    private HashSet<char>? _relevansVedelmiModokCache;

    public bool VanExI => RelevansVedelmiModok.Contains('i');
    public bool VanExD => RelevansVedelmiModok.Contains('d');
    public bool VanExM => RelevansVedelmiModok.Contains('m');
    public bool VanExE => RelevansVedelmiModok.Contains('e');
    public bool VanExP => RelevansVedelmiModok.Contains('p');

    private RbVedelmiMod? _vedelmiModAdatok;
    /// <summary>
    /// A "Védelmi mód" szöveghez tartozó strukturált egyértelműsítő tábla bejegyzés (ha ismert / admin jóváhagyott).
    /// Ezt a hívó fél (pl. RbMeresiTabla) tölti ki a betöltött RbVedelmiMod szótár alapján; ha nincs egyezés, marad null,
    /// és a rendszer a regex-alapú tartalék elemzésre esik vissza. Beállításakor a hozzá kapcsolódó gyorsítótárak érvénytelenítődnek.
    /// </summary>
    public RbVedelmiMod? VedelmiModAdatok
    {
        get => _vedelmiModAdatok;
        set
        {
            _vedelmiModAdatok = value;
            _alkalmazasiCsoportCache = null;
            _porcsoportCache = null;
            _homersOsztalyCache = null;
            _zonaMegfeleloCache = null;
            _zonaMegfeleloSzamitva = false;
        }
    }

    /// <summary>
    /// Ha a "Védelmi mód" mező értéke gyanúsan hasonlít egy már ismert/rögzített bejegyzésre (valószínűleg
    /// elgépelés vagy eltérő zárójel/szóköz), itt tárolódik a javasolt (hasonló) érték, hogy a felhasználói
    /// felületen sorra bontva megjelölhessük, hol történt a feltételezett elírás. Null, ha nincs ilyen gyanú.
    /// </summary>
    public string? VedelmiModGyanusHasonlo { get; set; }

    /// <summary>Igaz, ha ennél a sornál a Védelmi mód értéke gyanús (lásd <see cref="VedelmiModGyanusHasonlo"/>).</summary>
    public bool VedelmiModGyanus => !string.IsNullOrEmpty(VedelmiModGyanusHasonlo);

    private string? _alkalmazasiCsoportCache;
    /// <summary>Az "Alkalmazási csoport" (gázcsoport: I, IIA, IIB, IIC) - elsődlegesen az egyértelműsítő táblából, tartalékként a Védelmi mód szövegéből kiolvasva. Gyorsítótárazott.</summary>
    public string AlkalmazasiCsoportSzamitott
    {
        get
        {
            _alkalmazasiCsoportCache ??= !string.IsNullOrWhiteSpace(VedelmiModAdatok?.Gazcsoport)
                ? VedelmiModAdatok!.Gazcsoport!
                : RbVedelmiModHelper.MeghatarozGazcsoport(VedelmiMod);
            return _alkalmazasiCsoportCache;
        }
    }

    private string? _porcsoportCache;
    /// <summary>A "Porcsoport" (IIIA, IIIB, IIIC) - elsődlegesen az egyértelműsítő táblából, tartalékként a Védelmi mód szövegéből kiolvasva. Gyorsítótárazott.</summary>
    public string PorcsoportSzamitott
    {
        get
        {
            _porcsoportCache ??= !string.IsNullOrWhiteSpace(VedelmiModAdatok?.Porcsoport)
                ? VedelmiModAdatok!.Porcsoport!
                : RbVedelmiModHelper.MeghatarozPorcsoport(VedelmiMod);
            return _porcsoportCache;
        }
    }

    private string? _homersOsztalyCache;
    /// <summary>A "Hőmérséklet osztály" (T1-T6 vagy °C érték) - elsődlegesen az egyértelműsítő táblából, tartalékként a Védelmi mód szövegéből kiolvasva. Gyorsítótárazott.</summary>
    public string HomersOsztalySzamitott
    {
        get
        {
            _homersOsztalyCache ??= !string.IsNullOrWhiteSpace(VedelmiModAdatok?.HomersOsztaly)
                ? VedelmiModAdatok!.HomersOsztaly!
                : RbVedelmiModHelper.MeghatarozHomersOsztaly(VedelmiMod);
            return _homersOsztalyCache;
        }
    }

    private bool? _zonaMegfeleloCache;
    private bool _zonaMegfeleloSzamitva;
    /// <summary>
    /// Megvizsgálja, hogy a berendezés Védelmi módja megfelel-e a (helyiségre megadott) Zóna besorolásnak.
    /// Ha az egyértelműsítő táblában van engedélyezett zóna lista megadva, azt veszi elsődlegesen figyelembe;
    /// egyébként az RbZonaMegfeleltetesTablazat alapján, regex-elemzéssel dönt. Null, ha nincs elég adat az összehasonlításhoz.
    /// Gyorsítótárazott: csak akkor számol újra, ha a ZonaBesorolas, VedelmiMod vagy VedelmiModAdatok ténylegesen megváltozott.
    /// </summary>
    public bool? ZonaMegfelelo
    {
        get
        {
            if (_zonaMegfeleloSzamitva) return _zonaMegfeleloCache;

            var zona = RbZonaMegfeleltetesTablazat.NormalizalZona(ZonaBesorolas);
            bool? eredmeny;
            if (string.IsNullOrEmpty(zona))
            {
                eredmeny = null;
            }
            else if (VedelmiModAdatok?.EngedelyezettZonakLista is { Count: > 0 } engedelyezettZonak)
            {
                eredmeny = engedelyezettZonak.Contains(zona);
            }
            else
            {
                eredmeny = RbZonaMegfeleltetesTablazat.Megfelel(ZonaBesorolas, VedelmiMod);
            }

            _zonaMegfeleloCache = eredmeny;
            _zonaMegfeleloSzamitva = true;
            return eredmeny;
        }
    }

    /// <summary>
    /// A sor kitöltéséhez kötelezőnek számító mezők neveinek listája, amelyek jelenleg üresek/hiányoznak.
    /// Üres lista, ha minden kötelező adat ki van töltve.
    /// </summary>
    public List<string> HianyzoKotelezoMezok
    {
        get
        {
            var hianyzok = new List<string>();

            if (string.IsNullOrWhiteSpace(Elhelyezes)) hianyzok.Add("Elhelyezés");
            if (string.IsNullOrWhiteSpace(Megnevezes)) hianyzok.Add("Eszköz neve (Megnevezés)");
            if (string.IsNullOrWhiteSpace(Gyarto)) hianyzok.Add("Gyártó");
            if (string.IsNullOrWhiteSpace(Tipus)) hianyzok.Add("Típus");
            if (string.IsNullOrWhiteSpace(GyariSzam)) hianyzok.Add("Gyári szám");
            if (string.IsNullOrWhiteSpace(VedelmiMod)) hianyzok.Add("Védelmi mód");
            if (string.IsNullOrWhiteSpace(EngSzam)) hianyzok.Add("Eng. szám (vizsgáló állomás)");
            if (string.IsNullOrWhiteSpace(TuzveszOsztaly)) hianyzok.Add("Tűzvesz. osztály");
            if (string.IsNullOrWhiteSpace(ZonaBesorolas)) hianyzok.Add("Zóna besorolás");
            if (string.IsNullOrWhiteSpace(VizsgalatotVegezte)) hianyzok.Add("A vizsgálatot végezte (felülvizsgáló)");

            return hianyzok;
        }
    }

    /// <summary>Igaz, ha a sornál legalább egy kötelező mező hiányzik.</summary>
    public bool VanHianyzoAdat => HianyzoKotelezoMezok.Count > 0;

    /// <summary>Érvényteleníti a Védelmi mód szövegétől függő összes gyorsítótárazott (regex-elemzésből származó) mezőt.</summary>
    private void ErvenytelenitiVedelmiModCache()
    {
        _relevansVedelmiModokCache = null;
        _alkalmazasiCsoportCache = null;
        _porcsoportCache = null;
        _homersOsztalyCache = null;
        _zonaMegfeleloCache = null;
        _zonaMegfeleloSzamitva = false;
    }
}


/// <summary>Egy checklist tétel: szöveg + megfelelt jelölés + megjegyzés.</summary>
public class RbCheckTetel
{
    public string Szoveg { get; set; } = "";
    public bool Megfelelt { get; set; } = true;
    public string? Megjegyzes { get; set; }
}

/// <summary>Segéd az Rb "Védelmi mód" szöveg alapján a releváns Ex kategóriák (i/d/m/p/e) megállapítására.</summary>
public static class RbVedelmiModHelper
{
    private static readonly System.Text.RegularExpressions.Regex ModKodMinta = new(
        @"(?:(?i:Ex|Eex|EEx))\s+([a-z]{1,3})\b",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Gázcsoport minta: IIC, IIB, IIA vagy önálló I (bányászati). A hosszabb egyezést (IIC/IIB/IIA) kell előnyben részesíteni.</summary>
    private static readonly System.Text.RegularExpressions.Regex GazcsoportMinta = new(
        @"\bII[ABC]\b|\bI\b",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Porcsoport minta: IIIC, IIIB, IIIA.</summary>
    private static readonly System.Text.RegularExpressions.Regex PorcsoportMinta = new(
        @"\bIII[ABC]\b",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Hőmérséklet osztály minta: T1-T6, vagy közvetlen °C érték (pl. T80°C).</summary>
    private static readonly System.Text.RegularExpressions.Regex HomersOsztalyMinta = new(
        @"\bT[1-6]\b|\bT\d{2,3}\s?°?C\b",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

    public static HashSet<char> MeghatarozRelevansModok(string? vedelmiMod)
    {
        var eredmeny = new HashSet<char>();
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return eredmeny;

        foreach (System.Text.RegularExpressions.Match match in ModKodMinta.Matches(vedelmiMod))
        {
            var kod = match.Groups[1].Value.ToLowerInvariant();
            foreach (var betu in kod)
            {
                if (betu is 'i' or 'd' or 'm' or 'p' or 'e')
                    eredmeny.Add(betu);
            }
        }

        return eredmeny;
    }

    /// <summary>Kiolvassa az "Alkalmazási csoportot" (gázcsoport: I, IIA, IIB, IIC) a Védelmi mód szövegéből.</summary>
    public static string MeghatarozGazcsoport(string? vedelmiMod)
    {
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return "";
        var match = GazcsoportMinta.Match(vedelmiMod);
        return match.Success ? match.Value.ToUpperInvariant() : "";
    }

    /// <summary>Kiolvassa a "Porcsoportot" (IIIA, IIIB, IIIC) a Védelmi mód szövegéből.</summary>
    public static string MeghatarozPorcsoport(string? vedelmiMod)
    {
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return "";
        var match = PorcsoportMinta.Match(vedelmiMod);
        return match.Success ? match.Value.ToUpperInvariant() : "";
    }

    /// <summary>Kiolvassa a "Hőmérséklet osztályt" (T1-T6 vagy °C érték) a Védelmi mód szövegéből.</summary>
    public static string MeghatarozHomersOsztaly(string? vedelmiMod)
    {
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return "";
        var match = HomersOsztalyMinta.Match(vedelmiMod);
        return match.Success ? match.Value.ToUpperInvariant() : "";
    }

    /// <summary>
    /// Normalizálja a Védelmi mód szöveget összehasonlítás céljából: eltávolítja a felesleges/duplikált
    /// szóközöket és nagybetűsít. A zárójeltípus ( [ ] vs ( ) ) szándékosan NEM egységesül, mert az
    /// eltérő jelentést hordozhat (pl. Ex db [ia Ga] IIC != Ex db (ia Ga) IIC).
    /// </summary>
    public static string Normalizal(string? vedelmiMod)
    {
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return "";

        var szoveg = vedelmiMod.ToUpperInvariant();
        szoveg = System.Text.RegularExpressions.Regex.Replace(szoveg, @"\s+", " ").Trim();
        return szoveg;
    }

    /// <summary>Egyetlen szintű zárójeles szakaszt talál meg (a nyitó/záró jelet is beleértve), típusát ((...) vagy [...]) megőrizve.</summary>
    private static readonly System.Text.RegularExpressions.Regex ZarojelSzegmensMinta = new(
        @"([\(\[][^\(\)\[\]]*[\)\]])",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// A normalizált szöveget összehasonlítható alakra hozza:
    /// - a zárójeleken KÍVÜLI részeken a szavakon belüli betűk sorrendje irreleváns lesz
    ///   (pl. "IIC DE" -&gt; "CII DE", így "IIC DE" == "IIC ED" az összehasonlításnál),
    /// - a zárójeleken BELÜL a szavak (tokenek) sorrendje irreleváns, DE az egyes szavakon belüli
    ///   betűsorrend számít (pl. "(IA GG)" == "(GG IA)", de "(IA GD)" != "(AI GD)").
    /// A zárójeltípus (szögletes vs kerek) megmarad, így az továbbra is megkülönböztető marad.
    /// </summary>
    private static string OsszehasonlithatoAlakra(string normalizaltSzoveg)
    {
        if (string.IsNullOrEmpty(normalizaltSzoveg)) return "";

        var reszek = ZarojelSzegmensMinta.Split(normalizaltSzoveg);
        var eredmeny = new System.Text.StringBuilder();

        foreach (var resz in reszek)
        {
            if (resz.Length == 0) continue;

            var zarojeles = (resz[0] == '(' || resz[0] == '[') && (resz[^1] == ')' || resz[^1] == ']');
            if (zarojeles)
            {
                var nyito = resz[0];
                var zaro = resz[^1];
                var tartalom = resz[1..^1];

                var szavak = tartalom.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(szavak, StringComparer.Ordinal);

                eredmeny.Append(nyito).Append(string.Join(' ', szavak)).Append(zaro);
            }
            else
            {
                var szavak = resz.Split(' ');
                for (var i = 0; i < szavak.Length; i++)
                {
                    var szo = szavak[i];
                    if (szo.Length > 0 && szo.All(char.IsLetter))
                    {
                        var rendezett = szo.ToCharArray();
                        Array.Sort(rendezett);
                        szavak[i] = new string(rendezett);
                    }
                }

                eredmeny.Append(string.Join(' ', szavak));
            }
        }

        return eredmeny.ToString();
    }

    /// <summary>
    /// Levenshtein szerkesztési távolság két szöveg között (karakterenkénti beszúrás/törlés/csere száma).
    /// </summary>
    private static int SzerkesztesiTavolsag(string a, string b)
    {
        var elozo = new int[b.Length + 1];
        var aktualis = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
            elozo[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            aktualis[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var koltseg = a[i - 1] == b[j - 1] ? 0 : 1;
                aktualis[j] = Math.Min(
                    Math.Min(aktualis[j - 1] + 1, elozo[j] + 1),
                    elozo[j - 1] + koltseg);
            }
            (elozo, aktualis) = (aktualis, elozo);
        }

        return elozo[b.Length];
    }

    /// <summary>
    /// Megvizsgálja, hogy két Védelmi mód szöveg valószínűleg ugyanazt jelenti-e
    /// (pl. csak elgépelésben vagy egy szón belüli betűsorrendben térnek el), de nem egyeznek
    /// karakterről karakterre. A zárójeltípus ( [ ] vs ( ) ) eltérése MINDIG valódi különbségnek számít.
    /// Visszaadja true-t, ha a normalizált (és szavanként betűrendezett) szövegek azonosak, VAGY nagyon
    /// hasonlóak (kis szerkesztési távolság a hosszukhoz képest), de nem pontosan egyeznek.
    /// </summary>
    public static bool ValoszinulegUgyanaz(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;

        var normA = Normalizal(a);
        var normB = Normalizal(b);

        if (normA == normB) return true;

        // A zárójeltípus mindig megkülönböztető: ha az egyik szövegben szögletes, a másikban kerek
        // zárójel szerepel (eltérő számban), azt nem tekintjük elgépelésnek.
        if (ZarojelTipusEler(normA, normB)) return false;

        var rendezettA = OsszehasonlithatoAlakra(normA);
        var rendezettB = OsszehasonlithatoAlakra(normB);

        if (rendezettA == rendezettB) return true;

        var maxHossz = Math.Max(rendezettA.Length, rendezettB.Length);
        if (maxHossz == 0) return false;

        var tavolsag = SzerkesztesiTavolsag(rendezettA, rendezettB);
        // Rövid szövegeknél max 2, hosszabbaknál a hossz kb. 10%-a, de legfeljebb 5 karakternyi eltérés engedett.
        var kuszob = Math.Max(2, Math.Min(5, maxHossz / 10));
        return tavolsag <= kuszob;
    }

    /// <summary>Igaz, ha a két szöveg szögletes ("[","]") és kerek ("(",")") zárójeleinek száma eltér.</summary>
    private static bool ZarojelTipusEler(string a, string b)
    {
        int SzogletesSzam(string s) => s.Count(c => c is '[' or ']');
        int KerekSzam(string s) => s.Count(c => c is '(' or ')');

        return SzogletesSzam(a) != SzogletesSzam(b) || KerekSzam(a) != KerekSzam(b);
    }
}

/// <summary>
/// Egyértelműsítő táblázat a robbanásveszélyes zónák (0, 1, 2, 20, 21, 22) és az azokban
/// alkalmazható Ex védelmi módok (i, d, e, m, p, q, o, n, t) megfeleltetésére, MSZ EN 60079-14 alapján.
/// </summary>
public static class RbZonaMegfeleltetesTablazat
{
    /// <summary>Zóna -> engedélyezett védelmi mód betűk (kisbetűvel).</summary>
    public static readonly Dictionary<string, HashSet<char>> ZonaEngedelyezettModok = new(StringComparer.OrdinalIgnoreCase)
    {
        ["0"] = new HashSet<char> { 'i', 'm', 'o' },                    // ia, ma (0-ás zónában csak "a" szintű)
        ["1"] = new HashSet<char> { 'i', 'd', 'e', 'm', 'p', 'q', 'o' },
        ["2"] = new HashSet<char> { 'i', 'd', 'e', 'm', 'p', 'q', 'o', 'n' },
        ["20"] = new HashSet<char> { 'i', 'm', 't' },
        ["21"] = new HashSet<char> { 'i', 'd', 'e', 'm', 't' },
        ["22"] = new HashSet<char> { 'i', 'd', 'e', 'm', 't', 'n' },
    };

    private static readonly System.Text.RegularExpressions.Regex ModBetukMinta = new(
        @"(?:(?i:Ex|Eex|EEx))\s+([a-z]{1,3})\b",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// Kinyeri a zónaszámot egy "Zóna besorolás" szövegből (pl. "1. zóna", "Zóna 1", "21").
    /// </summary>
    public static string NormalizalZona(string? zonaBesorolas)
    {
        if (string.IsNullOrWhiteSpace(zonaBesorolas)) return "";
        var match = System.Text.RegularExpressions.Regex.Match(zonaBesorolas, @"\b(0|1|2|20|21|22)\b");
        return match.Success ? match.Value : "";
    }

    /// <summary>
    /// Megvizsgálja, hogy a megadott Védelmi mód megfelel-e a megadott Zóna besorolásnak.
    /// Null, ha a zóna vagy a védelmi mód nem állapítható meg egyértelműen.
    /// </summary>
    public static bool? Megfelel(string? zonaBesorolas, string? vedelmiMod)
    {
        var zona = NormalizalZona(zonaBesorolas);
        if (string.IsNullOrEmpty(zona) || !ZonaEngedelyezettModok.TryGetValue(zona, out var engedelyezett))
            return null;

        if (string.IsNullOrWhiteSpace(vedelmiMod)) return null;

        var talalt = new HashSet<char>();
        foreach (System.Text.RegularExpressions.Match match in ModBetukMinta.Matches(vedelmiMod))
        {
            foreach (var betu in match.Groups[1].Value.ToLowerInvariant())
                talalt.Add(betu);
        }

        if (talalt.Count == 0) return null;

        return talalt.Any(betu => engedelyezett.Contains(betu));
    }
}

/// <summary>Fix checklist sablon sorok az "Egyedi felülvizsgálati lap" mintája alapján.</summary>
public static class RbChecklistSablon
{
    public static List<RbCheckTetel> UjKornyezeti() => new()
    {
        new() { Szoveg = "Korrózióvédelem, rezgésvédelem" },
        new() { Szoveg = "A gyártmány mechanikai rögzítése" },
        new() { Szoveg = "Káros por- és szennyeződéslerakódás mentessége" },
    };

    public static List<RbCheckTetel> UjKeszulekAllapota() => new()
    {
        new() { Szoveg = "A térségbesorolásnak (zóna:alk.csop.:hőm.o.) megfelelősség" },
        new() { Szoveg = "Adattáblája, feliratai és azonosíthatósága" },
        new() { Szoveg = "A belső áramköreinek azonosítása" },
        new() { Szoveg = "Az engedélyhez képest van-e jogosulatlan módosítás" },
        new() { Szoveg = "A vill. csatlakozások minősége (sorkapcsok)" },
        new() { Szoveg = "A tömszelencék és a fel nem használt nyílások lezárása" },
        new() { Szoveg = "A bekötött vezeték(ek) húzásproba után" },
        new() { Szoveg = "Beszerelés és bekötés után is teljesül a megjelölt IP védettség" },
        new() { Szoveg = "A fel nem használt (tartalék) vez. lezárása" },
        new() { Szoveg = "A bekötött vezeték(ek) mérete, típusa, sérülésmentessége" },
    };

    public static List<RbCheckTetel> UjExI() => new()
    {
        new() { Szoveg = "Az alkalmazott szikragát földelése" },
        new() { Szoveg = "A készülék külső EPH-ba kötése" },
        new() { Szoveg = "A vill. elválasztási üresjárási feszültsége [V]" },
        new() { Szoveg = "A vill. elválasztási rövidzárási árama [mA]" },
    };

    public static List<RbCheckTetel> UjExD() => new()
    {
        new() { Szoveg = "A tokozás, az üvegrészek és ezek tömítettsége, tömítőanyagai" },
        new() { Szoveg = "A peremek csatlakozó felülete tisztasága, sértetlensége" },
        new() { Szoveg = "A tokozathoz csatlakozó kezelőszervek épsége" },
        new() { Szoveg = "A csatlakozó tömszelence befogatása, tömítése" },
        new() { Szoveg = "Az alkalmazott lámpák teljesítménye, típusa, beállítása" },
        new() { Szoveg = "Motoroknál a ventilátor és a ház közötti távolság" },
        new() { Szoveg = "Változó frekv. motor termikus védelme (engedélye alapján)" },
    };

    public static List<RbCheckTetel> UjExM() => new()
    {
        new() { Szoveg = "A gyártmány kiöntése, a kiöntés szilárdsága" },
        new() { Szoveg = "A kilépő vezeték sértetlensége, szigetelése" },
    };

    public static List<RbCheckTetel> UjExE() => new()
    {
        new() { Szoveg = "Az alkalmazott sorkapcsok beszerelése" },
        new() { Szoveg = "Az alkalmazott lámpák teljesítménye, típusa, beállítása" },
        new() { Szoveg = "A csatlakozó tömszelence befogatása, tömítése" },
        new() { Szoveg = "Változó frekv. motor termikus védelme (engedélye alapján)" },
        new() { Szoveg = "Motoroknál a ventilátor és a ház közötti távolság" },
    };

    public static List<RbCheckTetel> UjExP() => new()
    {
        new() { Szoveg = "A bemeneti védőgáz hőmérséklete" },
        new() { Szoveg = "A védőgáz nyomása és áramlása" },
        new() { Szoveg = "A riasztó és tereszelő áramkörök működése" },
        new() { Szoveg = "A fesz. alá helyezés előtti átöblítés" },
    };
}
