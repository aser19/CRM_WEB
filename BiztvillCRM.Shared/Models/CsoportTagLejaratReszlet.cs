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

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? LejaratDatumNullable
    {
        get => LejaratDatum;
        set => LejaratDatum = value;
    }
}