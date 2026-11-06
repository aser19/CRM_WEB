using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Hordozható munkaeszközök áramütés elleni vizsgálatának jegyzőkönyve - PDF generálás.
/// A korábbi Word (MiniWord + Hordozhato.docx sablon) generálás PDF megfelelője, QuestPDF-fel.
/// </summary>
public class HordozhatoPdfService : IHordozhatoPdfService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly IFileStorageService _fileStorageService;

    public HordozhatoPdfService(IMeresService meresService, ITenantService tenantService, ICegService cegService,
        IFelulvizsgaloService felulvizsgaloService, IFileStorageService fileStorageService)
    {
        _meresService = meresService;
        _tenantService = tenantService;
        _cegService = cegService;
        _felulvizsgaloService = felulvizsgaloService;
        _fileStorageService = fileStorageService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok)
    {
        Meres? meres = null;
        if (meresId > 0)
            meres = await _meresService.GetByIdAsync(meresId);

        adatok ??= new JegyzokonyvAdatok();

        Ceg? ceg = null;
        try { ceg = await _cegService.GetByIdAsync(_tenantService.GetCurrentCegId()); }
        catch { /* nincs tenant kontextus */ }

        var megrendelo = !string.IsNullOrWhiteSpace(adatok.Megrendelo) ? adatok.Megrendelo : meres?.Ugyfel?.Nev ?? "";
        var telephely = !string.IsNullOrWhiteSpace(adatok.VizsgalatHelye) ? adatok.VizsgalatHelye : meres?.Telephely?.Cim ?? "";
        var felulvizsgalatIdeje = meres?.Datum ?? DateTime.Today;
        var kovetkezoFelulvizsgalat = adatok.KovetkezoFelulvizsgalatDatum
            ?? meres?.KovetkezoDatum
            ?? felulvizsgalatIdeje.AddYears(3);
        var munkaszam = !string.IsNullOrWhiteSpace(adatok.Munkaszam) ? adatok.Munkaszam : $"HK-{meresId:D6}/{DateTime.Now:yyyy}";
        var muszerek = adatok.Muszerek?.Where(m => !string.IsNullOrWhiteSpace(m.Tipus)).ToList() ?? new List<MuszerSor>();
        var eszkozok = adatok.Eszkozok ?? new List<HordozhatoEszkozSor>();
        var osszesDb = eszkozok.Count;
        var mfDb = eszkozok.Count(e => e.Megtekint == "MF");
        var nmfDb = eszkozok.Count(e => e.Megtekint == "NMF");

        var felulvizsgaloNev = adatok.FelulvizsgaloNev ?? "";
        var felulvizsgaloBizonyitvany = adatok.FelulvizsgaloBizonyitvany ?? "";
        var kapcsolattarto = !string.IsNullOrWhiteSpace(adatok.KapcsolatTarto) ? adatok.KapcsolatTarto : meres?.Telephely?.Kapcsolattarto ?? "";
        var jegyzokonyvKeszito = felulvizsgaloNev;
        var jegyzokonyvKeszitesIdeje = meres?.Letrehozva ?? DateTime.Now;
        var letoltesIdopontja = DateTime.Now;

        byte[]? cegBelyegzoKep = null;
        byte[]? felulvizsgaloAlairasKep = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(ceg?.BelyegzoPath))
                cegBelyegzoKep = await _fileStorageService.GetFileBytesAsync(ceg.BelyegzoPath);
        }
        catch { /* nincs bélyegző */ }

        if (!string.IsNullOrWhiteSpace(felulvizsgaloNev))
        {
            try
            {
                var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();
                var felulvizsgalo = felulvizsgalok.FirstOrDefault(f => f.Nev == felulvizsgaloNev);
                if (!string.IsNullOrWhiteSpace(felulvizsgalo?.AlairasPath))
                    felulvizsgaloAlairasKep = await _fileStorageService.GetFileBytesAsync(felulvizsgalo.AlairasPath);
            }
            catch { /* nincs aláírás */ }
        }

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.3f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Content().Column(col =>
                {
                    col.Item().Element(c => Fejlec(c, ceg, adatok, megrendelo, telephely, felulvizsgalatIdeje, kovetkezoFelulvizsgalat));
                    col.Item().PaddingTop(10).Element(c => ErtekelesBlokkok(c));
                    col.Item().PaddingTop(10).Element(c => VizsgalatModja(c));
                    col.Item().PaddingTop(10).Element(c => MuszerAdatok(c, muszerek));
                    col.Item().PaddingTop(10).Element(c => FelelosAdatok(c, felulvizsgaloNev, felulvizsgaloBizonyitvany,
                        kapcsolattarto, jegyzokonyvKeszito, jegyzokonyvKeszitesIdeje, letoltesIdopontja));
                    col.Item().PaddingTop(10).Element(c => Osszesites(c, adatok, osszesDb, mfDb, nmfDb));
                    col.Item().PaddingTop(10).Element(c => AlairasBlokk(c, cegBelyegzoKep, felulvizsgaloAlairasKep, felulvizsgaloNev));

                    col.Item().PageBreak();
                    col.Item().Element(c => EszkozTablazat(c, eszkozok));
                    col.Item().PaddingTop(10).Element(c => AlairasBlokk(c, cegBelyegzoKep, felulvizsgaloAlairasKep, felulvizsgaloNev));
                });

                page.Footer().Element(c => Lablec(c, munkaszam, ceg));
            });
        }).GeneratePdf();
    }

    private void Fejlec(IContainer container, Ceg? ceg, JegyzokonyvAdatok adatok, string megrendelo, string telephely,
        DateTime felulvizsgalatIdeje, DateTime kovetkezoFelulvizsgalat)
    {
        container.Column(col =>
        {
            col.Item().AlignCenter().Text("HORDOZHATÓ MUNKAESZKÖZÖK ÁRAMÜTÉS ELLENI").Bold().FontSize(13);
            col.Item().AlignCenter().Text("VIZSGÁLATÁNAK JEGYZŐKÖNYVE").Bold().FontSize(13);

            col.Item().PaddingTop(8).Text(text =>
            {
                text.Span($"{ceg?.Nev ?? ""} | ").SemiBold();
                text.Span("Munkavállaló neve: ").SemiBold();
                text.Span(adatok.DolgozoNeve ?? "");
                if (!string.IsNullOrWhiteSpace(adatok.ForgalmiRendszam))
                {
                    text.Span(" | Forgalmi rendszám: ").SemiBold();
                    text.Span(adatok.ForgalmiRendszam);
                }
            });

            col.Item().PaddingTop(10).AlignCenter().Text("KÖVETKEZŐ FELÜLVIZSGÁLAT IDEJE:").Bold();
            col.Item().AlignCenter().Text(kovetkezoFelulvizsgalat.ToString("yyyy.MM.dd")).Bold().FontSize(12);

            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("Megrendelő: ").SemiBold();
                        t.Span(megrendelo);
                    });
                    c.Item().Text(t =>
                    {
                        t.Span("Telephely, Belsőpar. út: ").SemiBold();
                        t.Span(telephely);
                    });
                });
                row.RelativeItem().AlignRight().Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("Felülvizsgálat időpontja: ").SemiBold();
                        t.Span(felulvizsgalatIdeje.ToString("yyyy.MM.dd"));
                    });
                });
            });

            col.Item().PaddingTop(6).Text(
                "A legutóbbi időszakos ellenőrző-felülvizsgálat elvégzéséről a munkaeszközön elhelyezett, jól látható jelzéssel tájékoztatást kell nyújtani, ha a munkaeszközt a telephelyen kívül használják. – 10/2016. (IV. 5.) NGM rendelet 18. § (4).")
                .FontSize(7).Italic();

            if (adatok.VanMatricaSorszam)
            {
                col.Item().PaddingTop(2).Text(
                    $"A felülvizsgálat során elhelyezett matricák sorozatszáma: {adatok.MatricaSorszamTol} -tól {adatok.MatricaSorszamIg} -ig tart.")
                    .FontSize(7);
            }
        });
    }

    private void ErtekelesBlokkok(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => ErtekelesSor(c, "10/2016. (IV.5.) NGM rendelet szerinti értékelés"));
            col.Item().PaddingTop(6).Element(c => ErtekelesSor(c, "40/2017. (XII.4.) NGM rendelet szerinti értékelés"));
            col.Item().PaddingTop(6).Element(c => ErtekelesSor(c, "MSZ 4851-5:1991 szerinti értékelés"));

            col.Item().PaddingTop(4).Text(
                "Vizsgálat módja: szemrevételezés | I. és II. osztályú szerkezetek")
                .FontSize(7.5f);
        });
    }

    private void ErtekelesSor(IContainer container, string cim)
    {
        container.Column(col =>
        {
            col.Item().Text(cim).Bold().FontSize(9).Italic();
            col.Item().PaddingTop(2).Row(row =>
            {
                row.RelativeItem().Text("MEGTÖRTÉNT").Bold().Underline();
                row.RelativeItem().AlignRight().Text("NEM TÖRTÉNT MEG");
            });
        });
    }

    private void VizsgalatModja(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("Vizsgálat módja:").Bold().FontSize(9.5f);
            col.Item().Text("• szemrevételezés: I. és II. osztályú szerkezetek").FontSize(8.5f);
            col.Item().Text("• szigetelés ellenállás mérés: I. osztályú szerkezetek").FontSize(8.5f);
            col.Item().Text("• védővezető folytonosság mérés: I. osztályú szerkezetek").FontSize(8.5f);
            col.Item().Text("• érintési szivárgó áram mérés: I. és II. osztályú szerkezetek").FontSize(8.5f);
        });
    }

    private void MuszerAdatok(IContainer container, List<MuszerSor> muszerek)
    {
        container.Column(col =>
        {
            col.Item().Text("A vizsgálathoz használt műszer adatai:").Bold().FontSize(9.5f);
            if (!muszerek.Any())
            {
                col.Item().Text("-").FontSize(8.5f);
                return;
            }

            col.Item().PaddingTop(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Műszer típusa").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Gyári száma").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Kalibrálás").Bold();
                });

                foreach (var m in muszerek)
                {
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(m.Tipus ?? "");
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(m.GyariSzam ?? "");
                    table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(m.Kalibralas ?? "");
                }
            });
        });
    }

    private void FelelosAdatok(IContainer container, string felulvizsgaloNev, string felulvizsgaloBizonyitvany,
        string kapcsolattarto, string jegyzokonyvKeszito, DateTime keszitesIdeje, DateTime letoltesIdopontja)
    {
        container.DefaultTextStyle(x => x.FontSize(8.5f)).Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("Felülvizsgáló neve: ").SemiBold();
                        t.Span(felulvizsgaloNev);
                    });
                    c.Item().Text(t =>
                    {
                        t.Span("Bizonyítvány száma: ").SemiBold();
                        t.Span(felulvizsgaloBizonyitvany);
                    });
                    c.Item().Text(t =>
                    {
                        t.Span("Kapcsolattartó neve: ").SemiBold();
                        t.Span(kapcsolattarto);
                    });
                });
                row.RelativeItem().AlignRight().Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("Jegyzőkönyv készítője: ").SemiBold();
                        t.Span(jegyzokonyvKeszito);
                    });
                    c.Item().Text(t =>
                    {
                        t.Span("Jegyzőkönyv készítésének ideje: ").SemiBold();
                        t.Span(keszitesIdeje.ToString("yyyy.MM.dd HH:mm"));
                    });
                    c.Item().Text(t =>
                    {
                        t.Span("Letöltés időpontja: ").SemiBold();
                        t.Span(letoltesIdopontja.ToString("yyyy.MM.dd HH:mm"));
                    });
                });
            });
        });
    }


    private void AlairasBlokk(IContainer container, byte[]? cegBelyegzoKep, byte[]? felulvizsgaloAlairasKep, string felulvizsgaloNev)
    {
        container.PaddingTop(10).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Height(50).AlignLeft().Element(e =>
                {
                    if (cegBelyegzoKep is { Length: > 0 })
                        e.Image(cegBelyegzoKep).FitArea();
                });
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                c.Item().Text("Cég bélyegzője").FontSize(7).Italic();
            });
            row.RelativeItem().Column(c =>
            {
                c.Item().Height(50).AlignRight().Element(e =>
                {
                    if (felulvizsgaloAlairasKep is { Length: > 0 })
                        e.Image(felulvizsgaloAlairasKep).FitArea();
                });
                c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                c.Item().AlignRight().Text($"Felülvizsgáló aláírása ({felulvizsgaloNev})").FontSize(7).Italic();
            });
        });
    }

    private void Osszesites(IContainer container, JegyzokonyvAdatok adatok, int osszesDb, int mfDb, int nmfDb)
    {
        container.Column(col =>
        {
            col.Item().Text(
                $"A jegyzőkönyvben összesen {osszesDb}-db eszköz került feltüntetésre, melyből:")
                .FontSize(9);
            col.Item().Text($"{mfDb} db-ot MEGFELELT minősítéssel láttunk el.").FontSize(9);
            col.Item().Text($"{nmfDb} db-ot NEM MEGFELELT minősítéssel láttunk el.").FontSize(9);

            if (!string.IsNullOrWhiteSpace(adatok.Megjegyzes))
            {
                col.Item().PaddingTop(6).Text(t =>
                {
                    t.DefaultTextStyle(x => x.FontSize(8.5f));
                    t.Span("Megjegyzés: ").SemiBold();
                    t.Span(adatok.Megjegyzes);
                });
            }

            col.Item().PaddingTop(10).Text(
                "A 10/2016. (IV.5.) NGM rendelet 18. § 4. pontja alapján: „A legutóbbi időszakos ellenőrző-felülvizsgálat elvégzéséről a munkaeszközön elhelyezett, jól látható jelzéssel tájékoztatást kell nyújtani, ha a munkaeszközt a telephelyen kívül használják.”")
                .FontSize(7).Italic();
        });
    }

    private void EszkozTablazat(IContainer container, List<HordozhatoEszkozSor> eszkozok)
    {
        container.Column(col =>
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(25);   // Nr.
                    c.RelativeColumn(2.2f); // Megnevezés
                    c.RelativeColumn(1.3f); // Típus
                    c.RelativeColumn(1.4f); // Gyártási szám
                    c.RelativeColumn(1.3f); // Jellemző-teljesítmény
                    c.RelativeColumn(1.1f); // Megtekintés
                    c.RelativeColumn(1.1f); // Folytonosság
                    c.RelativeColumn(1.1f); // Szig.Ell.
                    c.RelativeColumn(1.2f); // Szivárgó Áram
                    c.RelativeColumn(1.8f); // Megjegyzés
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Nr.").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Megnevezés").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Típus").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Gyártási szám").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Jellemző-teljesítmény").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Megtekintés").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Folytonosság").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Szig.Ell.").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Szivárgó Áram").Bold().FontSize(7.5f);
                    header.Cell().Background(Colors.Grey.Lighten2).Border(0.5f).Padding(3).Text("Megjegyzés").Bold().FontSize(7.5f);
                });

                var sorszam = 1;
                var csoportok = eszkozok
                    .GroupBy(e => string.IsNullOrWhiteSpace(e.HelyisegNev) ? "" : e.HelyisegNev)
                    .Select((g, index) => new
                    {
                        g.Key,
                        Items = g.ToList(),
                        FirstIndex = eszkozok.FindIndex(e => (e.HelyisegNev ?? "") == g.Key)
                    })
                    .OrderBy(g => string.IsNullOrEmpty(g.Key) ? int.MaxValue : g.FirstIndex);

                foreach (var csoport in csoportok)
                {
                    if (!string.IsNullOrEmpty(csoport.Key))
                    {
                        table.Cell().ColumnSpan(10).Background(Colors.Grey.Lighten3).Border(0.5f).Padding(3)
                            .Text(csoport.Key).Bold().FontSize(8);
                    }

                    foreach (var e in csoport.Items)
                    {
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text($"{sorszam}.").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Megnevezes ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Tipus ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Azonosito ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Telj ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Megtekint ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.KellFolyt ? (e.Folyt ?? "-") : "-").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Szigell ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Szivargo ?? "").FontSize(7.5f);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(e.Megjegyzes ?? "").FontSize(7.5f);
                        sorszam++;
                    }
                }
            });

            col.Item().PaddingTop(8).Text(
                $"A jegyzőkönyvben összesen {eszkozok.Count}-db eszköz került feltüntetésre, melyből:\n" +
                $"{eszkozok.Count(e => e.Megtekint == "MF")} db-ot MEGFELELT minősítéssel\n" +
                $"{eszkozok.Count(e => e.Megtekint == "NMF")} db-ot NEM MEGFELELT minősítéssel láttunk el.")
                .FontSize(8);

            col.Item().PaddingTop(4).Text(
                "A 10/2016. (IV.5.) NGM-rendelet 18. § 4. pontja alapján: „A legutóbbi időszakos ellenőrző-felülvizsgálat elvégzéséről a munkaeszközön elhelyezett, jól látható jelzéssel tájékoztatást kell nyújtani, ha a munkaeszközt a telephelyen kívül használják.”")
                .FontSize(7).Italic();
        });
    }

    private void Lablec(IContainer container, string munkaszam, Ceg? ceg)
    {
        container.PaddingTop(6).BorderTop(0.5f).BorderColor(Colors.Grey.Lighten1).PaddingTop(2).Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text($"Munkaszám: {munkaszam}").FontSize(7);
                row.RelativeItem().AlignCenter().Text($"Dátum: {DateTime.Now:yyyy.MM.dd}").FontSize(7);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Oldal: ").FontSize(7);
                    text.CurrentPageNumber().FontSize(7);
                    text.Span(" / ").FontSize(7);
                    text.TotalPages().FontSize(7);
                });
            });

            col.Item().AlignCenter().Text(t =>
            {
                t.DefaultTextStyle(x => x.FontSize(7));
                var reszek = new List<string> { ceg?.Cim ?? "" };
                if (!string.IsNullOrWhiteSpace(ceg?.Weboldal)) reszek.Add(ceg.Weboldal);
                if (!string.IsNullOrWhiteSpace(ceg?.Telefon)) reszek.Add(ceg.Telefon);
                t.Span(string.Join(", ", reszek.Where(r => !string.IsNullOrWhiteSpace(r))));
            });
        });
    }
}
