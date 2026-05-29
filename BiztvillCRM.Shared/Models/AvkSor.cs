namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy áramvédő kapcsoló (RCD/RCBO) mérési sora az AVK melléklet-jegyzőkönyvben.
/// In, IΔn, Un és Pólusszám a kiválasztott típusból töltődik be.
/// t1x és t5x manuálisan kerül beírásra.
/// </summary>
public class AvkSor
{
    public int Sorsz { get; set; }
    public string Jele { get; set; } = "";
    public int? AvkTipusId { get; set; }
    public string TipusNev { get; set; } = "";
    public string TipusKod { get; set; } = "AC";
    public string Helye { get; set; } = "";
    public string In { get; set; } = "";
    public string IDn { get; set; } = "";
    public string IDnMert { get; set; } = "";
    public string Un { get; set; } = "";
    public string Polusszam { get; set; } = "";
    public string T1x { get; set; } = "";
    public string T5x { get; set; } = "";

    // 4 pólusú (L1/L2/L3) értékek
    public string T1xL1 { get; set; } = "";
    public string T1xL2 { get; set; } = "";
    public string T1xL3 { get; set; } = "";
    public string T5xL1 { get; set; } = "";
    public string T5xL2 { get; set; } = "";
    public string T5xL3 { get; set; } = "";

    public string Eredmeny { get; set; } = "MF";
    public string Megjegyzes { get; set; } = "";
    public bool MegfeleltE => Eredmeny == "MF";

    /// <summary>Igaz, ha 4 pólusú (háromfázisú mérés szükséges)</summary>
    public bool IsNegyPolus => Polusszam == "4";

    /// <summary>Word-be kerülő összesített t1x érték</summary>
    public string T1xWord => IsNegyPolus
        ? $"L1: {T1xL1} / L2: {T1xL2} / L3: {T1xL3}"
        : T1x;

    /// <summary>Word-be kerülő összesített t5x érték</summary>
    public string T5xWord => IsNegyPolus
        ? $"L1: {T5xL1} / L2: {T5xL2} / L3: {T5xL3}"
        : T5x;
}