using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

public class JegyzokonyvPdfService : IJegyzokonyvPdfService
{
    private readonly IMeresService _meresService;

    public JegyzokonyvPdfService(IMeresService meresService)
    {
        _meresService = meresService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GeneralasAsync(int meresId)
    {
        var meres = await _meresService.GetByIdAsync(meresId);
        if (meres == null) throw new ArgumentException("Mérés nem található", nameof(meresId));

        var adatok = new MeresJegyzokonyvAdatok
        {
            JegyzokonyvSzam = $"JK-{meres.Id:D6}/{DateTime.Now:yyyy}",
            KiallitasDatum = DateTime.Today,
            UgyfelNev = meres.Ugyfel?.Nev ?? "",
            UgyfelCim = meres.Ugyfel?.Cim ?? "",
            TelephelyNev = meres.Telephely?.Nev ?? "",
            TelephelyCim = meres.Telephely?.Cim ?? "",
            MeresTipusNev = meres.MeresTipus?.Nev ?? "",
            MeresDatum = meres.Datum,
            KovetkezoDatum = meres.KovetkezoDatum,
            Eredmeny = meres.Eredmeny,
            Megjegyzes = meres.Megjegyzes
        };

        return Generalas(adatok);
    }

    public byte[] Generalas(MeresJegyzokonyvAdatok adatok)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => Tartalom(c, adatok));
                page.Footer().Element(Lablec);
            });
        }).GeneratePdf();
    }

    private void Fejlec(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("BIZTVILL KFT.").Bold().FontSize(14);
                    c.Item().Text("Villamos biztonsági mérések");
                });
                row.ConstantItem(120).AlignRight().Column(c =>
                {
                    c.Item().Text($"Szám: {adatok.JegyzokonyvSzam}").Bold();
                    c.Item().Text($"Dátum: {adatok.KiallitasDatum:yyyy.MM.dd}");
                });
            });

            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            
            col.Item().AlignCenter().Text("MÉRÉSI JEGYZŐKÖNYV").Bold().FontSize(16);
            col.Item().AlignCenter().Text(adatok.MeresTipusNev).FontSize(12);
            
            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private void Tartalom(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.PaddingVertical(10).Column(col =>
        {
            // Ügyfél és telephely adatok
            col.Item().Element(c => UgyfelAdatok(c, adatok));
            
            // Mérés adatok
            col.Item().PaddingTop(15).Element(c => MeresAdatok(c, adatok));
            
            // Mért értékek táblázat (ha van)
            if (adatok.MertErtekek.Count > 0)
            {
                col.Item().PaddingTop(15).Element(c => MertErtekekTabla(c, adatok));
            }
            
            // Eredmény és megjegyzés
            col.Item().PaddingTop(20).Element(c => Eredmeny(c, adatok));
            
            // Aláírások
            col.Item().PaddingTop(30).Element(c => Alairasok(c, adatok));
        });
    }

    private void UgyfelAdatok(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(col =>
        {
            col.Item().Text("ÜGYFÉL ADATAI").Bold().FontSize(11);
            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Név: {adatok.UgyfelNev}");
                    c.Item().Text($"Cím: {adatok.UgyfelCim}");
                    if (!string.IsNullOrEmpty(adatok.UgyfelAdoszam))
                        c.Item().Text($"Adószám: {adatok.UgyfelAdoszam}");
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Telephely: {adatok.TelephelyNev}");
                    c.Item().Text($"Telephely címe: {adatok.TelephelyCim}");
                });
            });
        });
    }

    private void MeresAdatok(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(col =>
        {
            col.Item().Text("MÉRÉS ADATAI").Bold().FontSize(11);
            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text($"Típus: {adatok.MeresTipusNev}");
                row.RelativeItem().Text($"Dátum: {adatok.MeresDatum:yyyy.MM.dd}");
                row.RelativeItem().Text($"Következő: {adatok.KovetkezoDatum?.ToString("yyyy.MM.dd") ?? "-"}");
            });
        });
    }

    private void MertErtekekTabla(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().Text("MÉRT ÉRTÉKEK").Bold().FontSize(11);
            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(40);   // Sorszám
                    c.RelativeColumn(3);     // Méréspont
                    c.RelativeColumn(2);     // Mért érték
                    c.RelativeColumn(1);     // Egység
                    c.RelativeColumn(2);     // Határérték
                    c.ConstantColumn(60);    // Megfelelés
                });

                // Fejléc
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("#").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Méréspont").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mért érték").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Egység").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Határérték").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eredmény").Bold();
                });

                // Sorok
                foreach (var ertek in adatok.MertErtekek)
                {
                    var hatterSzin = ertek.Megfelelt ? Colors.White : Colors.Red.Lighten4;
                    
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(ertek.Sorszam.ToString());
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(ertek.MerespontNev);
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(ertek.Ertek ?? "-");
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(ertek.Egyseg ?? "");
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(ertek.HatarErtek ?? "-");
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4)
                        .Text(ertek.Megfelelt ? "✓" : "✗")
                        .FontColor(ertek.Megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
                }
            });
        });
    }

    private void Eredmeny(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        var megfelelt = adatok.Eredmeny == "MEGFELELT";
        var hatter = megfelelt ? Colors.Green.Lighten4 : Colors.Red.Lighten4;
        var szovegSzin = megfelelt ? Colors.Green.Darken3 : Colors.Red.Darken3;

        container.Column(col =>
        {
            col.Item().Background(hatter).Border(2).BorderColor(szovegSzin).Padding(15).Column(c =>
            {
                c.Item().AlignCenter().Text("VÉGEREDMÉNY").Bold().FontSize(12);
                c.Item().AlignCenter().Text(adatok.Eredmeny ?? "NEM ÉRTÉKELT")
                    .Bold().FontSize(18).FontColor(szovegSzin);
            });

            if (!string.IsNullOrEmpty(adatok.Megjegyzes))
            {
                col.Item().PaddingTop(10).Text("Megjegyzés:").Bold();
                col.Item().Text(adatok.Megjegyzes);
            }
        });
    }

    private void Alairasok(IContainer container, MeresJegyzokonyvAdatok adatok)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().AlignCenter().PaddingBottom(40).Text("________________________");
                col.Item().AlignCenter().Text(adatok.MeroNeve ?? "Mérést végezte");
                col.Item().AlignCenter().Text("mérőbiztos").FontSize(9).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(50); // Térköz

            row.RelativeItem().Column(col =>
            {
                col.Item().AlignCenter().PaddingBottom(40).Text("________________________");
                col.Item().AlignCenter().Text(adatok.UgyfelKepviseloNeve ?? "Ügyfél képviselője");
                col.Item().AlignCenter().Text("megrendelő").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private void Lablec(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.CurrentPageNumber();
            text.Span(" / ");
            text.TotalPages();
        });
    }
}