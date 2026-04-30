namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Vizsgált berendezés mérési pontjának egy sora.
/// A rendszer típusát (TN/TT/IT) a JegyzokonyvAdatok.MeresiRendszerTipus tárolja.
/// </summary>
public class MeresiPontSor
{
    /// <summary>Sorszám (1., 2., 3., ...)</summary>
    public int Sorszam { get; set; }
    
    /// <summary>Mérési pont helye, megnevezése, egyéb közlendő adat (pl. Daf-102-YM-J-3G2.5mm²-F24a)</summary>
    public string MeresiPontHelye { get; set; } = "";
    
    /// <summary>Módszer/Oszlop (pl. I/0Ω)</summary>
    public string Modszer { get; set; } = "";
    
    /// <summary>Túláramvédelem Helye (pl. E3)</summary>
    public string TularamvedelemHelye { get; set; } = "";
    
    /// <summary>Túláramvédelem Típusa (I_n, kar.) (pl. TDK—C16 vagy C16)</summary>
    public string TularamvedelemTipusa { get; set; } = "";

    /// <summary>ÁVK checkbox értéke (true = ✓ zöld, false = ✗ piros)</summary>
    public bool AVKCsatolva { get; set; } = false;
    
    /// <summary>ÁVK (automatikus visszakapcsoló érték)</summary>
    public string AVK { get; set; } = "";

    /// <summary>PE foly. (védővezető folytonossági ellenállás Ω-ban)</summary>
    public string PEFolytOhm { get; set; } = "";
    
    /// <summary>ÉRTÉK [Ω] (hurokimpedancia/érintésfeszültség Ω-ban) - szabad szöveges</summary>
    public string ErtekOhm { get; set; } = "";
    
    /// <summary>Mért hurokimpedancia (Zs) számértékként</summary>
    public decimal? MertHurokimpedancia { get; set; }
    
    /// <summary>MINŐSÍTÉS (MEGFELELT / NEM FELELT MEG)</summary>
    public string Minosites { get; set; } = "MEGFELELT";
    
    /// <summary>Kiegészítő megjegyzés</summary>
    public string Megjegyzes { get; set; } = "";

    // ============================================================
    // Számított property-k (Zs ≤ U0 * α / Ia)
    // ============================================================

    private const decimal U0 = 230m;
    private const decimal Alfa = 0.8m;

    /// <summary>
    /// Kinyeri a karakterisztikát (B/C/D) a TularamvedelemTipusa mezőből.
    /// Pl. "TDK—C16" → "C", "B20" → "B", "D10" → "D"
    /// </summary>
    public string? Karakterisztika
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TularamvedelemTipusa)) return null;
            var match = System.Text.RegularExpressions.Regex.Match(
                TularamvedelemTipusa, @"[BbCcDd](?=\d)", 
                System.Text.RegularExpressions.RegexOptions.None);
            return match.Success ? match.Value.ToUpperInvariant() : null;
        }
    }

    /// <summary>
    /// Kinyeri a névleges áramot (In) a TularamvedelemTipusa mezőből.
    /// Pl. "TDK—C16" → 16, "B20" → 20
    /// </summary>
    public decimal? NevlegesAram
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TularamvedelemTipusa)) return null;
            var match = System.Text.RegularExpressions.Regex.Match(
                TularamvedelemTipusa, @"[BbCcDd](\d+(\.\d+)?)");
            if (match.Success && decimal.TryParse(
                match.Groups[1].Value, 
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, 
                out var in_))
                return in_;
            return null;
        }
    }

    /// <summary>
    /// Kikapcsolási szorzó karakterisztikától függően:
    /// B = 5, C = 10, D = 20
    /// </summary>
    public decimal? KikapcsolasiSzorzo => Karakterisztika switch
    {
        "B" => 5m,
        "C" => 10m,
        "D" => 20m,
        _ => null
    };

    /// <summary>
    /// Maximális megengedett hurokimpedancia: Zs ≤ (U0 × α) / Ia
    /// ahol Ia = In × szorzó
    /// </summary>
    public decimal? ZsMaxOhm
    {
        get
        {
            var in_ = NevlegesAram;
            var szorzo = KikapcsolasiSzorzo;
            if (in_ == null || szorzo == null || in_ == 0) return null;
            var ia = in_.Value * szorzo.Value;
            return Math.Round((U0 * Alfa) / ia, 3);
        }
    }

    /// <summary>
    /// Automatikus minősítés a mért és megengedett Zs alapján.
    /// </summary>
    public string SzamitottMinosites
    {
        get
        {
            if (MertHurokimpedancia == null || ZsMaxOhm == null) return Minosites;
            return MertHurokimpedancia.Value <= ZsMaxOhm.Value ? "MEGFELELT" : "NEM FELELT MEG";
        }
    }
}