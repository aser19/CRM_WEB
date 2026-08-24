using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Hibavédelmi mérési jegyzőkönyv (HVM) PDF generálása.
/// A korábbi "Hibavédelem_csakmeres.docx" Word sablon vizuális elrendezését követi:
/// fejléc (cím + szabvány), mérési pontok táblázata, lábléc (Kelt / Munkaszám / Felelős felülvizsgáló).
/// </summary>
public class HvmPdfService : IHvmPdfService
{
    private readonly IMeresService _meresService;

    public HvmPdfService(IMeresService meresService)
    {
        _meresService = meresService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GeneralasAsync(int meresId, HvmAdatok adatok)
    {
        Meres? meres = null;
        if (meresId > 0)
        {
            meres = await _meresService.GetByIdAsync(meresId);
        }

        adatok ??= new HvmAdatok();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => MeresiPontokTablazat(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });
        }).GeneratePdf();
    }

    private void Fejlec(IContainer container, HvmAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().AlignCenter().Text("VILLAMOS BERENDEZÉS FELÜLVIZSGÁLATÁNAK JELENTÉSE").Bold().FontSize(11);
            col.Item().AlignCenter().Text("MSZ HD 60364-6:2017").FontSize(9);

            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text($"Jelentés típusa: {adatok.JelentesTipus}").FontSize(8);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Lapszám: ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });

            col.Item().PaddingTop(2).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private void MeresiPontokTablazat(IContainer container, HvmAdatok adatok)
    {
        container.PaddingTop(4).Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(22);   // Sorszám
                c.RelativeColumn(3.2f); // Mérési pont helye, megnevezése, egyéb közlendő adat
                c.RelativeColumn(1.1f); // Mód/Oszt.
                c.RelativeColumn(1.6f); // Kioldószerv - Túláramvédelmi szerv Helye
                c.RelativeColumn(1.6f); // Kioldószerv - Túláramvédelmi szerv Típus
                c.ConstantColumn(35);   // PE folyt.
                c.ConstantColumn(38);   // ÉRTÉK [Ω]
                c.RelativeColumn(1.3f); // MINŐSÍTÉS
            });

            table.Header(header =>
            {
                FejlecCella(header, "Sor-szám");
                FejlecCella(header, "MÉRÉSI PONT HELYE, MEGNEVEZÉSE, EGYÉB KÖZLENDŐ ADAT\n(vezeték adatai, áramkör tervjele stb.)");
                FejlecCella(header, "MÓD OSZT.");
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nHelye");
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nTípus (In, kar.)");
                FejlecCella(header, "PE folyt.");
                FejlecCella(header, "ÉRTÉK [Ω]");
                FejlecCella(header, "MINŐSÍTÉS");
            });

            foreach (var mp in adatok.MeresiPontok)
            {
                var megfelelt = string.Equals(mp.Minosites, "MEGFELELT", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(mp.Minosites, "MEGFELEL", StringComparison.OrdinalIgnoreCase);
                var hatterSzin = megfelelt ? Colors.White : Colors.Red.Lighten4;

                SorCella(table, $"{mp.Sorszam}.", hatterSzin);
                SorCella(table, mp.MeresiPontHelye, hatterSzin);
                SorCella(table, mp.Modszer, hatterSzin);
                SorCella(table, mp.TularamvedelemHelye, hatterSzin);
                SorCella(table, mp.TularamvedelemTipusa, hatterSzin);
                SorCella(table, mp.PEFolytMegfelelt ? "✓" : "✗", hatterSzin);
                SorCella(table, mp.MertHurokimpedancia?.ToString("F2") ?? mp.ErtekOhm ?? "", hatterSzin);
                table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3)
                    .Text(mp.Minosites)
                    .FontColor(megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
            }
        });
    }

    private void FejlecCella(TableCellDescriptor header, string szoveg)
    {
        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(szoveg).Bold().FontSize(7);
    }

    private void SorCella(TableDescriptor table, string szoveg, string hatterSzin)
    {
        table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(szoveg ?? "").FontSize(7);
    }

    private void Lablec(IContainer container, HvmAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text($"Kelt: {adatok.KeszitesDatum:yyyy. MMMM d.}").FontSize(8);
                row.RelativeItem().Text($"Munkaszám: {adatok.Munkaszam}").FontSize(8);
                row.RelativeItem().Text($"Felelős felülvizsgáló: {adatok.FelelosNev}").FontSize(8);
            });
        });
    }
}
