namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Vizsgált berendezés mérési pontjának egy sora.
/// A rendszer típusát (TN/TT/IT) a JegyzokonyvAdatok.MeresiRendszerTipus tárolja.
/// </summary>
public class MeresiPontSor
{
    public int Sorszam { get; set; }
    public string MeresiPontHelye { get; set; } = "";
    public string Modszer { get; set; } = "";
    public string TularamvedelemHelye { get; set; } = "";
    public string TularamvedelemTipusa { get; set; } = "";
    public bool AVKCsatolva { get; set; } = false;
    public string AVK { get; set; } = "";
    public string PEFolytOhm { get; set; } = "";
    public bool PEFolytMegfelelt { get; set; } = false;
    public string ErtekOhm { get; set; } = "";
    public decimal? MertHurokimpedancia { get; set; }
    public string Minosites { get; set; } = "MEGFELELT";
    public string Megjegyzes { get; set; } = "";
    public string HelyisegNev { get; set; } = "";

    // ============================================================
    // Számított property-k (Zs ≤ U0 * α / Ia)
    // ============================================================

    //    private const decimal U0 = 230m;
    //    private const decimal Alfa = 0.8m;
    //
    //    /// <summary>
    //    /// Kinyeri a karakterisztikát (B/C/D) a TularamvedelemTipusa mezőből.
    //    /// Pl. "TDK—C16" → "C", "B20" → "B", "D10" → "D"
    //    /// </summary>
    //    public string? Karakterisztika
    //    {
    //        get
    //        {
    //            if (string.IsNullOrWhiteSpace(TularamvedelemTipusa)) return null;
    //            var match = System.Text.RegularExpressions.Regex.Match(
    //                TularamvedelemTipusa, @"[BbCcDd](?=\d)", 
    //                System.Text.RegularExpressions.RegexOptions.None);
    //            return match.Success ? match.Value.ToUpperInvariant() : null;
    //        }
    //    }
    //
    //    /// <summary>
    //    /// Kinyeri a névleges áramot (In) a TularamvedelemTipusa mezőből.
    //    /// Pl. "TDK—C16" → 16, "B20" → 20
    //    /// </summary>
    //    public decimal? NevlegesAram
    //    {
    //        get
    //        {
    //            if (string.IsNullOrWhiteSpace(TularamvedelemTipusa)) return null;
    //            var match = System.Text.RegularExpressions.Regex.Match(
    //                TularamvedelemTipusa, @"[BbCcDd](\d+(\.\d+)?)");
    //            if (match.Success && decimal.TryParse(
    //                match.Groups[1].Value, 
    //                System.Globalization.NumberStyles.Any,
    //                System.Globalization.CultureInfo.InvariantCulture, 
    //                out var in_))
    //                return in_;
    //            return null;
    //        }
    //    }
    //
    //    /// <summary>
    //    /// Kikapcsolási szorzó karakterisztikától függően:
    //    /// B = 5, C = 10, D = 20
    //    /// </summary>
    //    public decimal? KikapcsolasiSzorzo => Karakterisztika switch
    //    {
    //        "B" => 5m,
    //        "C" => 10m,
    //        "D" => 20m,
    //        _ => null
    //    };
    //
    //    /// <summary>
    //    /// Maximális megengedett hurokimpedancia: Zs ≤ (U0 × α) / Ia
    //    /// ahol Ia = In × szorzó
    //    /// </summary>
    //    public decimal? ZsMaxOhm
    //    {
    //        get
    //        {
    //            var in_ = NevlegesAram;
    //            var szorzo = KikapcsolasiSzorzo;
    //            if (in_ == null || szorzo == null || in_ == 0) return null;
    //            var ia = in_.Value * szorzo.Value;
    //            return Math.Round((U0 * Alfa) / ia, 3);
    //        }
    //    }
    //
    //    /// <summary>
    //    /// Automatikus minősítés a mért és megengedett Zs alapján.
    //    /// </summary>
    //    public string SzamitottMinosites
    //    {
    //        get
    //        {
    //            if (MertHurokimpedancia == null || ZsMaxOhm == null) return Minosites;
    //            return MertHurokimpedancia.Value <= ZsMaxOhm.Value ? "MEGFELELT" : "NEM FELELT MEG";
    //        }
    //    }

    /// <summary>
    /// Igaz, ha a sor minden kötelező mezője ki van töltve.
    /// </summary>
    public bool TeljeskituoltE =>
           !string.IsNullOrWhiteSpace(MeresiPontHelye) &&
           !string.IsNullOrWhiteSpace(Modszer) &&
           !string.IsNullOrWhiteSpace(TularamvedelemHelye) &&
           !string.IsNullOrWhiteSpace(TularamvedelemTipusa) &&
           (MertHurokimpedancia.HasValue || !string.IsNullOrWhiteSpace(ErtekOhm));

    public IEnumerable<string> HianyzoPontok()
    {
        if (string.IsNullOrWhiteSpace(MeresiPontHelye)) yield return "Mérési pont helye";
        if (string.IsNullOrWhiteSpace(Modszer)) yield return "Módszer/Osztály";
        if (string.IsNullOrWhiteSpace(TularamvedelemHelye)) yield return "Túláramvédelem helye";
        if (string.IsNullOrWhiteSpace(TularamvedelemTipusa)) yield return "Túláramvédelem típusa";
        if (!MertHurokimpedancia.HasValue && string.IsNullOrWhiteSpace(ErtekOhm))
            yield return "Érték [Ω]";
    }
}