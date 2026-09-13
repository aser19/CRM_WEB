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
    /// <summary>Rendeltetés szerinti kockázati osztály (R1 = legmagasabb kockázat/védelmi igény, R5 = legalacsonyabb).</summary>
    public static readonly Dictionary<string, string> Rendeltetes = new()
    {
        ["R1"] = "Fokozottan veszélyeztetett létesítmény (pl. tömegtartózkodásra szolgáló, robbanásveszélyes, kiemelt értékű létesítmény)",
        ["R2"] = "Veszélyeztetett létesítmény (pl. jelentős anyagi kár, üzemszünet kockázata)",
        ["R3"] = "Közepesen veszélyeztetett létesítmény",
        ["R4"] = "Kevésbé veszélyeztetett létesítmény",
        ["R5"] = "Nem veszélyeztetett, kis kockázatú létesítmény",
    };

    /// <summary>Az épület magassága szerinti osztályba sorolás.</summary>
    public static readonly Dictionary<string, string> Magassag = new()
    {
        ["M1"] = "Alacsony épület (jellemzően 20 m alatt)",
        ["M2"] = "Közepes magasságú épület",
        ["M3"] = "Magas épület (jellemzően 60 m felett)",
    };

    /// <summary>A tetőszerkezet anyaga szerinti besorolás (tűz-/villamos szempontból).</summary>
    public static readonly Dictionary<string, string> TetoAnyaga = new()
    {
        ["T1"] = "Nehezen gyulladó, nem éghető tetőfedés (pl. fém, cserép, beton)",
        ["T2"] = "Mérsékelten éghető tetőfedés",
        ["T3"] = "Könnyen gyulladó, éghető tetőfedés (pl. zsindely, nád, szigetelt könnyűszerkezet)",
    };

    /// <summary>A körítő (homlokzati) falak anyaga szerinti besorolás.</summary>
    public static readonly Dictionary<string, string> KoritoFalakAnyaga = new()
    {
        ["K1"] = "Nem éghető anyagú falazat (tégla, beton, fém)",
        ["K2"] = "Vegyes / részben éghető anyagú falazat",
        ["K3"] = "Éghető anyagú falazat (pl. fa)",
    };

    /// <summary>A környező levegő szennyezettsége (korróziós/ipari terhelés) szerinti besorolás.</summary>
    public static readonly Dictionary<string, string> LevegoSzennyezettsege = new()
    {
        ["S1"] = "Enyhén szennyezett (lakó-, vidéki környezet)",
        ["S2"] = "Közepesen szennyezett (városi, kereskedelmi környezet)",
        ["S3"] = "Erősen szennyezett (ipari, vegyi, tengerparti/sós környezet)",
    };

    /// <summary>Másodlagos hatások (pl. érintésvédelem, elektronikai berendezések érzékenysége) szerinti besorolás.</summary>
    public static readonly Dictionary<string, string> MasodlagosHatas = new()
    {
        ["H1"] = "Nincs jelentős másodlagos kockázat",
        ["H2"] = "Mérsékelt másodlagos kockázat (pl. érzékeny elektronikai berendezések jelenléte)",
        ["H3"] = "Fokozott másodlagos kockázat (pl. tűz-/robbanásveszély, kritikus infrastruktúra)",
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
    {
        new VillamBesorolasSor { SzempontKod = "Rendeltetes", Megnevezes = "Rendeltetés" },
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
