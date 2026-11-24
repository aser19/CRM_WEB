using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Az önálló "ÁVK mérés" (Áram-védőkapcsolók működésének ellenőrzése) jegyzőkönyv PDF generálása,
/// a "VILLAMOS BERENDEZÉS JELENTÉSE / MSZ HD 60364-6:2017 - JEGYZŐKÖNYV – ÁRAM-VÉDŐKAPCSOLÓK MŰKÖDÉSÉNEK
/// ELLENŐRZÉSE" mintát követve, aláírás- és bélyegzőképpel.
/// </summary>
public class AvkPdfService : IAvkPdfService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly IFileStorageService _fileStorageService;
    private byte[]? _cegBelyegzoKep;
    private byte[]? _felulvizsgaloAlairasKep;

    public AvkPdfService(IMeresService meresService, ITenantService tenantService, ICegService cegService,
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
        {
            meres = await _meresService.GetByIdAsync(meresId);
        }

        adatok ??= new JegyzokonyvAdatok();

        var munkaszam = !string.IsNullOrWhiteSpace(adatok.Munkaszam)
            ? adatok.Munkaszam
            : $"AVK-{meresId:D6}/{DateTime.Now:yyyy}";
        var keltDatum = meres?.Datum ?? DateTime.Today;
        var avkSorok = adatok.AvkSorok ?? new List<AvkSor>();

        var jogosultsagIgazolas = "";
        try
        {
            var belyegzoCegId = adatok.CegId > 0 ? adatok.CegId : _tenantService.GetCurrentCegId();
            var ceg = await _cegService.GetByIdAsync(belyegzoCegId);
            if (!string.IsNullOrWhiteSpace(ceg?.BelyegzoPath))
            {
                _cegBelyegzoKep = await _fileStorageService.GetFileBytesAsync(ceg.BelyegzoPath);
            }
        }
        catch { /* nincs tenant kontextus vagy nincs bélyegző */ }

        if (!string.IsNullOrWhiteSpace(adatok.FelulvizsgaloNev))
        {
            try
            {
                var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();
                var felulvizsgalo = felulvizsgalok.FirstOrDefault(f => f.Nev == adatok.FelulvizsgaloNev);
                if (!string.IsNullOrWhiteSpace(felulvizsgalo?.AlairasPath))
                {
                    _felulvizsgaloAlairasKep = await _fileStorageService.GetFileBytesAsync(felulvizsgalo.AlairasPath);
                }
                if (felulvizsgalo?.Kepzesek != null && felulvizsgalo.Kepzesek.Any())
                {
                    jogosultsagIgazolas = string.Join(", ", felulvizsgalo.Kepzesek
                        .Select(k => k.BizonyitvanySzam)
                        .Where(b => !string.IsNullOrWhiteSpace(b)));
                }
            }
            catch { /* nincs aláírás / képzés */ }
        }
        if (string.IsNullOrWhiteSpace(jogosultsagIgazolas))
        {
            jogosultsagIgazolas = adatok.FelulvizsgaloBizonyitvany ?? "";
        }

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Element(c => Tartalom(c, adatok, avkSorok, munkaszam, keltDatum, jogosultsagIgazolas));
            });
        }).GeneratePdf();
    }

    private void Tartalom(IContainer container, JegyzokonyvAdatok adatok, List<AvkSor> avkSorok,
        string munkaszam, DateTime keltDatum, string jogosultsagIgazolas)
    {
        container.Border(1).BorderColor(Colors.Black).Column(col =>
        {
            // === CÍM BLOKK ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Column(c =>
            {
                c.Item().AlignCenter().Text("VILLAMOS BERENDEZÉS JELENTÉSE").Bold().FontSize(12);
                c.Item().AlignCenter().Text("MSZ HD 60364-6:2017").Bold().FontSize(9);
            });

            col.Item().BorderBottom(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(4)
                .AlignCenter().Text("JEGYZŐKÖNYV – ÁRAM-VÉDŐKAPCSOLÓK MŰKÖDÉSÉNEK ELLENŐRZÉSE").Bold().FontSize(10);

            col.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Text(text =>
            {
                text.Span("A jelentés típusa: ").SemiBold();
                text.Span(!string.IsNullOrWhiteSpace(adatok.AvkJelentesTipusa)
                    ? adatok.AvkJelentesTipusa
                    : "Használatba vételt megelőző");
            });

            // === MÉRÉST VÉGEZTÉK / MŰSZER ADATAI ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(1.3f).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text("Mérést végezték:").SemiBold();
                    c.Item().PaddingTop(2).Text(text =>
                    {
                        text.Span("Név: ").SemiBold();
                        text.Span(adatok.FelulvizsgaloNev ?? "");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Vizsgabizonyítvány(ok) száma: ").SemiBold();
                        text.Span(jogosultsagIgazolas);
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Megújító képzés(ek) száma: ").SemiBold();
                        text.Span(adatok.FelulvizsgaloKepzes ?? "");
                    });
                });
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("Név: ").SemiBold();
                        text.Span(adatok.SegitoFelulvizsgalo ?? "");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Beosztás: ").SemiBold();
                        text.Span(adatok.SegitoKepzes ?? "");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Vizsgabizonyítvány száma: ").SemiBold();
                        text.Span(adatok.SegitoBizonyitvany ?? "");
                    });
                });
                row.RelativeItem(1.4f).Padding(4).Column(c =>
                {
                    c.Item().Text("Mérésnél alkalmazott műszer adatai:").SemiBold();
                    var muszer = adatok.Muszerek?.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Tipus));
                    var tipus = muszer?.Tipus ?? adatok.Muszer1Tipus ?? "";
                    var gyariSzam = muszer?.GyariSzam ?? adatok.Muszer1GyariSzam ?? "";
                    var kalibralas = muszer?.Kalibralas ?? adatok.Muszer1Kalibralas ?? "";
                    c.Item().PaddingTop(2).Text($"Típus: {tipus}");
                    c.Item().Text($"Gyári szám: {gyariSzam}");
                    c.Item().Text($"Kalibrálási adatok: {kalibralas}");

                    if (!string.IsNullOrWhiteSpace(adatok.Ellenor))
                    {
                        c.Item().PaddingTop(4).Text(text =>
                        {
                            text.Span("Ellenőrizte: ").SemiBold();
                            text.Span(adatok.Ellenor);
                        });
                        if (!string.IsNullOrWhiteSpace(adatok.EllenorBizonyitvany))
                        {
                            c.Item().Text(text =>
                            {
                                text.Span("Bizonyítvány száma: ").SemiBold();
                                text.Span(adatok.EllenorBizonyitvany);
                            });
                        }
                    }
                });
            });

            // === ÁVK MÉRÉSI TÁBLÁZAT ===
            col.Item().Element(c => AvkTablazat(c, avkSorok));

            // === LÁBLÉC: KELT / MUNKASZÁM / FELELŐS FELÜLVIZSGÁLÓ ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text($"Kelt: {keltDatum:yyyy.MM.dd}");
                row.RelativeItem(1.3f).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text($"Munkaszám: {munkaszam}");
                row.RelativeItem(1.4f).Padding(4).Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("Felelős felülvizsgáló: ").SemiBold();
                        text.Span(adatok.FelulvizsgaloNev ?? "");
                    });
                    c.Item().Row(r =>
                    {
                        if (_felulvizsgaloAlairasKep != null)
                        {
                            r.RelativeItem().AlignCenter().Height(25).Image(_felulvizsgaloAlairasKep).FitArea();
                        }
                        if (_cegBelyegzoKep != null)
                        {
                            r.RelativeItem().AlignRight().Height(35).Image(_cegBelyegzoKep).FitArea();
                        }
                    });
                });
            });
        });
    }

    private void AvkTablazat(IContainer container, List<AvkSor> avkSorok)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(24);   // Ssz.
                c.RelativeColumn(1.1f); // Helye
                c.RelativeColumn(1.1f); // Jele
                c.RelativeColumn(1.6f); // Típus
                c.RelativeColumn(0.8f); // In
                c.RelativeColumn(0.9f); // IΔn
                c.RelativeColumn(0.8f); // Un
                c.RelativeColumn(0.9f); // Pólussz.
                c.RelativeColumn(1.1f); // IΔn mért
                c.RelativeColumn(1.3f); // t [ms] - IΔn
                c.RelativeColumn(1.3f); // t [ms] - 5xIΔn
                c.RelativeColumn(1.1f); // Minősítés
            });

            table.Header(header =>
            {
                FejlecCella(header, "Ssz.");
                FejlecCella(header, "Helye");
                FejlecCella(header, "Jele");
                FejlecCella(header, "Típus");
                FejlecCella(header, "In [A]");
                FejlecCella(header, "IΔn [mA]");
                FejlecCella(header, "Un [V]");
                FejlecCella(header, "Pólussz.");
                FejlecCella(header, "IΔn [mA]");
                FejlecCella(header, "t [ms] - IΔn");
                FejlecCella(header, "t [ms] - 5xIΔn");
                FejlecCella(header, "MINŐSÍTÉS");
            });

            var i = 0;
            foreach (var s in avkSorok)
            {
                i++;
                var hatterSzin = s.Eredmeny == "NMF" ? Colors.Red.Lighten4 : Colors.White;
                SorCella(table, i.ToString(), hatterSzin);
                SorCella(table, s.Helye, hatterSzin);
                SorCella(table, s.Jele, hatterSzin);
                SorCella(table, s.TipusNev, hatterSzin);
                SorCella(table, s.In, hatterSzin);
                SorCella(table, s.IDn, hatterSzin);
                SorCella(table, s.Un, hatterSzin);
                SorCella(table, s.Polusszam, hatterSzin);
                SorCella(table, s.IDnMertWord, hatterSzin);
                SorCella(table, s.T1xWord, hatterSzin);
                SorCella(table, s.T5xWord, hatterSzin);
                table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3)
                    .Text(s.Eredmeny ?? "").FontSize(7).SemiBold()
                    .FontColor(s.Eredmeny == "NMF" ? Colors.Red.Darken2 : Colors.Green.Darken2);
            }
        });
    }

    private void FejlecCella(TableCellDescriptor header, string szoveg)
    {
        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(szoveg).Bold().FontSize(7);
    }

    private void SorCella(TableDescriptor table, string? szoveg, string hatterSzin)
    {
        table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3)
            .Text(szoveg ?? "").FontSize(7);
    }
}
