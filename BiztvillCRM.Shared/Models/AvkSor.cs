namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy áramvédő kapcsoló (RCD/MCB) mérési sora az AVK melléklet-jegyzőkönyvben.
/// </summary>
public class AvkSor
{
    public int Sorsz { get; set; }

    /// <summary>Áramkör megnevezése / helyiség</summary>
    public string AramkorNev { get; set; } = "";

    /// <summary>Típus: RCD / MCB / RCBO</summary>
    public string Tipus { get; set; } = "RCD";

    /// <summary>Névleges áram [A]</summary>
    public string NevlegesAram { get; set; } = "";

    /// <summary>Névleges kioldóáram [mA] (RCD-nél)</summary>
    public string KioldoAram { get; set; } = "30";

    /// <summary>Mért kioldási idő [ms]</summary>
    public string KioldasiIdo { get; set; } = "";

    /// <summary>Mért érintési feszültség [V]</summary>
    public string ErintesiFeszultseg { get; set; } = "";

    /// <summary>Eredmény: MF / NMF</summary>
    public string Eredmeny { get; set; } = "MF";

    /// <summary>Megjegyzés</summary>
    public string Megjegyzes { get; set; } = "";

    /// <summary>UI: nincs DB mentés, csak lokális számítás</summary>
    public bool MegfeleltE => Eredmeny == "MF";
}