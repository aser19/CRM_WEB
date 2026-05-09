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

    /// <summary>Névleges áram In [A] – típusból</summary>
    public string In { get; set; } = "";

    /// <summary>Névleges kioldóáram IΔn [mA] – típusból</summary>
    public string IDn { get; set; } = "";

    /// <summary>Mért kioldóáram IΔn [mA] – manuális</summary>
    public string IDnMert { get; set; } = "";

    public string Un { get; set; } = "";
    public string Polusszam { get; set; } = "";
    public string T1x { get; set; } = "";
    public string T5x { get; set; } = "";
    public string Eredmeny { get; set; } = "MF";
    public string Megjegyzes { get; set; } = "";
    public bool MegfeleltE => Eredmeny == "MF";
}