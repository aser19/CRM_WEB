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

    /// <summary>Felülvizsgálatra vár - automatikusan létrehozott eszköz, amit még nem ellenőrzött az admin</summary>
    public bool FelulvizsgalasraVar { get; set; } = false;

    public DateTime Letrehozva { get; set; } = DateTime.Now;

    /// <summary>
    /// Manuálisan megadott karakterisztika (A/B/C/D).
    /// Ha be van állítva, felülírja az automatikus névből való felismerést.
    /// </summary>
    public string? KarakterisztikaFeluliras { get; set; }

    // ============================================================
    // Számított property-k
    // ============================================================

    /// <summary>Hálózati feszültség (Uo)</summary>
    private const decimal Uo = 230m;

    /// <summary>
    /// Karakterisztika (A/B/C/D):
    /// 1. Elsőként a manuálisan megadott KarakterisztikaFeluliras-t veszi figyelembe.
    /// 2. Ha nincs, megpróbálja kiolvasni a névből (pl. "TDK-C16" → "C").
    /// </summary>
    public string? Karakterisztika
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(KarakterisztikaFeluliras))
                return KarakterisztikaFeluliras.ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(Nev)) return null;
            var match = System.Text.RegularExpressions.Regex.Match(
                Nev, @"[AaBbCcDd](?=\d)",
                System.Text.RegularExpressions.RegexOptions.None);
            return match.Success ? match.Value.ToUpperInvariant() : null;
        }
    }

    /// <summary>
    /// Kikapcsolási áram szorzó karakterisztikától függően (IEC 60898):
    ///   A = 3×In
    ///   B = 5×In
    ///   C = 10×In
    ///   D = 20×In
    /// </summary>
    public decimal? KikapcsolasiSzorzo => Karakterisztika switch
    {
        "A" => 3m,
        "B" => 5m,
        "C" => 10m,
        "D" => 20m,
        _ => null
    };

    /// <summary>
    /// Azonnali kikapcsolási áram: Ia = In × szorzó
    /// </summary>
    public decimal? Ia => KikapcsolasiSzorzo.HasValue
        ? NevlegesAram * KikapcsolasiSzorzo.Value
        : null;

    /// <summary>
    /// Maximális megengedett hurokimpedancia:
    /// Zs max = Uo / Ia = 230 / (In × szorzó)
    /// Ha nincs karakterisztika: Uo / In (egyszerűsített)
    /// </summary>
    public decimal MaxHurokimpedancia
    {
        get
        {
            if (NevlegesAram <= 0) return 0;
            var ia = Ia ?? NevlegesAram;
            return Math.Round(Uo / ia, 3);
        }
    }
}