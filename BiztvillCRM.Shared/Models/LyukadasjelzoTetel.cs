namespace BiztvillCRM.Shared.Models;

using BiztvillCRM.Shared.Enums;

/// <summary>
/// Egy önálló lyukadásjelző (gyári szám szerint) adatai és jegyzőkönyvei,
/// amikor egy hitelesítésen belül több lyukadásjelzőt kell kezelni.
/// JSON-ként tárolódik a Hitelesites.LyukadasjelzoTetelek mezőben.
/// </summary>
public class LyukadasjelzoTetel
{
    /// <summary>Kliensoldali egyedi azonosító (csak UI-n belüli kulcs, nem perzisztens).</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public Guid KliensId { get; set; } = Guid.NewGuid();

    /// <summary>A lyukadásjelző altípusa (pl. Túlnyomásos, Tartályos).</summary>
    public LyukadasjelzoAltipus? LyukadasjelzoAltipus { get; set; }

    /// <summary>JSON formátumban tárolt túlnyomásos lyukadásjelző vizsgálati jegyzőkönyv adatok.</summary>
    public string? LyukadasjelzoAdatokJson { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public TulnyomasosLyukadasjelzoAdatok LyukadasjelzoAdatok
    {
        get => string.IsNullOrWhiteSpace(LyukadasjelzoAdatokJson)
            ? new TulnyomasosLyukadasjelzoAdatok()
            : System.Text.Json.JsonSerializer.Deserialize<TulnyomasosLyukadasjelzoAdatok>(LyukadasjelzoAdatokJson)
              ?? new TulnyomasosLyukadasjelzoAdatok();
        set => LyukadasjelzoAdatokJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

    /// <summary>JSON formátumban tárolt "Folyadékkal működtetett lyukadásjelző" vizsgálati jegyzőkönyv adatok.</summary>
    public string? FolyadekosLyukadasjelzoAdatokJson { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public FolyadekosLyukadasjelzoAdatok FolyadekosLyukadasjelzoAdatok
    {
        get => string.IsNullOrWhiteSpace(FolyadekosLyukadasjelzoAdatokJson)
            ? new FolyadekosLyukadasjelzoAdatok()
            : System.Text.Json.JsonSerializer.Deserialize<FolyadekosLyukadasjelzoAdatok>(FolyadekosLyukadasjelzoAdatokJson)
              ?? new FolyadekosLyukadasjelzoAdatok();
        set => FolyadekosLyukadasjelzoAdatokJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

    /// <summary>Legenerált vizsgálati jegyzőkönyv fájlok listája JSON formátumban.</summary>
    public string? JegyzokonyvPaths { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> JegyzokonyvPathsLista
    {
        get
        {
            if (string.IsNullOrWhiteSpace(JegyzokonyvPaths))
                return new List<string>();
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(JegyzokonyvPaths) ?? new List<string>();
            }
            catch (System.Text.Json.JsonException)
            {
                return new List<string>();
            }
        }
        set => JegyzokonyvPaths = (value == null || !value.Any())
            ? null
            : System.Text.Json.JsonSerializer.Serialize(value);
    }
}
