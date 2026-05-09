namespace BiztvillCRM.Services;

/// <summary>
/// IEC 61008-1 / IEC 61009-1 / MSZ HD 60364-4-41 alapján.
/// Nem szelektív: AC, A, F, B típus
/// Szelektív: S típus (késleltetett kioldás)
/// Idők egysége: ms (milliszekundum)
/// </summary>
public static class AvkHatarertekService
{
    public record AvkHatarertekek(
        decimal MaxIDnMa,    // Max. mért IΔn [mA]
        decimal MaxT1x_s,    // Max. kioldási idő 1×IΔn-nél [ms]
        decimal MaxT5x_s     // Max. kioldási idő 5×IΔn-nél [ms]
    );

    private static readonly Dictionary<(string, string), AvkHatarertekek> _tabla = new()
    {
        // === TT HÁLÓZAT – nem szelektív (max t1×=300ms, t5×=40ms) ===
        { ("TT", "AC"), new(30, 300m, 40m) },
        { ("TT", "A"),  new(30, 300m, 40m) },
        { ("TT", "F"),  new(30, 300m, 40m) },
        { ("TT", "B"),  new(30, 300m, 40m) },
        // TT – szelektív (max t1×=500ms, t5×=150ms)
        { ("TT", "S"),  new(30, 500m, 150m) },

        // === TN HÁLÓZAT – nem szelektív ===
        { ("TN", "AC"), new(30, 300m, 40m) },
        { ("TN", "A"),  new(30, 300m, 40m) },
        { ("TN", "F"),  new(30, 300m, 40m) },
        { ("TN", "B"),  new(30, 300m, 40m) },
        // TN – szelektív
        { ("TN", "S"),  new(30, 500m, 150m) },

        // === IT HÁLÓZAT – nem szelektív ===
        { ("IT", "AC"), new(30, 300m, 40m) },
        { ("IT", "A"),  new(30, 300m, 40m) },
        { ("IT", "F"),  new(30, 300m, 40m) },
        { ("IT", "B"),  new(30, 300m, 40m) },
        // IT – szelektív
        { ("IT", "S"),  new(30, 500m, 150m) },
    };

    public static AvkHatarertekek GetHatarertekek(string? halozatTipus, string? avkTipusKod)
    {
        var h = (halozatTipus ?? "TN").ToUpperInvariant();
        var t = (avkTipusKod ?? "AC").ToUpperInvariant();

        if (_tabla.TryGetValue((h, t), out var ertek))
            return ertek;

        return _tabla[("TN", "AC")]; // fallback
    }

    public static string SzamitEredmeny(
        string? halozatTipus, string? avkTipusKod,
        string? idnMertSzoveg, string? t1xSzoveg, string? t5xSzoveg)
    {
        var hatar = GetHatarertekek(halozatTipus, avkTipusKod);
        var cult = System.Globalization.CultureInfo.InvariantCulture;
        var style = System.Globalization.NumberStyles.Any;

        bool ok = true;

        if (decimal.TryParse(idnMertSzoveg, style, cult, out var idnMert))
            if (idnMert > hatar.MaxIDnMa) ok = false;

        // t1x és t5x most ms-ben érkezik, MaxT1x_s és MaxT5x_s is ms-ben van tárolva
        if (decimal.TryParse(t1xSzoveg, style, cult, out var t1x))
            if (t1x > hatar.MaxT1x_s) ok = false;

        if (decimal.TryParse(t5xSzoveg, style, cult, out var t5x))
            if (t5x > hatar.MaxT5x_s) ok = false;

        return ok ? "MF" : "NMF";
    }
}