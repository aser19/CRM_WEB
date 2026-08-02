using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Rb (robbanásbiztos) berendezések "Egyedi felülvizsgálati lap" PDF-jének generálása.
/// Minden RbSor egy önálló oldalra kerül.
/// </summary>
public class RbPdfService : IRbPdfService
{
    public RbPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generalas(List<RbSor> sorok, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam)
    {
        return Document.Create(container =>
        {
            foreach (var sor in sorok)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.2f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Element(c => Fejlec(c, sor, cegNev, cegCim, cegWeb, jegyzokonyvSzam));
                    page.Content().Element(c => Tartalom(c, sor));
                    page.Footer().Element(Lablec);
                });
            }
        }).GeneratePdf();
    }

    private void Fejlec(IContainer container, RbSor sor, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(cegNev).Bold().FontSize(14);
                    c.Item().Text(cegCim).FontSize(9);
                    if (!string.IsNullOrWhiteSpace(cegWeb))
                        c.Item().Text(cegWeb).FontSize(9);
                });
                row.ConstantItem(150).AlignRight().Column(c =>
                {
                    c.Item().Text($"{jegyzokonyvSzam} {sor.Sorsz}").Bold();
                    if (!string.IsNullOrWhiteSpace(sor.CimkeSorszam))
                        c.Item().Text($"Címke sorszám: {sor.CimkeSorszam}").FontSize(9);
                });
            });

            col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);

            col.Item().AlignCenter().Text("Egyedi felülvizsgálati lap").Italic().FontSize(16);

            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem(2).Text($"Megnevezés: {sor.Megnevezes}").Bold();
                row.RelativeItem(1).Text($"Tervjel: {sor.Tervjel}");
                row.RelativeItem(2).Text($"Objektum: {sor.Objektum}");
            });

            col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
        });
    }

    private void Tartalom(IContainer container, RbSor sor)
    {
        container.PaddingTop(6).Column(col =>
        {
            col.Item().Element(c => AlapAdatok(c, sor));
            col.Item().PaddingTop(4).Element(c => TerulettBesorolas(c, sor));

            col.Item().PaddingTop(6).Element(c => ChecklistSzekcioKetOszlopos(c, "Környezeti állapotok", sor.Kornyezeti));
            col.Item().PaddingTop(6).Element(c => ChecklistSzekcioKetOszlopos(c, "A készülék vagy gyártmány állapota", sor.KeszulekAllapota));

            // Ex "i" és Ex "d" egymás mellett (két oszlopban), a mintának megfelelően
            if (sor.VanExI || sor.VanExD)
            {
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.Spacing(10);
                    if (sor.VanExI)
                        row.RelativeItem().Element(c => ChecklistSzekcio(c, "Ex \"i\" gyártmányok további követelményei", sor.ExI));
                    if (sor.VanExD)
                        row.RelativeItem().Element(c => ChecklistSzekcio(c, "Ex \"d\" gyártmányok további követelményei", sor.ExD));
                });
            }

            // Ex "m" és Ex "e" egymás mellett (két oszlopban)
            if (sor.VanExM || sor.VanExE)
            {
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.Spacing(10);
                    if (sor.VanExM)
                        row.RelativeItem().Element(c => ChecklistSzekcio(c, "Ex \"m\" gyártmányok további követelményei", sor.ExM));
                    if (sor.VanExE)
                        row.RelativeItem().Element(c => ChecklistSzekcio(c, "Ex \"e\" gyártmányok további követelményei", sor.ExE));
                });
            }

            if (sor.VanExP)
                col.Item().PaddingTop(6).Element(c => ChecklistSzekcio(c, "Ex \"p\" gyártmányok további követelményei", sor.ExP));

            col.Item().PaddingTop(6).Element(c => Eredmeny(c, sor));
            col.Item().PaddingTop(8).Element(c => Alairas(c, sor));
        });
    }

    private void AlapAdatok(IContainer container, RbSor sor)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(6).Column(col =>
        {
            col.Item().Text("KÉSZÜLÉK ADATAI").Bold().FontSize(10);
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text($"Gyártó cég: {sor.Gyarto}");
                row.RelativeItem().Text($"Típus: {sor.Tipus}");
                row.RelativeItem().Text($"Gyári szám: {sor.GyariSzam}");
            });
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem(2).Text($"Rb védelmi mód jele / VÁ. jkv. száma: {sor.VedelmiMod} {(string.IsNullOrWhiteSpace(sor.EngSzam) ? "" : $"/ {sor.EngSzam}")}");
                row.RelativeItem(1).Text($"A készülék jellemzői IP védettség: {sor.IpVedelem}");
                row.RelativeItem(1).Text($"Az év mód meglétele (szemrevételezéssel): {(sor.EvModMeglete ? "megfelelő" : "nem megfelelő")}");
            });
        });
    }

    private void TerulettBesorolas(IContainer container, RbSor sor)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(6).Column(col =>
        {
            col.Item().Text("A TERÜLET ÖVEZETEK SZERINTI BESOROLÁSA").Bold().FontSize(10);
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text($"Tűzvesz. osztály: {sor.TuzveszOsztaly}");
                row.RelativeItem().Text($"Zóna besorolás: {sor.ZonaBesorolas}");
                row.RelativeItem().Text($"Alkalmazási csop.: {sor.AlkalmazasiCsoportSzamitott}");
                row.RelativeItem().Text($"Hőmérs. osztály: {sor.HomersOsztalySzamitott}");
            });
        });
    }

    private void ChecklistSzekcio(IContainer container, string cim, List<RbCheckTetel> tetelek)
    {
        container.Column(col =>
        {
            col.Item().Text(cim).Bold().FontSize(10);
            col.Item().PaddingTop(3).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(5);
                    c.ConstantColumn(70);
                    c.RelativeColumn(3);
                });

                foreach (var tetel in tetelek)
                {
                    var hatterSzin = tetel.Megfelelt ? Colors.White : Colors.Red.Lighten4;

                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(tetel.Szoveg);
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2)
                        .Text(tetel.Megfelelt ? "megfelelő" : "nem megfelelő")
                        .FontColor(tetel.Megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(tetel.Megjegyzes ?? "");
                }
            });
        });
    }

    /// <summary>
    /// Kompakt, k\u00e9t oszlopos checklist szekci\u00f3 (c\u00edm + \u00e9rt\u00e9k soronk\u00e9nt, a mint\u00e1nak megfelel\u0151en),
    /// hogy t\u00f6bb sz\u00f6veges checklist elf\u00e9rjen egy oldalon.
    /// </summary>
    private void ChecklistSzekcioKetOszlopos(IContainer container, string cim, List<RbCheckTetel> tetelek)
    {
        container.Column(col =>
        {
            col.Item().Text(cim).Bold().FontSize(10).AlignCenter();
            col.Item().PaddingTop(3).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(4);
                    c.ConstantColumn(55);
                    c.RelativeColumn(4);
                    c.ConstantColumn(55);
                });

                var felezoPont = (tetelek.Count + 1) / 2;
                var balOldal = tetelek.Take(felezoPont).ToList();
                var jobbOldal = tetelek.Skip(felezoPont).ToList();
                var sorokSzama = Math.Max(balOldal.Count, jobbOldal.Count);

                for (var i = 0; i < sorokSzama; i++)
                {
                    RbCheckTetel? bal = i < balOldal.Count ? balOldal[i] : null;
                    RbCheckTetel? jobb = i < jobbOldal.Count ? jobbOldal[i] : null;

                    ChecklistCellaPar(table, bal);
                    ChecklistCellaPar(table, jobb);
                }
            });
        });
    }

    private void ChecklistCellaPar(TableDescriptor table, RbCheckTetel? tetel)
    {
        if (tetel is null)
        {
            table.Cell().Padding(2).Text("");
            table.Cell().Padding(2).Text("");
            return;
        }

        var hatterSzin = tetel.Megfelelt ? Colors.White : Colors.Red.Lighten4;

        table.Cell().Background(hatterSzin).Padding(2).Text(tetel.Szoveg).FontSize(8);
        table.Cell().Background(hatterSzin).Padding(2)
            .Text(tetel.Megfelelt ? "megfelelő" : "nem megfelelő")
            .Italic().FontSize(8)
            .FontColor(tetel.Megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
    }

    private void Eredmeny(IContainer container, RbSor sor)
    {
        var megfelelt = sor.VegsoMinosites?.Equals("megfelelt", StringComparison.OrdinalIgnoreCase) ?? true;
        var hatter = megfelelt ? Colors.Green.Lighten4 : Colors.Red.Lighten4;
        var szovegSzin = megfelelt ? Colors.Green.Darken3 : Colors.Red.Darken3;

        container.Background(hatter).Border(1).BorderColor(szovegSzin).Padding(6).Column(col =>
        {
            col.Item().AlignCenter().Text("A vizsgálat időpontjában a felszerelt készülék az előírt rb védelmi módnak az adott térségben:").FontSize(9);
            col.Item().AlignCenter().Text(sor.VegsoMinosites ?? "megfelelt").Bold().FontSize(14).FontColor(szovegSzin);
        });
    }

    private void Alairas(IContainer container, RbSor sor)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().AlignCenter().PaddingBottom(15).Text("________________________");
                col.Item().AlignCenter().Text(string.IsNullOrWhiteSpace(sor.VizsgalatotVegezte) ? "A vizsgálatot végezte" : sor.VizsgalatotVegezte);
                col.Item().AlignCenter().Text("felülvizsgáló").FontSize(8).FontColor(Colors.Grey.Darken1);
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
