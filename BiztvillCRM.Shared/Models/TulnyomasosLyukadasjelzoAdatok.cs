namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A "Túlnyomással működtetett lyukadásjelző időszakos vizsgálatáról" szóló
/// vizsgálati jegyzőkönyv kitöltéséhez szükséges adatok.
/// </summary>
public class TulnyomasosLyukadasjelzoAdatok
{
    /// <summary>Jegyzőkönyv sorszáma (pl. LYUK-000123/2025). A kiállító cég alapján generált, auto-increment sorszám. Egyszer generálódik, utána stabil marad.</summary>
    public string? JegyzokonyvSzam { get; set; }

    /// <summary>
    /// A jegyzőkönyv kiállítás utáni módosításainak száma. 0 = eredeti, kiállítás utáni állapot; 1 = első módosítás (modv1), stb.
    /// A megjelenített jegyzőkönyv száma ilyenkor "{JegyzokonyvSzam} modv{ModositasVerzio}" formátumú.
    /// </summary>
    public int ModositasVerzio { get; set; } = 0;

    /// <summary>Az adott jegyzőkönyv-generálás/letöltés dátuma (nap pontossággal), a lábléchez.</summary>
    public DateTime? LetoltesDatum { get; set; }

    /// <summary>A megbízó (ügyfél) neve, a "Vizsgálat helye" sor felett jelenik meg a jegyzőkönyvben.</summary>
    public string? MegbizoNev { get; set; }

    public string VizsgalatHelye { get; set; } = string.Empty;
    public DateTime VizsgalatIdeje { get; set; } = DateTime.Today;

    /// <summary>Jelen vannak (név, cég)</summary>
    public string JelenlevokNevCeg { get; set; } = string.Empty;

    /// <summary>Üzemeltető részéről</summary>
    public string UzemeltetoReszerol { get; set; } = string.Empty;

    /// <summary>Vizsgálatot végző</summary>
    public string VizsgalatotVegzo { get; set; } = "Petrol Kft";

    public string LyukadasjelzoTipusa { get; set; } = string.Empty;
    public string LyukadasjelzoGyariszama { get; set; } = string.Empty;

    /// <summary>Lyukadásjelző berendezés felerősítése</summary>
    public bool? BerendezesFelerositeseMegfelel { get; set; }

    /// <summary>Egyéb kiegészítő berendezés működő-képessége, tisztasága</summary>
    public bool? KiegeszitoBerendezesMegfelel { get; set; }

    /// <summary>Lyukadásjelző működőképessége</summary>
    public bool? LyukadasjelzoMukodokepesMegfelel { get; set; }

    /// <summary>Ellenőrző tér nyomása bar-ban (alapértelmezett: 0,5 bar)</summary>
    public decimal EllenorzoTerNyomasaBar { get; set; } = 0.5m;

    /// <summary>Tömörségi próba időtartama percben (alapértelmezett: 30 perc)</summary>
    public int TomorsegiProbaIdotartamPerc { get; set; } = 30;

    /// <summary>A nyomás nem csökkent, alakváltozás nem történt</summary>
    public bool NyomasNemCsokkentAlakvaltozasNemTortent { get; set; } = true;

    /// <summary>A tömörségi nyomáspróba sikerrel járt-e</summary>
    public bool TomorsegiProbaSikeres { get; set; } = true;

    public string Rendelet { get; set; } = "9/2008. (II.28.) ÖTM rendelet alapján";

    public string? Megjegyzes { get; set; }
}
