namespace BiztvillCRM.Shared.Models;

using BiztvillCRM.Shared.Enums;

/// <summary>
/// Egy hitelesítési csoporton belüli közbenső vizsgálat egyedi lejárati dátuma.
/// JSON-ként tárolódik a Hitelesites.CsoportTagLejaratok mezőben.
/// </summary>
public class CsoportTagLejaratReszlet
{
    public int EszkozTipusId { get; set; }
    public string EszkozTipusNev { get; set; } = "";
    public DateTime? LejaratDatum { get; set; }
    public string? Megjegyzes { get; set; }

    /// <summary>
    /// Ha true, ez a vizsgálat ennél a konkrét eszköznél nem alkalmazható
    /// (pl. nincs lyukadásjelző szimpla falú, kármentős tartálynál).
    /// </summary>
    public bool NemAlkalmazhato { get; set; } = false;

    /// <summary>
    /// Munkalap fájl relatív elérési útja (strukturált mappában)
    /// LEGACY: egyetlen fájl támogatáshoz (visszafele kompatibilitás)
    /// </summary>
    public string? MunkalapPath { get; set; }

    /// <summary>
    /// Hitelesítési bizonyítvány fájl relatív elérési útja (strukturált mappában)
    /// LEGACY: egyetlen fájl támogatáshoz (visszafele kompatibilitás)
    /// </summary>
    public string? BizonyitvanyPath { get; set; }

    /// <summary>
    /// Munkalap fájlok listája JSON formátumban (több fájl támogatás)
    /// </summary>
    public string? MunkalapPaths { get; set; }

    /// <summary>
    /// Bizonyítvány fájlok listája JSON formátumban (több fájl támogatás)
    /// </summary>
    public string? BizonyitvanyPaths { get; set; }

    /// <summary>
    /// Nem mapped property: munkalap fájlok strukturált formában
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> MunkalapPathsLista
    {
        get
        {
            // Visszafele kompatibilitás: ha van régi MunkalapPath, azt is hozzáadjuk
            var lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(MunkalapPath))
                lista.Add(MunkalapPath);
            if (!string.IsNullOrWhiteSpace(MunkalapPaths))
            {
                var deserializalt = System.Text.Json.JsonSerializer.Deserialize<List<string>>(MunkalapPaths);
                if (deserializalt != null)
                    lista.AddRange(deserializalt.Where(p => !lista.Contains(p)));
            }
            return lista;
        }
        set
        {
            if (value == null || !value.Any())
            {
                MunkalapPaths = null;
                MunkalapPath = null;
            }
            else
            {
                MunkalapPaths = System.Text.Json.JsonSerializer.Serialize(value);
                MunkalapPath = value.FirstOrDefault(); // visszafele kompatibilitás
            }
        }
    }

    /// <summary>
    /// Nem mapped property: bizonyítvány fájlok strukturált formában
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> BizonyitvanyPathsLista
    {
        get
        {
            // Visszafele kompatibilitás: ha van régi BizonyitvanyPath, azt is hozzáadjuk
            var lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(BizonyitvanyPath))
                lista.Add(BizonyitvanyPath);
            if (!string.IsNullOrWhiteSpace(BizonyitvanyPaths))
            {
                var deserializalt = System.Text.Json.JsonSerializer.Deserialize<List<string>>(BizonyitvanyPaths);
                if (deserializalt != null)
                    lista.AddRange(deserializalt.Where(p => !lista.Contains(p)));
            }
            return lista;
        }
        set
        {
            if (value == null || !value.Any())
            {
                BizonyitvanyPaths = null;
                BizonyitvanyPath = null;
            }
            else
            {
                BizonyitvanyPaths = System.Text.Json.JsonSerializer.Serialize(value);
                BizonyitvanyPath = value.FirstOrDefault(); // visszafele kompatibilitás
            }
        }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? LejaratDatumNullable
    {
        get => LejaratDatum;
        set => LejaratDatum = value;
    }

    /// <summary>
    /// Legenerált vizsgálati jegyzőkönyv fájlok listája JSON formátumban
    /// (pl. a túlnyomásos lyukadásjelző időszakos vizsgálatáról szóló jegyzőkönyv).
    /// </summary>
    public string? JegyzokonyvPaths { get; set; }

    /// <summary>
    /// Nem mapped property: jegyzőkönyv fájlok strukturált formában
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> JegyzokonyvPathsLista
    {
        get
        {
            if (string.IsNullOrWhiteSpace(JegyzokonyvPaths))
                return new List<string>();
            var deserializalt = System.Text.Json.JsonSerializer.Deserialize<List<string>>(JegyzokonyvPaths);
            return deserializalt ?? new List<string>();
        }
        set
        {
            JegyzokonyvPaths = (value == null || !value.Any())
                ? null
                : System.Text.Json.JsonSerializer.Serialize(value);
        }
    }

    /// <summary>
    /// Ha true, ennél a közbenső vizsgálatnál be van kapcsolva a jegyzőkönyv
    /// kitöltési mód (pl. Tartály - Lyukadásjelző minősítés a Tartály csoporton belül).
    /// </summary>
    public bool JegyzokonyvKitoltesMod { get; set; } = false;

    /// <summary>
    /// A közbenső vizsgálathoz tartozó lyukadásjelző altípus (pl. Túlnyomásos, Tartályos),
    /// ugyanúgy mint a fő eszköztípusként kiválasztott lyukadásjelzőnél.
    /// </summary>
    public LyukadasjelzoAltipus? LyukadasjelzoAltipus { get; set; }

    /// <summary>
    /// JSON formátumban tárolt lyukadásjelző vizsgálati jegyzőkönyv adatok ehhez a közbenső vizsgálathoz.
    /// </summary>
    public string? LyukadasjelzoAdatokJson { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public TulnyomasosLyukadasjelzoAdatok? LyukadasjelzoAdatok
    {
        get => string.IsNullOrWhiteSpace(LyukadasjelzoAdatokJson)
            ? null
            : System.Text.Json.JsonSerializer.Deserialize<TulnyomasosLyukadasjelzoAdatok>(LyukadasjelzoAdatokJson);
        set => LyukadasjelzoAdatokJson = value == null ? null : System.Text.Json.JsonSerializer.Serialize(value);
    }

    /// <summary>JSON formátumban tárolt "Folyadékkal működtetett lyukadásjelző" vizsgálati jegyzőkönyv adatok ehhez a közbenső vizsgálathoz.</summary>
    public string? FolyadekosLyukadasjelzoAdatokJson { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public FolyadekosLyukadasjelzoAdatok? FolyadekosLyukadasjelzoAdatok
    {
        get => string.IsNullOrWhiteSpace(FolyadekosLyukadasjelzoAdatokJson)
            ? null
            : System.Text.Json.JsonSerializer.Deserialize<FolyadekosLyukadasjelzoAdatok>(FolyadekosLyukadasjelzoAdatokJson);
        set => FolyadekosLyukadasjelzoAdatokJson = value == null ? null : System.Text.Json.JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// JSON formátumban tárolt lista, amely lehetővé teszi, hogy egy lyukadásjelző
    /// közbenső vizsgálaton (csoporttagon) belül is több, önálló gyári számú
    /// lyukadásjelzőt lehessen rögzíteni, saját adatlappal és saját PDF-jegyzőkönyvvel.
    /// </summary>
    public string? LyukadasjelzoTetelek { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<LyukadasjelzoTetel> LyukadasjelzoTetelekLista
    {
        get => string.IsNullOrWhiteSpace(LyukadasjelzoTetelek)
            ? new List<LyukadasjelzoTetel>()
            : System.Text.Json.JsonSerializer.Deserialize<List<LyukadasjelzoTetel>>(LyukadasjelzoTetelek) ?? new List<LyukadasjelzoTetel>();
        set => LyukadasjelzoTetelek = (value == null || !value.Any())
            ? null
            : System.Text.Json.JsonSerializer.Serialize(value);
    }
}