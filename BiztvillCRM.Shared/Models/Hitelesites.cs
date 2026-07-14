using BiztvillCRM.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz hitelesítésének adatai.</summary>
public class Hitelesites
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    // Ügyfél
    public int? UgyfelId { get; set; }
    public Ugyfel? Ugyfel { get; set; }

    // Telephely
    public int? TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }

    // Eszköz típus (Kútoszlop, Szintmérő, stb.)
    public int EszkozTipusId { get; set; }
    public EszkozTipus? EszkozTipus { get; set; }

    // Hatóság
    public int? HatosagId { get; set; }
    public Hatosag? Hatosag { get; set; }

    public int Darabszam { get; set; } = 1;
    public DateTime Datum { get; set; }
    
    /// <summary>Általános lejárati dátum (a teljes kútoszlopra/eszközre vonatkozik).</summary>
    public DateTime? LejaratDatum { get; set; }
    
    public HitelesitesStatusz HitelesitesStatusz { get; set; }
    public string? Megjegyzes { get; set; }
    
    /// <summary>
    /// JSON formátumban tárolt lista az egyedi eszközök (pisztolyok) eltérő lejárati dátumairól.
    /// Csak akkor töltjük ki, ha van olyan pisztoly, amelynek lejárata eltér az általánostól.
    /// </summary>
    public string? EgyediLejaratok { get; set; }
    
    /// <summary>
    /// JSON formátumban tárolt közbenső vizsgálatok egyedi lejárati dátumai
    /// (hitelesítési csoporthoz tartozó tagok eltérő dátumai).
    /// </summary>
    public string? CsoportTagLejaratok { get; set; }

    /// <summary>
    /// Nem mapped property: az egyedi lejáratok strukturált formában.
    /// </summary>
    [NotMapped]
    public List<HitelesitesReszlet> EgyediLejaratokLista
    {
        get => string.IsNullOrWhiteSpace(EgyediLejaratok) 
            ? new List<HitelesitesReszlet>() 
            : JsonSerializer.Deserialize<List<HitelesitesReszlet>>(EgyediLejaratok) ?? new List<HitelesitesReszlet>();
        set => EgyediLejaratok = value.Any() 
            ? JsonSerializer.Serialize(value) 
            : null;
    }

    [NotMapped]
    public List<CsoportTagLejaratReszlet> CsoportTagLejaratokLista
    {
        get => string.IsNullOrWhiteSpace(CsoportTagLejaratok)
            ? new List<CsoportTagLejaratReszlet>()
            : System.Text.Json.JsonSerializer.Deserialize<List<CsoportTagLejaratReszlet>>(CsoportTagLejaratok)
              ?? new List<CsoportTagLejaratReszlet>();
        set => CsoportTagLejaratok = value.Any()
            ? System.Text.Json.JsonSerializer.Serialize(value)
            : null;
    }

    /// <summary>
    /// Az egyedi eszköz azonosítója a telephelyen belül.
    /// Pl. "Tartály #1", "50m³-es tartály", "Északi tároló"
    /// Üres = az egész telephely egységes hitelesítése.
    /// </summary>
    [MaxLength(200)]
    public string? EszkozAzonosito { get; set; }

    /// <summary>
    /// Aktív-e a hitelesítés. Inaktív hitelesítések nem jelennek meg az alapértelmezett listában,
    /// de megmaradnak aHistory-ban.
    /// </summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>
    /// Munkalap fájl relatív elérési útja (strukturált mappában: Cég\Ügyfél\munkalap_*.pdf)
    /// LEGACY: egyetlen fájl támogatáshoz (visszafele kompatibilitás)
    /// </summary>
    [MaxLength(500)]
    public string? MunkalapPath { get; set; }

    /// <summary>
    /// Hitelesítési bizonyítvány fájl relatív elérési útja (strukturált mappában: Cég\Ügyfél\bizonyitvany_*.pdf)
    /// LEGACY: egyetlen fájl támogatáshoz (visszafele kompatibilitás)
    /// </summary>
    [MaxLength(500)]
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
    [NotMapped]
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
                var deserializalt = JsonSerializer.Deserialize<List<string>>(MunkalapPaths);
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
                MunkalapPaths = JsonSerializer.Serialize(value);
                MunkalapPath = value.FirstOrDefault(); // visszafele kompatibilitás
            }
        }
    }

    /// <summary>
    /// Nem mapped property: bizonyítvány fájlok strukturált formában
    /// </summary>
    [NotMapped]
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
                var deserializalt = JsonSerializer.Deserialize<List<string>>(BizonyitvanyPaths);
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
                BizonyitvanyPaths = JsonSerializer.Serialize(value);
                BizonyitvanyPath = value.FirstOrDefault(); // visszafele kompatibilitás
            }
        }
    }
}
