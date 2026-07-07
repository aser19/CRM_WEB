using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

public class ExportService : IExportService
{
    public ExportService()
    {
        // QuestPDF License - Community license for evaluation/non-commercial use
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Hitelesítések Export

    public async Task<byte[]> ExportHitelesitesekExcelAsync(List<Hitelesites> hitelesitesek)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hitelesítések");

            // Fejlécek
            worksheet.Cell(1, 1).Value = "Ügyfél";
            worksheet.Cell(1, 2).Value = "Telephely";
            worksheet.Cell(1, 3).Value = "Eszköz típus";
            worksheet.Cell(1, 4).Value = "Darabszám";
            worksheet.Cell(1, 5).Value = "Hitelesítés dátuma";
            worksheet.Cell(1, 6).Value = "Lejárat";
            worksheet.Cell(1, 7).Value = "Hatóság";
            worksheet.Cell(1, 8).Value = "Státusz";

            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Adatok
            int row = 2;
            foreach (var h in hitelesitesek)
            {
                worksheet.Cell(row, 1).Value = h.Ugyfel?.Nev ?? "";
                worksheet.Cell(row, 2).Value = h.Telephely?.Nev ?? "";
                worksheet.Cell(row, 3).Value = h.EszkozTipus?.Nev ?? "";
                worksheet.Cell(row, 4).Value = h.Darabszam;
                worksheet.Cell(row, 5).Value = h.Datum.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 6).Value = h.LejaratDatum?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 7).Value = h.Hatosag?.Nev ?? "";
                worksheet.Cell(row, 8).Value = h.HitelesitesStatusz.ToString();
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    public async Task<byte[]> ExportHitelesitesekPdfAsync(List<Hitelesites> hitelesitesek)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Text("Hitelesítések").FontSize(16).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ügyfél").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Telephely").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eszköz típus").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Darabszám").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hitelesítés").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Lejárat").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hatóság").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Státusz").Bold();
                        });

                        foreach (var h in hitelesitesek)
                        {
                            table.Cell().Padding(5).Text(h.Ugyfel?.Nev ?? "");
                            table.Cell().Padding(5).Text(h.Telephely?.Nev ?? "");
                            table.Cell().Padding(5).Text(h.EszkozTipus?.Nev ?? "");
                            table.Cell().Padding(5).Text(h.Darabszam.ToString());
                            table.Cell().Padding(5).Text(h.Datum.ToString("yyyy-MM-dd"));
                            table.Cell().Padding(5).Text(h.LejaratDatum?.ToString("yyyy-MM-dd") ?? "");
                            table.Cell().Padding(5).Text(h.Hatosag?.Nev ?? "");
                            table.Cell().Padding(5).Text(h.HitelesitesStatusz.ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Oldal: ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }

    #endregion

    #region Mérések Export

    public async Task<byte[]> ExportMeresekExcelAsync(List<Meres> meresek)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Mérések");

            worksheet.Cell(1, 1).Value = "Ügyfél";
            worksheet.Cell(1, 2).Value = "Telephely";
            worksheet.Cell(1, 3).Value = "Helyiség";
            worksheet.Cell(1, 4).Value = "Mérés típus";
            worksheet.Cell(1, 5).Value = "Mérés dátuma";
            worksheet.Cell(1, 6).Value = "Következő mérés";
            worksheet.Cell(1, 7).Value = "Eredmény";
            worksheet.Cell(1, 8).Value = "Státusz";

            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var m in meresek)
            {
                worksheet.Cell(row, 1).Value = m.Ugyfel?.Nev ?? "";
                worksheet.Cell(row, 2).Value = m.Telephely?.Nev ?? "";
                worksheet.Cell(row, 3).Value = m.Helyiseg?.Nev ?? "";
                worksheet.Cell(row, 4).Value = m.MeresTipus?.Nev ?? "";
                worksheet.Cell(row, 5).Value = m.Datum.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 6).Value = m.KovetkezoDatum?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 7).Value = m.Eredmeny ?? "";
                worksheet.Cell(row, 8).Value = m.MeresStatusz.ToString();
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    public async Task<byte[]> ExportMeresekPdfAsync(List<Meres> meresek)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Text("Mérések").FontSize(16).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ügyfél").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Telephely").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Helyiség").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mérés típus").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mérés dátuma").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Következő").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Eredmény").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Státusz").Bold();
                        });

                        foreach (var m in meresek)
                        {
                            table.Cell().Padding(5).Text(m.Ugyfel?.Nev ?? "");
                            table.Cell().Padding(5).Text(m.Telephely?.Nev ?? "");
                            table.Cell().Padding(5).Text(m.Helyiseg?.Nev ?? "");
                            table.Cell().Padding(5).Text(m.MeresTipus?.Nev ?? "");
                            table.Cell().Padding(5).Text(m.Datum.ToString("yyyy-MM-dd"));
                            table.Cell().Padding(5).Text(m.KovetkezoDatum?.ToString("yyyy-MM-dd") ?? "");
                            table.Cell().Padding(5).Text(m.Eredmeny ?? "");
                            table.Cell().Padding(5).Text(m.MeresStatusz.ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Oldal: ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }

    #endregion

    #region Üzemeltető Adatok Export

    public async Task<byte[]> ExportUzemeltetoAdatokExcelAsync(List<UzemeltetoAdat> adatok)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Üzemeltető adatok");

            worksheet.Cell(1, 1).Value = "Sablon";
            worksheet.Cell(1, 2).Value = "Rögzítés dátuma";
            worksheet.Cell(1, 3).Value = "Következő esedékesség";
            worksheet.Cell(1, 4).Value = "Státusz";
            worksheet.Cell(1, 5).Value = "Rögzítő";
            worksheet.Cell(1, 6).Value = "Cég";

            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var a in adatok)
            {
                worksheet.Cell(row, 1).Value = a.UzemeltetoSablon?.Nev ?? "";
                worksheet.Cell(row, 2).Value = a.RogzitesDatum.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 3).Value = a.KovetkezoEsedekesseg?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 4).Value = a.Statusz;
                worksheet.Cell(row, 5).Value = a.RogzitoFelhasznalo?.Nev ?? "";
                worksheet.Cell(row, 6).Value = a.Ceg?.Nev ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    public async Task<byte[]> ExportUzemeltetoAdatokPdfAsync(List<UzemeltetoAdat> adatok)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Text("Üzemeltető adatok").FontSize(16).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Sablon").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rögzítés").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Következő esedékes").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Státusz").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rögzítő").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cég").Bold();
                        });

                        foreach (var a in adatok)
                        {
                            table.Cell().Padding(5).Text(a.UzemeltetoSablon?.Nev ?? "");
                            table.Cell().Padding(5).Text(a.RogzitesDatum.ToString("yyyy-MM-dd"));
                            table.Cell().Padding(5).Text(a.KovetkezoEsedekesseg?.ToString("yyyy-MM-dd") ?? "");
                            table.Cell().Padding(5).Text(a.Statusz);
                            table.Cell().Padding(5).Text(a.RogzitoFelhasznalo?.Nev ?? "");
                            table.Cell().Padding(5).Text(a.Ceg?.Nev ?? "");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Oldal: ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }

    #endregion

    #region Felhasználók Export

    public async Task<byte[]> ExportFelhasznalokExcelAsync(List<Felhasznalo> felhasznalok)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Felhasználók");

            worksheet.Cell(1, 1).Value = "Név";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Beosztás";
            worksheet.Cell(1, 4).Value = "Elsődleges cég";
            worksheet.Cell(1, 5).Value = "Státusz";
            worksheet.Cell(1, 6).Value = "Utolsó belépés";

            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var f in felhasznalok)
            {
                worksheet.Cell(row, 1).Value = f.Nev ?? "";
                worksheet.Cell(row, 2).Value = f.Email ?? "";
                worksheet.Cell(row, 3).Value = f.Beosztas ?? "";
                worksheet.Cell(row, 4).Value = f.Ceg?.Nev ?? "";
                worksheet.Cell(row, 5).Value = f.Aktiv ? "Aktív" : "Inaktív";
                worksheet.Cell(row, 6).Value = f.UtolsoBelepes?.ToString("yyyy-MM-dd HH:mm") ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    public async Task<byte[]> ExportFelhasznalokPdfAsync(List<Felhasznalo> felhasznalok)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Text("Felhasználók").FontSize(16).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Név").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Email").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Beosztás").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Elsődleges cég").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Státusz").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Utolsó belépés").Bold();
                        });

                        foreach (var f in felhasznalok)
                        {
                            table.Cell().Padding(5).Text(f.Nev ?? "");
                            table.Cell().Padding(5).Text(f.Email ?? "");
                            table.Cell().Padding(5).Text(f.Beosztas ?? "");
                            table.Cell().Padding(5).Text(f.Ceg?.Nev ?? "");
                            table.Cell().Padding(5).Text(f.Aktiv ? "Aktív" : "Inaktív");
                            table.Cell().Padding(5).Text(f.UtolsoBelepes?.ToString("yyyy-MM-dd HH:mm") ?? "");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Oldal: ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }

    #endregion
}
