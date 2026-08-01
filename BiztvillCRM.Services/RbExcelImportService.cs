using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using ClosedXML.Excel;

namespace BiztvillCRM.Services;

public class RbExcelImportService : IRbExcelImportService
{
    // Oszlopnév-szinonimák a rugalmas fejléc-illesztéshez (a felhasználó mintája szerint:
    // Sorsz. | Elhelyezés | Megnevezés | Áramköri megjelölés | Gyártó | Típus | Gyári szám | IP védelem | Védelmi mód | eng. szám | Minősítés)
    private static readonly Dictionary<string, string[]> OszlopSzinonimak = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Sorsz"] = new[] { "sorsz", "sorszám", "sor" },
        ["Elhelyezes"] = new[] { "elhelyezés", "elhelyezes", "hely", "helye" },
        ["Megnevezes"] = new[] { "megnevezés", "megnevezes", "berendezés", "berendezes" },
        ["AramkoriJel"] = new[] { "áramköri megjelölés", "aramkori megjeloles", "áramköri jel", "jele" },
        ["Gyarto"] = new[] { "gyártó", "gyarto", "gyártó cég", "gyarto ceg" },
        ["Tipus"] = new[] { "típus", "tipus", "típusjel", "tipusjel" },
        ["GyariSzam"] = new[] { "gyári szám", "gyari szam" },
        ["IpVedelem"] = new[] { "ip védelem", "ip vedelem", "ip védettség", "ip vedettseg" },
        ["VedelmiMod"] = new[] { "védelmi mód", "vedelmi mod", "rb védelmi mód", "rb vedelmi mod" },
        ["EngSzam"] = new[] { "eng. szám", "eng szam", "engedélyszám", "engedelyszam" },
        ["Minositas"] = new[] { "minősítés", "minosites" },
    };

    public List<RbSor> Beolvasas(byte[] fajlTartalom)
    {
        var eredmeny = new List<RbSor>();

        using var stream = new MemoryStream(fajlTartalom);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var fejlecSor = worksheet.FirstRowUsed();
        if (fejlecSor == null) return eredmeny;

        var oszlopIndexek = OszlopokFelismerese(fejlecSor);

        int sorszamlalo = 1;

        foreach (var sor in worksheet.RowsUsed().Skip(fejlecSor.RowNumber()))
        {
            // üres sor kihagyása
            if (sor.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                continue;

            var rbSor = new RbSor
            {
                Sorsz = sorszamlalo++,
                Elhelyezes = CellaErteke(sor, oszlopIndexek, "Elhelyezes"),
                Megnevezes = CellaErteke(sor, oszlopIndexek, "Megnevezes"),
                AramkoriJel = CellaErteke(sor, oszlopIndexek, "AramkoriJel"),
                Gyarto = CellaErteke(sor, oszlopIndexek, "Gyarto"),
                Tipus = CellaErteke(sor, oszlopIndexek, "Tipus"),
                GyariSzam = CellaErteke(sor, oszlopIndexek, "GyariSzam"),
                IpVedelem = CellaErteke(sor, oszlopIndexek, "IpVedelem"),
                VedelmiMod = CellaErteke(sor, oszlopIndexek, "VedelmiMod"),
                EngSzam = CellaErteke(sor, oszlopIndexek, "EngSzam"),
            };

            var minositas = CellaErteke(sor, oszlopIndexek, "Minositas");
            if (!string.IsNullOrWhiteSpace(minositas))
                rbSor.Minositas = minositas;

            eredmeny.Add(rbSor);
        }

        return eredmeny;
    }

    private static Dictionary<string, int> OszlopokFelismerese(IXLRow fejlecSor)
    {
        var talalatok = new Dictionary<string, int>();

        foreach (var cella in fejlecSor.CellsUsed())
        {
            var szoveg = cella.GetString().Trim();
            if (string.IsNullOrEmpty(szoveg)) continue;

            foreach (var (mezo, szinonimak) in OszlopSzinonimak)
            {
                if (talalatok.ContainsKey(mezo)) continue;

                if (szinonimak.Any(sz => szoveg.Contains(sz, StringComparison.OrdinalIgnoreCase)))
                {
                    talalatok[mezo] = cella.Address.ColumnNumber;
                }
            }
        }

        return talalatok;
    }

    private static string CellaErteke(IXLRow sor, Dictionary<string, int> oszlopIndexek, string mezo)
    {
        if (!oszlopIndexek.TryGetValue(mezo, out var oszlop)) return "";
        return sor.Cell(oszlop).GetString().Trim();
    }
}
