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
    public string GyariSzam { get; set; } = "";
    public string IpVedelem { get; set; } = "";
    public string VedelmiMod { get; set; } = "";
    public string EngSzam { get; set; } = "";
    public string Minositas { get; set; } = "megfelelő";

    // === "Egyedi felülvizsgálati lap" fejléc-specifikus mezők (1. kép) ===
    public string Tervjel { get; set; } = "";
    public string Objektum { get; set; } = "";
    public string CimkeSorszam { get; set; } = "";

    public string TuzveszOsztaly { get; set; } = "";
    public string ZonaBesorolas { get; set; } = "";
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
    /// amelyek szerinti checklist szekciókat meg kell jeleníteni.
    /// </summary>
    public HashSet<char> RelevansVedelmiModok => RbVedelmiModHelper.MeghatarozRelevansModok(VedelmiMod);

    public bool VanExI => RelevansVedelmiModok.Contains('i');
    public bool VanExD => RelevansVedelmiModok.Contains('d');
    public bool VanExM => RelevansVedelmiModok.Contains('m');
    public bool VanExE => RelevansVedelmiModok.Contains('e');
    public bool VanExP => RelevansVedelmiModok.Contains('p');

    /// <summary>Az "Alkalmazási csoport" (gázcsoport: I, IIA, IIB, IIC) a Védelmi mód szövegéből kiolvasva.</summary>
    public string AlkalmazasiCsoportSzamitott => RbVedelmiModHelper.MeghatarozGazcsoport(VedelmiMod);

    /// <summary>A "Hőmérséklet osztály" (T1-T6) a Védelmi mód szövegéből kiolvasva.</summary>
    public string HomersOsztalySzamitott => RbVedelmiModHelper.MeghatarozHomersOsztaly(VedelmiMod);

    /// <summary>
    /// Megvizsgálja, hogy a berendezés Védelmi módja megfelel-e a (helyiségre megadott) Zóna besorolásnak,
    /// az RbZonaMegfeleltetesTablazat alapján. Null, ha nincs elég adat az összehasonlításhoz.
    /// </summary>
    public bool? ZonaMegfelelo => RbZonaMegfeleltetesTablazat.Megfelel(ZonaBesorolas, VedelmiMod);
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
        @"(?:Ex|Eex|EEx)\s+([a-z]{1,3})\b",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Gázcsoport minta: IIC, IIB, IIA vagy önálló I (bányászati). A hosszabb egyezést (IIC/IIB/IIA) kell előnyben részesíteni.</summary>
    private static readonly System.Text.RegularExpressions.Regex GazcsoportMinta = new(
        @"\bII[ABC]\b|\bI\b",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Hőmérséklet osztály minta: T1-T6.</summary>
    private static readonly System.Text.RegularExpressions.Regex HomersOsztalyMinta = new(
        @"\bT[1-6]\b",
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

    /// <summary>Kiolvassa a "Hőmérséklet osztályt" (T1-T6) a Védelmi mód szövegéből.</summary>
    public static string MeghatarozHomersOsztaly(string? vedelmiMod)
    {
        if (string.IsNullOrWhiteSpace(vedelmiMod)) return "";
        var match = HomersOsztalyMinta.Match(vedelmiMod);
        return match.Success ? match.Value.ToUpperInvariant() : "";
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
        @"(?:Ex|Eex|EEx)\s+([a-z]{1,3})\b",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

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
