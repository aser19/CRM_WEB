namespace BiztvillCRM.Shared.Models;

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

    // 4 pólusú IΔn mért (L1/L2/L3)
    public string IDnMertL1 { get; set; } = "";
    public string IDnMertL2 { get; set; } = "";
    public string IDnMertL3 { get; set; } = "";

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

    // === ÚJ: Működési próba és szemrevételezés ===
    /// <summary>Működési próba megfelelt-e (MP oszlop)</summary>
    public bool MukodesProba { get; set; } = true;
    /// <summary>Szemrevételezés megfelelt-e (SZV oszlop)</summary>
    public bool Szemrevetelez { get; set; } = true;

    public bool MegfeleltE => Eredmeny == "MF";

    /// <summary>Igaz, ha 4 pólusú (háromfázisú mérés szükséges)</summary>
    public bool IsNegyPolus => Polusszam == "4";

    /// <summary>Word-be kerülő IΔn mért érték</summary>
    public string IDnMertWord => IsNegyPolus
        ? $"L1: {IDnMertL1} / L2: {IDnMertL2} / L3: {IDnMertL3}"
        : IDnMert;

    /// <summary>Word-be kerülő összesített t1x érték</summary>
    public string T1xWord => IsNegyPolus
        ? $"L1: {T1xL1} / L2: {T1xL2} / L3: {T1xL3}"
        : T1x;

    /// <summary>Word-be kerülő összesített t5x érték</summary>
    public string T5xWord => IsNegyPolus
        ? $"L1: {T5xL1} / L2: {T5xL2} / L3: {T5xL3}"
        : T5x;

    /// <summary>Igaz, ha a sor minimálisan kitöltött (típus + mért értékek megvannak)</summary>
    public bool TeljeskitoltottE =>
        !string.IsNullOrWhiteSpace(TipusNev) &&
        (IsNegyPolus
            ? (
                // IΔn mért – mind a 3 fázis
                !string.IsNullOrWhiteSpace(IDnMertL1) &&
                !string.IsNullOrWhiteSpace(IDnMertL2) &&
                !string.IsNullOrWhiteSpace(IDnMertL3) &&
                // t1× – mind a 3 fázis
                !string.IsNullOrWhiteSpace(T1xL1) &&
                !string.IsNullOrWhiteSpace(T1xL2) &&
                !string.IsNullOrWhiteSpace(T1xL3) &&
                // t5× – mind a 3 fázis
                !string.IsNullOrWhiteSpace(T5xL1) &&
                !string.IsNullOrWhiteSpace(T5xL2) &&
                !string.IsNullOrWhiteSpace(T5xL3)
              )
            : (
                !string.IsNullOrWhiteSpace(IDnMert) &&
                !string.IsNullOrWhiteSpace(T1x) &&
                !string.IsNullOrWhiteSpace(T5x)
              ));
}