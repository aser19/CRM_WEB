namespace BiztvillCRM.Shared.Models;

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
}