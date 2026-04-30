namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Túláramvédelmi eszköz típusa és paraméterei
/// Pl. A9Z422316 → In=16A, Karakterisztika=C
/// </summary>
public class TularamvedelemTipus
{
    public int Id { get; set; }

    /// <summary>Típus megnevezése (pl. A9Z422316, TDK-C16)</summary>
    public string Nev { get; set; } = "";

    /// <summary>Névleges áram (In) Amperben</summary>
    public decimal NevlegesAram { get; set; }

    /// <summary>Leírás (opcionális)</summary>
    public string? Leiras { get; set; }

    /// <summary>Aktív-e</summary>
    public bool Aktiv { get; set; } = true;

    public DateTime Letrehozva { get; set; } = DateTime.Now;

    // ============================================================
    // Számított property-k
    // ============================================================

    private const decimal U0 = 230m;
    private const decimal Alfa = 0.8m;

    /// <summary>
    /// Kinyeri a karakterisztikát (B/C/D) a Nev mezőből.
    /// Pl. "TDK-C16" → "C", "B20" → "B"
    /// </summary>
    public string? Karakterisztika
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Nev)) return null;
            var match = System.Text.RegularExpressions.Regex.Match(
                Nev, @"[BbCcDd](?=\d)",
                System.Text.RegularExpressions.RegexOptions.None);
            return match.Success ? match.Value.ToUpperInvariant() : null;
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
    /// Számított maximális megengedett hurokimpedancia:
    /// Zs max = (U0 × α) / (In × szorzó)
    /// Ha nincs karakterisztika: U0 / In (egyszerűsített)
    /// </summary>
    public decimal MaxHurokimpedancia
    {
        get
        {
            if (NevlegesAram <= 0) return 0;
            if (KikapcsolasiSzorzo.HasValue)
                return Math.Round((U0 * Alfa) / (NevlegesAram * KikapcsolasiSzorzo.Value), 3);
            return Math.Round(U0 / NevlegesAram, 3);
        }
    }
}