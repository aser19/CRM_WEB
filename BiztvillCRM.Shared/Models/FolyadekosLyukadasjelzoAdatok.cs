namespace BiztvillCRM.Shared.Models;

/// <summary>
/// A "Folyadékkal működtetett lyukadásjelző időszakos vizsgálatáról" szóló
/// vizsgálati jegyzőkönyv kitöltéséhez szükséges adatok.
/// </summary>
public class FolyadekosLyukadasjelzoAdatok
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

    /// <summary>Következő vizsgálat időpontja (opcionális, a vizsgálat időpontja alatt jelenik meg).</summary>
    public DateTime? KovetkezoVizsgalatIdopontja { get; set; }

    /// <summary>Az eszköz azonosítója (ha ki van töltve a hitelesítésnél), a jegyzőkönyvben jelenik meg.</summary>
    public string? EszkozAzonosito { get; set; }

    /// <summary>Jelen vannak (név, cég)</summary>
    public string JelenlevokNevCeg { get; set; } = string.Empty;

    /// <summary>Üzemeltető részéről</summary>
    public string UzemeltetoReszerol { get; set; } = string.Empty;

    /// <summary>Vizsgálatot végző</summary>
    public string VizsgalatotVegzo { get; set; } = "Petrol Kft";

    /// <summary>Lyukadásjelző típusa (pl. AFRISO LAG-14)</summary>
    public string LyukadasjelzoTipusa { get; set; } = string.Empty;

    /// <summary>Lyukadásjelző berendezés megfelel-e</summary>
    public bool? LyukadasjelzoBerendezesMegfelel { get; set; }

    /// <summary>Egyéb kiegészítő berendezés működő-képessége, tisztasága megfelel-e</summary>
    public bool? KiegeszitoBerendezesMegfelel { get; set; }

    /// <summary>Letöltőcső megfelel-e</summary>
    public bool? LetoltocsoMegfelel { get; set; }

    public string Szabvany { get; set; } = "MSZ 9909-4:4.4.4. pontja szerint a szivárgásérzékelő megfelelően működik.";

    /// <summary>Következő ellenőrzés esedékessége évben (alapértelmezett: 1 év múlva)</summary>
    public int KovetkezoEllenorzesEvMulva { get; set; } = 1;

    public string? Megjegyzes { get; set; }
}
