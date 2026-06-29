namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Üzemeltetői sablon mezője - meghatározza, milyen adatokat kell rögzíteni az üzemeltetőnek.
/// </summary>
public class UzemeltetoSablonMezo
{
    public int Id { get; set; }

    /// <summary>Melyik sablonhoz tartozik</summary>
    public int UzemeltetoSablonId { get; set; }
    public UzemeltetoSablon UzemeltetoSablon { get; set; } = null!;

    /// <summary>Mező neve (pl. "Eszköz típusa", "Ellenőrzés dátuma")</summary>
    public string MezoNev { get; set; } = string.Empty;

    /// <summary>Mező típusa (Text, Datum, Szam, Boolean, Fajl)</summary>
    public string MezoTipus { get; set; } = string.Empty;

    /// <summary>Kötelező kitölteni?</summary>
    public bool Kotelezo { get; set; } = true;

    /// <summary>Sorrend a sablon mezői között</summary>
    public int Sorrend { get; set; }

    /// <summary>Alapértelmezett érték (opcionális)</summary>
    public string? AlapErtek { get; set; }

    /// <summary>Súgó szöveg a felhasználónak</summary>
    public string? Sugo { get; set; }

    /// <summary>Validációs szabály (pl. minimum/maximum érték, regex pattern)</summary>
    public string? ValidaciosSzabaly { get; set; }
}
