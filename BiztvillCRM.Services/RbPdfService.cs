using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;

namespace BiztvillCRM.Services;

/// <summary>
/// Rb (robbanásbiztos) berendezések "Egyedi felülvizsgálati lap" PDF-jének generálása.
/// Minden RbSor egy önálló oldalra kerül.
/// </summary>
public class RbPdfService : IRbPdfService
{
    private byte[]? _cegBelyegzoKep;
    private byte[]? _alairoAlairasKep;
    private Dictionary<string, byte[]>? _felulvizsgaloAlairasKepek;

    public RbPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generalas(List<RbSor> sorok, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam, string? targyLeiras = null, string? megrendeloNev = null, DateTime? keszultDatum = null,
        string? vizsgalatTipusa = null, string? vizsgalatHelyszine = null, DateTime? vizsgalatIdopontja = null,
        string? alairoNev = null, string? alairoBizonyitvany = null, string? alairoBeosztas = null,
        List<KijeloltJogszabaly>? kijeloltJogszabalyok = null, string? rbBevezetes = null, string? rbTalaltAllapotok = null,
        bool rbAtexTanusitvanyMegvan = true, bool rbVedelmiModEgyezik = true,
        bool rbVedelmiModMegfelelTersegbesorolasnak = true, bool rbAlkalmazasiCsoportHomersOsztalyMegfelelo = true,
        Dictionary<string, bool>? rbReszMinositesFelulbiralas = null, bool? rbFoMinositesFelulbiralas = null, string? rbMinositesMegjegyzes = null,
        byte[]? cegBelyegzoKep = null, byte[]? alairoAlairasKep = null, Dictionary<string, byte[]>? felulvizsgaloAlairasKepek = null)
    {
        _cegBelyegzoKep = cegBelyegzoKep;
        _alairoAlairasKep = alairoAlairasKep;
        _felulvizsgaloAlairasKepek = felulvizsgaloAlairasKepek;

        var keszult = keszultDatum ?? DateTime.Today;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Element(c => Cimlap(c, cegNev, cegCim, cegWeb, jegyzokonyvSzam, targyLeiras, megrendeloNev, keszult));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Element(c => AdatlapMasodikOldal(c, cegNev, cegCim, megrendeloNev, targyLeiras, vizsgalatTipusa,
                    vizsgalatHelyszine ?? cegCim, jegyzokonyvSzam, vizsgalatIdopontja, alairoNev, alairoBeosztas, alairoBizonyitvany, keszult));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(c => Tartalomjegyzek(c, sorok.Count));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(c => TechnologiaiTeruletekBesorolasa(c, sorok));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(c => RendeletekSzabvanyok(c, kijeloltJogszabalyok));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(c => FelulvizsgalatLeirasa(c, vizsgalatTipusa, rbBevezetes, rbTalaltAllapotok,
                    rbAtexTanusitvanyMegvan, rbVedelmiModEgyezik, rbVedelmiModMegfelelTersegbesorolasnak, rbAlkalmazasiCsoportHomersOsztalyMegfelelo));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(VedelmiModMinositesiSzempontjai);
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(c => AdatlapFejlecEgyszeru(c, cegNev, cegCim));
                page.Content().Element(c => MinositesOldal(c, sorok, rbReszMinositesFelulbiralas, rbFoMinositesFelulbiralas, rbMinositesMegjegyzes));
                page.Footer().Element(c => AdatlapLablec(c, jegyzokonyvSzam));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Header().Element(c => OsszefoglaloFejlec(c, cegNev, targyLeiras, jegyzokonyvSzam, "Hibalista"));
                page.Content().Element(c => HibalistaTablazat(c, sorok));
                page.Footer().Element(Lablec);
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Header().Element(c => OsszefoglaloFejlec(c, cegNev, targyLeiras, jegyzokonyvSzam, "Hiányosság lista"));
                page.Content().Element(c => HianyossagTablazat(c, sorok));
                page.Footer().Element(Lablec);
            });

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

    /// <summary>
    /// Az előlap: a kiállító cég adatai jobb felül, középen a dokumentum jellegének megnevezése,
    /// alatta a megrendelő neve és a vizsgálat tárgya, alul a dokumentum-azonosító és a készülés dátuma.
    /// </summary>
    private void Cimlap(IContainer container, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam, string? targyLeiras, string? megrendeloNev, DateTime keszultDatum)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem(2).AlignRight().Column(c =>
                {
                    c.Item().AlignRight().Text(cegNev).Bold().FontSize(13);
                    if (!string.IsNullOrWhiteSpace(cegCim))
                        c.Item().AlignRight().Text(cegCim).FontSize(9);
                    if (!string.IsNullOrWhiteSpace(cegWeb))
                        c.Item().AlignRight().Text(cegWeb).FontSize(9).Italic();
                });
            });

            col.Item().PaddingTop(60).AlignCenter().Text("ROBBANÁSBIZTOS BERENDEZÉSEK\nBIZTONSÁGTECHNIKAI\nFELÜLVIZSGÁLATA")
                .Bold().FontSize(18).FontColor(Colors.Black);

            if (!string.IsNullOrWhiteSpace(megrendeloNev))
                col.Item().PaddingTop(60).AlignCenter().Text(megrendeloNev).Bold().FontSize(16);

            if (!string.IsNullOrWhiteSpace(targyLeiras))
                col.Item().PaddingTop(20).AlignCenter().Text(targyLeiras).Bold().FontSize(13);

            col.Item().PaddingTop(80).AlignCenter().Text("Dokumentum-azonosító:").FontSize(9);
            col.Item().AlignCenter().Text(jegyzokonyvSzam).Bold().FontSize(14);

            col.Item().PaddingTop(20).AlignCenter().Text("Készült:").FontSize(9);
            col.Item().AlignCenter().Text($"{keszultDatum:yyyy. MMMM d.}").Bold().FontSize(14);
        });
    }

    /// <summary>
    /// A 2. oldal: a megrendelő adatai, a felülvizsgálat tárgya és típusa, a vizsgálat helyszíne/időpontja,
    /// az aláíró személy adatai, valamint alul az aláírás blokk.
    /// </summary>
    private void AdatlapMasodikOldal(IContainer container, string cegNev, string cegCim, string? megrendeloNev, string? targyLeiras,
        string? vizsgalatTipusa, string? vizsgalatHelyszine, string jegyzokonyvSzam, DateTime? vizsgalatIdopontja,
        string? alairoNev, string? alairoBeosztas, string? alairoBizonyitvany, DateTime keszultDatum)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(cegNev).Bold().FontSize(13);
                    if (!string.IsNullOrWhiteSpace(cegCim))
                        c.Item().Text(cegCim).FontSize(9);
                });
            });

            col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);

            col.Item().PaddingTop(30).Element(c => AdatSor(c, "Megrendelő neve:", megrendeloNev));
            col.Item().PaddingTop(16).Element(c => AdatSor(c, "Felülvizsgálat tárgya:", targyLeiras));
            col.Item().PaddingTop(16).Element(c => AdatSor(c, "Vizsgálat típusa:", vizsgalatTipusa));
            col.Item().PaddingTop(16).Element(c => AdatSor(c, "Vizsgálat helyszíne:", vizsgalatHelyszine));
            col.Item().PaddingTop(16).Element(c => AdatSor(c, "Jegyzőkönyv sorszáma:", jegyzokonyvSzam));
            col.Item().PaddingTop(16).Element(c => AdatSor(c, "A vizsgálat időpontja:",
                vizsgalatIdopontja.HasValue ? $"{vizsgalatIdopontja:yyyy.MM.dd.}" : null));

            var alairoSzoveg = string.IsNullOrWhiteSpace(alairoNev)
                ? null
                : string.Join("\n", new[] { alairoNev, alairoBeosztas, string.IsNullOrWhiteSpace(alairoBizonyitvany) ? null : $"biz. száma: {alairoBizonyitvany}" }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            col.Item().PaddingTop(16).Element(c => AdatSor(c, "A felülvizsgálatot végezte:", alairoSzoveg));

            col.Item().PaddingTop(40).Text($"{(string.IsNullOrWhiteSpace(cegCim) ? "" : cegCim.Split(',')[0].Trim() + ", ")}{keszultDatum:yyyy. MMMM d.}").FontSize(9);

            col.Item().PaddingTop(60).Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem().Column(c =>
                {
                    if (_alairoAlairasKep != null)
                    {
                        c.Item().AlignCenter().Height(30).Image(_alairoAlairasKep).FitArea();
                    }
                    else
                    {
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");
                    }
                    c.Item().AlignCenter().Text(alairoNev ?? "").Bold().FontSize(10);
                    c.Item().AlignCenter().Text("Auditor").FontSize(9).FontColor(Colors.Grey.Darken1);
                    if (_cegBelyegzoKep != null)
                    {
                        c.Item().AlignCenter().PaddingTop(6).Height(30).Image(_cegBelyegzoKep).FitArea();
                    }
                });
            });
        });
    }

    private void AdatSor(IContainer container, string cimke, string? ertek)
    {
        container.Row(row =>
        {
            row.RelativeItem(1).Text(cimke).Bold().Underline();
            row.RelativeItem(2).Text(ertek ?? "").FontSize(10);
        });
    }

    /// <summary>A 2-3. oldalak lábléce: oldalszám/összes oldal, jegyzőkönyv szám formátumban, a mintának megfelelően.</summary>
    private void AdatlapLablec(IContainer container, string jegyzokonyvSzam)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                text.CurrentPageNumber();
                text.Span("/");
                text.TotalPages();
                text.Span(". oldal");
            });
            row.RelativeItem().AlignRight().Text($"{jegyzokonyvSzam} sz. jegyzőkönyv").Bold().FontSize(9);
        });
    }

    /// <summary>A tartalomjegyzék oldal egyszerű fejléce: cégnév/cím balra, ExNB-stílusú elválasztó vonallal.</summary>
    private void AdatlapFejlecEgyszeru(IContainer container, string cegNev, string cegCim)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(cegNev).Bold().FontSize(13);
                    if (!string.IsNullOrWhiteSpace(cegCim))
                        c.Item().Text(cegCim).FontSize(9);
                });
            });
            col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    /// <summary>
    /// A 3. oldal: tartalomjegyzék, amely a dokumentum fő fejezeteire és a mellékletekre hivatkozik
    /// (a mellékletek oldalszáma a berendezések számától függ, ezért csak felsorolásként szerepelnek, oldalszám nélkül).
    /// </summary>
    private void Tartalomjegyzek(IContainer container, int rbSorokSzama)
    {
        container.PaddingTop(20).Column(col =>
        {
            col.Item().AlignCenter().Text("Tartalomjegyzék").Bold().FontSize(16);

            col.Item().PaddingTop(30).Column(inner =>
            {
                inner.Spacing(6);

                TartalomjegyzekSor(inner, "1.", "Megrendelő és a felülvizsgálat tárgya", 2);
                TartalomjegyzekSor(inner, "2.", "A felülvizsgálat adatai és aláírás", 2);
                TartalomjegyzekSor(inner, "3.", "A technológiai területek besorolása", 4);
                TartalomjegyzekSor(inner, "4.", "A felülvizsgálatnál alkalmazott rendeletek, szabványok, dokumentumok", 5);
                TartalomjegyzekSor(inner, "5.", "A felülvizsgálat leírása", 6);
                TartalomjegyzekSor(inner, "6.", "A védelmi mód minősítési szempontjai", 7);
                TartalomjegyzekSor(inner, "7.", "Hibalista", 8);
                TartalomjegyzekSor(inner, "8.", "Hiányosság lista", 8);
                TartalomjegyzekSor(inner, "9.", "Egyedi felülvizsgálati lapok", 9);
            });

            col.Item().PaddingTop(40).Text("Mellékletek:").Bold().FontSize(11);
            col.Item().PaddingTop(6).PaddingLeft(15).Text("Robbanásbiztonság-technikai felülvizsgálati jegyzőkönyv összesítő").Italic();
            col.Item().PaddingLeft(15).Text($"Egyedi lapok ({rbSorokSzama} db)").Italic();
        });
    }

    private void TartalomjegyzekSor(ColumnDescriptor column, string sorszam, string cim, int oldal)
    {
        column.Item().Row(row =>
        {
            row.ConstantItem(25).Text(sorszam).Bold();
            row.RelativeItem().Text(text =>
            {
                text.Span(cim);
                text.Span(" ");
                text.Span(new string('.', 80)).FontColor(Colors.Grey.Lighten1);
            });
            row.ConstantItem(20).AlignRight().Text(oldal.ToString());
        });
    }

    /// <summary>
    /// A 4. oldal: "A technológiai területek besorolása" fejezet. A helyiségenkénti/berendezésenkénti
    /// besorolási adatok az RbSor-okból kerülnek kigyűjtésre (elhelyezés/terület, tűzveszélyességi osztály,
    /// robbanásveszélyes tér fajtája [zóna], alkalmazási csoport, hőmérsékleti osztály), a hőmérsékleti osztályok
    /// magyarázata (3. kép) pedig fix, nem szerkeszthető tartalomként jelenik meg. A szabványok/rendeletek
    /// szekció külön oldalra kerül majd (később megadott tartalom alapján).
    /// </summary>
    private void TechnologiaiTeruletekBesorolasa(IContainer container, List<RbSor> sorok)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Text("1. A technológiai területek besorolása").Bold().FontSize(14);

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    void FejlecCella(string szoveg) => header.Cell().Background(Colors.Yellow.Medium).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(szoveg).Bold();
                    FejlecCella("Terület megnevezése");
                    FejlecCella("Tűzveszélyességi osztály (régi OTSZ szerint)");
                    FejlecCella("Robbanásveszélyes tér fajtája");
                    FejlecCella("Alkalmazási csoport");
                    FejlecCella("Hőmérsékleti osztály");
                });

                var terulentenkent = sorok
                    .GroupBy(s => string.IsNullOrWhiteSpace(s.Elhelyezes) ? s.Objektum : s.Elhelyezes)
                    .Where(g => !string.IsNullOrWhiteSpace(g.Key));

                foreach (var terulet in terulentenkent)
                {
                    var elso = terulet.First();
                    void Cella(string szoveg) => table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(szoveg);
                    Cella(terulet.Key);
                    Cella(string.IsNullOrWhiteSpace(elso.TuzveszOsztaly) ? "-" : elso.TuzveszOsztaly);
                    Cella(string.IsNullOrWhiteSpace(elso.ZonaBesorolas) ? "-" : elso.ZonaBesorolas);
                    Cella(string.IsNullOrWhiteSpace(elso.AlkalmazasiCsoportSzamitott) ? "-" : elso.AlkalmazasiCsoportSzamitott);
                    Cella(string.IsNullOrWhiteSpace(elso.HomersOsztalySzamitott) ? "-" : elso.HomersOsztalySzamitott);
                }
            });

            col.Item().PaddingTop(16).Element(HomersOsztalyokMagyarazata);
        });
    }

    /// <summary>
    /// A hőmérsékleti osztályok fix, nem szerkeszthető magyarázó táblázata (3. kép). A táblázat tartalma
    /// az ATEX szabvány szerinti T1-T6 osztályok maximális felületi hőmérsékleteit sorolja fel.
    /// </summary>
    private void HomersOsztalyokMagyarazata(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("A táblázatban szereplő hőmérsékleti osztályok magyarázata:").Bold().FontSize(10);

            col.Item().PaddingTop(4).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Yellow.Medium).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text("Hőmérsékleti osztály [ATEX]").Bold();
                    header.Cell().Background(Colors.Yellow.Medium).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text("Maximum felületi hőmérséklet [°C]").Bold();
                });

                (string Osztaly, string MaxHomerseklet)[] sorok =
                {
                    ("T1", "450 (440)"),
                    ("T2", "300 (290)"),
                    ("T3", "200 (195)"),
                    ("T4", "135 (130)"),
                    ("T5", "100 (95)"),
                    ("T6", "85 (80)"),
                };

                foreach (var (osztaly, maxHo) in sorok)
                {
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignCenter().Text(osztaly);
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignCenter().Text(maxHo);
                }
            });

            col.Item().PaddingTop(6).Text("A tűzveszélyességi osztály a 28/2011. (IX. 6.) BM rendelet (régi OTSZ) és az 54/2014 (XII.5) BM rendelet (új OTSZ) alapján került meghatározásra.").FontSize(8).Italic();
        });
    }

    /// <summary>
    /// A felülvizsgálatnál alkalmazott rendeletek és szabványok fejezete (2.1 Rendeletek, 2.2 Szabványok).
    /// A megjelenő tételek a jegyzőkönyv kitöltésekor kiválasztott, "Kivalasztva" jelölésű jogszabályok/szabványok
    /// (lásd <see cref="KijeloltJogszabaly"/>), amelyeket a jegyzőkönyv 1. oldalán az RB-hez kapcsolódó
    /// (taggel megjelölt) törzsadatok közül lehet kiválasztani.
    /// </summary>
    private void RendeletekSzabvanyok(IContainer container, List<KijeloltJogszabaly>? kijeloltJogszabalyok)
    {
        var tetelek = kijeloltJogszabalyok?.Where(j => j.Kivalasztva).ToList() ?? new List<KijeloltJogszabaly>();
        var rendeletek = tetelek.Where(j => !j.IsSzabvany).OrderBy(j => j.Szam).ToList();
        var szabvanyok = tetelek.Where(j => j.IsSzabvany).OrderBy(j => j.Szam).ToList();

        container.PaddingTop(10).Column(col =>
        {
            col.Item().Text("2. A felülvizsgálatnál alkalmazott rendeletek, szabványok, dokumentumok").Bold().FontSize(14);

            col.Item().PaddingTop(14).Text("2.1 Rendeletek").Bold().FontSize(12).Underline();
            if (rendeletek.Count == 0)
            {
                col.Item().PaddingTop(4).Text("Nincs kiválasztott rendelet.").Italic().FontSize(9);
            }
            else
            {
                foreach (var r in rendeletek)
                    col.Item().PaddingTop(8).Element(c => JogszabalyTetel(c, r.Szam, r.Cim));
            }

            col.Item().PaddingTop(16).Text("2.2 Szabványok").Bold().FontSize(12).Underline();
            if (szabvanyok.Count == 0)
            {
                col.Item().PaddingTop(4).Text("Nincs kiválasztott szabvány.").Italic().FontSize(9);
            }
            else
            {
                foreach (var s in szabvanyok)
                    col.Item().PaddingTop(8).Element(c => JogszabalyTetel(c, s.Szam, s.Cim));
            }

            col.Item().PaddingTop(16).Text("A vonatkozó előírások fejezet \"szabványok\" részében a felülvizsgálat során figyelembe vett alapvető szabványokat soroltuk fel, az adott területre esetleg vonatkozó speciális műszaki előírásokat azok nagy száma miatt külön megnevezésük nélkül is figyelembe vettük.")
                .FontSize(8).Italic();
        });
    }

    /// <summary>
    /// A "felülvizsgálat leírása" fejezet: 3.1 Bevezetés és 3.2 A talált állapotok leírása, a jegyzőkönyv
    /// kitöltésekor megadott (vagy alapértelmezett) szabad szöveggel.
    /// </summary>
    private void FelulvizsgalatLeirasa(IContainer container, string? vizsgalatTipusa, string? rbBevezetes, string? rbTalaltAllapotok,
        bool atexTanusitvanyMegvan, bool vedelmiModEgyezik, bool vedelmiModMegfelelTersegbesorolasnak, bool alkalmazasiCsoportHomersOsztalyMegfelelo)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Text("3. A felülvizsgálat leírása").Bold().FontSize(14);

            if (!string.IsNullOrWhiteSpace(vizsgalatTipusa))
            {
                col.Item().PaddingTop(10).Text("Vizsgálat típusa").Bold().FontSize(11);
                col.Item().PaddingTop(2).Text(vizsgalatTipusa).FontSize(9);
            }

            col.Item().PaddingTop(16).Text("3.1 Bevezetés").Bold().FontSize(12).Underline();
            col.Item().PaddingTop(6).Text(string.IsNullOrWhiteSpace(rbBevezetes) ? AlapertelmezettBevezetes : rbBevezetes).FontSize(9);

            col.Item().PaddingTop(16).Text("3.2 A talált állapotok leírása").Bold().FontSize(12).Underline();
            col.Item().PaddingTop(6).Text(string.IsNullOrWhiteSpace(rbTalaltAllapotok) ? AlapertelmezettTalaltAllapotok : rbTalaltAllapotok).FontSize(9);

            col.Item().PaddingTop(16).Text("3.3 A rendeltetés megfelelősségének vizsgálata").Bold().FontSize(12).Underline();

            col.Item().PaddingTop(8).Text("3.3.1 Dokumentációk vizsgálata").Italic().FontSize(10);

            col.Item().PaddingTop(6).Element(c => IgenNemKerdes(c,
                "A gyártmány rendelkezik-e ATEX jelzésű tanúsítvánnyal, vagy gyártói nyilatkozattal?", atexTanusitvanyMegvan));
            col.Item().PaddingTop(6).Element(c => IgenNemKerdes(c,
                "A gyártmány dokumentációjában feltüntetett védelmi mód egyezik-e az adattáblán szereplő védelmi móddal?", vedelmiModEgyezik));
            col.Item().PaddingTop(6).Element(c => IgenNemKerdes(c,
                "A védelmi mód megfelel-e az adatlapon megjelölt térségbesorolásnak?", vedelmiModMegfelelTersegbesorolasnak));
            col.Item().PaddingTop(6).Element(c => IgenNemKerdes(c,
                "A gyártmány alkalmazási csoportja és hőmérsékleti osztálya megfelelő-e?", alkalmazasiCsoportHomersOsztalyMegfelelo));

            col.Item().PaddingTop(16).Text("3.3.2 A működőképesség felülvizsgálati szempontjai").Italic().FontSize(10);
            col.Item().PaddingTop(6).Text("A vizsgálat során – minden védelmi módra - ellenőriztük az alábbiakat:").FontSize(9);
            col.Item().PaddingTop(6).Element(c => FelsorolasSor(c, "tokozás zártsága, épsége,"));
            col.Item().Element(c => FelsorolasSor(c, "a jelöléseket, felirati táblákat,"));
            col.Item().Element(c => FelsorolasSor(c, "nincs-e szemmel láthatóan jogosulatlan módosítás,"));
            col.Item().Element(c => FelsorolasSor(c, "a fel nem használt bevezető nyílások szabvány szerinti lezárása,"));
            col.Item().Element(c => FelsorolasSor(c, "a felszerelt tömszelencék megfelelő tömítettségét,"));
            col.Item().Element(c => FelsorolasSor(c, "a villamos csatlakozókat (sorkapcsokat), villamos szigetelőanyagokat,"));
            col.Item().Element(c => FelsorolasSor(c, "az alkalmazott vezetéket, az azokhoz csatlakozó áramkörök azonosíthatóságát,"));
            col.Item().Element(c => FelsorolasSor(c, "a földelővezetékek meglétét, állapotát."));

            col.Item().PaddingTop(16).Text("3.3.3 A környezet és az alkalmazási körülmények").Italic().FontSize(10);
            col.Item().PaddingTop(6).Text("Szemrevételezéssel ellenőriztük az alábbi szempontok szerint: üzemi igénybevétel, szennyeződés, por lerakódás mértéke, korrózió, melegedés, káros rezgések, sztatikus töltődés felhalmozódásának lehetősége.")
                .FontSize(9);
            col.Item().PaddingTop(6).Text("A berendezések szabadtéren és zárt térben kerülnek felhasználásra, a vizsgálat ideje alatt a működést későbbiekben károsan befolyásoló tényezőket nem tapasztaltunk.")
                .FontSize(9);

            col.Item().PaddingTop(16).Text("3.3.4 Tömítettség és szigetelési állapotok ellenőrzése").Italic().FontSize(10);
            col.Item().PaddingTop(6).Text("Szigetelési állapot ellenőrzése körébe tartoztak: az üzemszerűen feszültség alatt álló részek – csatlakozó kapocsházak, kábelek, vezetékek üzemi szigetelése, az egyes elemek (tömszelencék, sorkapcsok) – ellenőrzése.")
                .FontSize(9);
            col.Item().PaddingTop(6).Text("A vizsgálat idején a készülékekben nedvességet, port, vagy egyéb szennyeződés behatolását nem tapasztaltuk.")
                .FontSize(9);

            col.Item().PaddingTop(16).Text("3.3.5 Installációra vonatkozó ellenőrzések").Italic().FontSize(10);
            col.Item().PaddingTop(6).Text("A kiépítésre, védelemre vonatkozó ellenőrzéseket úgymint:").FontSize(9);
            col.Item().PaddingTop(6).Element(c => FelsorolasSor(c, "vezeték típusának megfelelősége,"));
            col.Item().Element(c => FelsorolasSor(c, "a vezeték épsége,"));
            col.Item().Element(c => FelsorolasSor(c, "a vezetékcsatornák, profilsövek, telex sínek épsége, méretezése,"));
            col.Item().Element(c => FelsorolasSor(c, "túláram védelmek."));
        });
    }

    /// <summary>Egy kötőjeles felsorolás sor megjelenítése (pl. "3.3.2" és "3.3.5" fejezetek listás elemei).</summary>
    private void FelsorolasSor(IContainer container, string szoveg)
    {
        container.PaddingLeft(10).Row(row =>
        {
            row.ConstantItem(12).Text("-").FontSize(9);
            row.RelativeItem().Text(szoveg).FontSize(9);
        });
    }

    /// <summary>
    /// A "3.4 A védelmi mód minősítési szempontjai" fejezet: a 3.4.1 "Felülvizsgálati program az Ex"d", Ex"e", Ex"n" és
    /// Ex"t/tD" berendezésekhez" táblázat, fix (nem szerkeszthető) tartalommal, az MSZ EN 60079-17:2014 szabvány szerint.
    /// Oszlopok: Ex"d" (R/K/S), Ex"e" (R/K/S), Ex"n" és Ex"t/tD" (R/K/S).
    /// </summary>
    private void VedelmiModMinositesiSzempontjai(IContainer container)
    {
        // Minden sor: (sorszám, leírás, [Ex"d" R,K,S, Ex"e" R,K,S, Ex"n"/Ex"t/tD" R,K,S])
        var sorok = new (string Szam, string Leiras, string[] Jelek)[]
        {
            ("A", "Általános (minden gyártmányra vonatkozik)", Array.Empty<string>()),
            ("1", "A gyártmány megfelel a térségbesorolásnak, EPL követelménynek", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("2", "Az alkalmazási csoport megfelelő", new[] { "X","","X", "X","","X", "","","" }),
            ("3", "A gyártmány hőmérsékleti osztálya megfelelő", new[] { "X","X","", "X","X","", "n","n","" }),
            ("4", "A gyártmány maximális felületi hőmérséklete megfelelő", new[] { "","","", "","","", "t","t","" }),
            ("5", "A gyártmány IP védettsége megfelel a védelmi mód, készülék kategória, védettőképesség követelményeinek", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("6", "A gyártmány áramköreinek azonosítása megfelelő", new[] { "X","","", "X","","", "","","" }),
            ("7", "A gyártmány áramköreinek azonosítói rendelkezésre állnak", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("8", "A tokozás, az üvegrészek és az üveg-fém részek tömítései és/vagy tömítőanyagai kielégítők", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("9", "Nincs jogosulatlan módosítás", new[] { "X","","", "X","","", "X","","" }),
            ("10", "Nincs szemmel látható jogosulatlan módosítás", new[] { "","X","X", "","X","X", "","X","X" }),
            ("11", "Csavarok, vezetékbevezető eszközök (közvetlen és közvetett) és lezáró elemek megfelelő típusúak, sértetlenek és nincsenek kilazulva", Array.Empty<string>()),
            ("", "-  fizikai ellenőrzés", new[] { "X","X","", "X","X","", "X","X","" }),
            ("", "-  szemrevételezés", new[] { "","","X", "","","X", "","","X" }),
            ("12", "A tokozat záróösvarjai megfelelő típusúak, lezárásuk szoros és biztonságos", Array.Empty<string>()),
            ("", "-  fizikai ellenőrzés", new[] { "X","X","", "X","X","", "X","X","" }),
            ("", "-  szemrevételezés", new[] { "","","X", "","","X", "","","X" }),
            ("13", "A peremek csatlakozó felülete tiszta és sértetlen és a tömítések, ha vannak kielégítők", new[] { "X","X","", "X","X","", "X","X","" }),
            ("14", "A tokozat illeszkedő felületei megfelelő állapotúak", new[] { "","","X", "","X","", "","X","" }),
            ("15", "Por vagy víz behatolásának nincs nyoma a tokozatban", new[] { "X","","X", "X","","X", "X","","X" }),
            ("16", "Az átgyújtásbiztos rések mérete", Array.Empty<string>()),
            ("", "- a gyártói dokumentációban szereplő határokon belül van, vagy", new[] { "X","","", "","","", "","","" }),
            ("", "- telepítéskor a vonatkozó szabvány megengedett maximum értékein belül van, vagy", new[] { "X","","", "","","", "","","" }),
            ("", "- az üzemi dokumentációk szerint megengedett maximum értékeken belül van", new[] { "X","","", "","","", "","","" }),
            ("17", "A villamos csatlakozások szorosak", new[] { "","X","", "","X","", "X","","" }),
            ("18", "A nem használt csatlakozások szorosan rögzítettek", new[] { "","X","", "","X","", "n","","" }),
            ("19", "A tokozott és hermetikusan lezárt berendezések sértetlenek", new[] { "","","", "","X","", "n","","" }),
            ("20", "A kiöntött berendezések részek sértetlenek", new[] { "","","", "","X","", "n","","" }),
            ("21", "A nyomásálló berendezés részek sértetlenek", new[] { "","","", "","X","", "n","","" }),
            ("22", "A kigőzölgésbiztos tokozat kielégítő – (csak „nR” esetén)", new[] { "","","", "","","", "n","","" }),
            ("23", "Vizsgáló nyílás – ha van – megfelelő – (csak „nR” esetén)", new[] { "","","", "","","", "n","","" }),
            ("24", "A szellőző funkció kielégítő – (csak „nR” esetén)", new[] { "X","","", "X","","", "n","","" }),
            ("25", "A légzsák és leeresztő elemek kielégítőek", new[] { "X","X","", "X","X","", "n","n","" }),
            ("§", "Világítási berendezések", Array.Empty<string>()),
            ("26", "Fénycsöves lámpatestek esetén az EOL jelenség kizárt", new[] { "","","", "X","X","X", "X","X","X" }),
            ("27", "HID lámpatestek esetén az EOL jelenség kizárt", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("28", "Fényforrás típusa, teljesítménye, bekötése megfelelő", new[] { "X","","", "X","","", "X","","" }),
            ("§", "Motorok", Array.Empty<string>()),
            ("29", "A motorventillátorok távolsága a tokozástól és/vagy borításoktól elegendő, a hűtési rendszer sértetlen, a motor rögzítése látható sérüléstől mentes", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("30", "A hűtési levegőáramlás nem akadályozott", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("31", "A motortekercselés szigetelési ellenállása kielégítő", new[] { "X","","", "X","","", "X","","" }),
        };

        var telepitesAltalanos = new (string Szam, string Leiras, string[] Jelek)[]
        {
            ("1", "A vezeték típusa megfelelő", new[] { "X","","", "X","","", "X","","" }),
            ("2", "A vezetéken nincs szemmel látható sérülés", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("3", "A vezetékcsatornák, profilsövek, csövek és/vagy védőcsövek tömítése kielégítő", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("4", "A végzáró- és vezetékdobozok kiöntése megfelelő", new[] { "X","","", "","","", "","","" }),
            ("5", "A védőcsőrendszer és a vegyes rendszerrel összekötő elemek sértetlenek", new[] { "X","","", "X","","", "X","","" }),
            ("6", "A földelés csatlakozásai, beleértve bármely helyi (kiegészítő) földelés csatlakozásait is, kielégítők", Array.Empty<string>()),
            ("", "-  fizikai ellenőrzés", new[] { "X","","", "X","","", "X","","" }),
            ("", "-  szemrevételezés", new[] { "","X","X", "","X","X", "","X","X" }),
            ("7", "A zárlati hurokimpedancia (TN-rendszerek) vagy a földelési ellenállás (IT-rendszerek) kielégítő", new[] { "X","","", "X","","", "X","","" }),
            ("8", "Az önműködő villamos védelmi eszközök beállítása megfelelő", new[] { "X","","", "X","","", "X","","" }),
            ("9", "Az önműködő villamos védelmi eszközök a megengedett értékhatáron belül működnek", new[] { "X","","", "X","","", "X","","" }),
            ("10", "Az alkalmazás különleges feltételei (ha vannak) teljesülnek", new[] { "X","","", "X","","", "X","","" }),
            ("11", "A használaton kívüli vezetékek végzárása megfelelő", new[] { "X","","", "X","","", "X","","" }),
            ("12", "A nyomásálló peremek illeszkedő felületeihez közeli akadályok megfelelnek az IEC 60079-14-nek", new[] { "X","X","X", "","","", "","","" }),
            ("13", "Szabályozható feszültség/frekvencia berendezés a dokumentáció szerinti", new[] { "X","X","", "X","X","", "X","X","" }),
        };

        var telepitesFutesiRendszerek = new (string Szam, string Leiras, string[] Jelek)[]
        {
            ("14", "Hőmérséklet érzékelők működése a gyártói dokumentáció szerinti", new[] { "X","","", "X","","", "t","","" }),
            ("15", "A biztonsági lekapcsoló berendezések működése a gyártói dokumentáció szerinti", new[] { "X","","", "X","","", "t","","" }),
            ("16", "A biztonsági lekapcsoló berendezések elzártak", new[] { "X","X","", "X","X","", "","","" }),
            ("17", "A biztonsági lekapcsoló berendezések visszakapcsolása csak szerszámmal lehetséges", new[] { "X","X","", "X","X","", "","","" }),
            ("18", "Önműködő újraindulás nem lehetséges", new[] { "X","X","", "X","X","", "","","" }),
            ("19", "A biztonsági lekapcsoló berendezések visszakapcsolása hiba esetén lehetetlen", new[] { "X","","", "X","","", "","","" }),
            ("20", "A biztonsági lekapcsolás független a vezérlő rendszertől", new[] { "X","","", "X","","", "","","" }),
            ("21", "Amennyiben szintkapcsoló van beépítve, megfelelően van beállítva", new[] { "X","","", "X","","", "","","" }),
            ("22", "Amennyiben áramláskapcsoló van beépítve, megfelelően van beállítva", new[] { "X","","", "X","","", "","","" }),
        };

        var telepitesMotorok = new (string Szam, string Leiras, string[] Jelek)[]
        {
            ("23", "A motorvédelmi eszközök a megengedett tE vagy tA idők között működnek", new[] { "","","", "","","", "X","","" }),
        };

        var kornyezet341 = new (string Szam, string Leiras, string[] Jelek)[]
        {
            ("1", "A gyártmány megfelelően védett a korróziótól, az időjárás hatásaitól, rezgéstől és más káros tényezőktől", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("2", "A gyártmányon nincs káros por- és más szennyeződés-lerakódás", new[] { "X","X","X", "X","X","X", "X","X","X" }),
            ("3", "A villamos szigetelés tiszta és száraz", new[] { "","","", "X","","", "X","","" }),
        };

        var mindenSor = new List<(string Szam, string Leiras, string[] Jelek)>();
        mindenSor.AddRange(sorok);
        mindenSor.Add(("B", "Telepítés - Általános", Array.Empty<string>()));
        mindenSor.AddRange(telepitesAltalanos);
        mindenSor.Add(("§", "Telepítés – Fűtési rendszerek", Array.Empty<string>()));
        mindenSor.AddRange(telepitesFutesiRendszerek);
        mindenSor.Add(("§", "Telepítés - Motorok", Array.Empty<string>()));
        mindenSor.AddRange(telepitesMotorok);
        mindenSor.Add(("C", "Környezet", Array.Empty<string>()));
        mindenSor.AddRange(kornyezet341);

        container.PaddingTop(6).Column(col =>
        {
            col.Item().Text("3.4 A védelmi mód minősítési szempontjai").Bold().FontSize(14);

            col.Item().PaddingTop(10).Text("A különböző védelmi módok felülvizsgálati követelményeit részletesen, táblázatos formában az MSZ EN 60079-17:2014-es szabvány tartalmazza.")
                .FontSize(9);

            col.Item().PaddingTop(10).Text("3.4.1 Felülvizsgálati program az Ex„d”, Ex„e”, Ex„n” és Ex„t/tD” berendezésekhez").Italic().FontSize(10);

            col.Item().PaddingTop(6).Text("X – minden védelmi módra vonatkozik      n – csak Ex „n” védelmi mód esetén      t – csak Ex „t” védelmi mód esetén").FontSize(8.5f);

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(20);   // Sorsz.
                    c.RelativeColumn(6);    // Leírás
                    c.ConstantColumn(20); c.ConstantColumn(20); c.ConstantColumn(20); // Ex"d" R K S
                    c.ConstantColumn(20); c.ConstantColumn(20); c.ConstantColumn(20); // Ex"e" R K S
                    c.ConstantColumn(20); c.ConstantColumn(20); c.ConstantColumn(20); // Ex"n"/t R K S
                });

                table.Header(header =>
                {
                    header.Cell().RowSpan(2).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignMiddle().Text("").FontSize(8);
                    header.Cell().RowSpan(2).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignMiddle().Text("A felülvizsgálat fokozata*").FontSize(8);
                    header.Cell().ColumnSpan(3).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignCenter().Text("Ex„d”").Bold().FontSize(8.5f);
                    header.Cell().ColumnSpan(3).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignCenter().Text("Ex„e”").Bold().FontSize(8.5f);
                    header.Cell().ColumnSpan(3).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignCenter().Text("Ex„n” és\nEx„t/tD”").Bold().FontSize(7.5f);

                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("R").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("K").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("S").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("R").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("K").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("S").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("R").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("K").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("S").Bold().FontSize(8.5f);
                });

                foreach (var (szam, leiras, jelek) in mindenSor)
                {
                    var szekcioFejlec = szam is "A" or "B" or "C" or "§";
                    var vastag = szekcioFejlec;

                    if (szekcioFejlec)
                    {
                        table.Cell().ColumnSpan(11).Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Medium).Padding(3)
                            .Text(t =>
                            {
                                if (szam != "§") t.Span(szam + " ").Bold().FontSize(8.5f);
                                t.Span(leiras).Bold().FontSize(8.5f);
                            });
                        continue;
                    }

                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignMiddle()
                        .Text(t => { var s = t.Span(szam).FontSize(8); if (vastag) s.Bold(); });
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignMiddle()
                        .Text(t => { var s = t.Span(leiras).FontSize(8); if (vastag) s.Bold(); });

                    for (var i = 0; i < 9; i++)
                    {
                        var jel = i < jelek.Length ? jelek[i] : "";
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text(jel).FontSize(8).Bold();
                    }
                }
            });

            col.Item().PaddingTop(4).Text("* : R=részletes, K=közeli, S=szemrevételezéses").FontSize(7.5f).Italic();

            col.Item().PaddingTop(16).Text("3.4.2 Felülvizsgálati program az Ex„i” berendezésekhez").Italic().FontSize(10);

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(20);   // Sorsz.
                    c.RelativeColumn(6);    // Leírás
                    c.ConstantColumn(24); c.ConstantColumn(24); c.ConstantColumn(24); // R K SZ
                });

                table.Header(header =>
                {
                    header.Cell().RowSpan(2).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2).Text("").FontSize(8);
                    header.Cell().RowSpan(2).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2).Text("").FontSize(8);
                    header.Cell().ColumnSpan(3).Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(2)
                        .AlignCenter().Text("A felülvizsgálat fokozata*").Bold().FontSize(8.5f);

                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("R").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("K").Bold().FontSize(8.5f);
                    header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).AlignCenter().Text("SZ").Bold().FontSize(8.5f);
                });

                var exiSorok = new (string Szam, string Leiras, string[] Jelek)[]
                {
                    ("A", "Gyártmány", Array.Empty<string>()),
                    ("1", "Az áramkör és/vagy a gyártmány dokumentációja megfelel a térségbesorolásnak/EPL követelménynek", new[] { "X","X","X" }),
                    ("2", "A beszerelt gyártmány megfelel a dokumentációban előírtaknak – csak a helyhez kötött gyártmányok esetében", new[] { "X","X","" }),
                    ("3", "Az áramkör és/vagy a gyártmány kategóriája és alkalmazási csoportja megfelelő", new[] { "X","X","" }),
                    ("4", "A készülék IP védelme megfelel a III főcsoportnak", new[] { "X","X","" }),
                    ("5", "A gyártmány hőmérsékleti osztálya megfelelő", new[] { "X","X","" }),
                    ("6", "A készülék környezeti hőmérséklet tartománya az alkalmazásnak megfelelő", new[] { "X","X","" }),
                    ("7", "A készülék szervíz hőmérséklet tartománya az alkalmazásnak megfelelő", new[] { "X","X","" }),
                    ("8", "A berendezés címkézése azonosítható", new[] { "X","X","" }),
                    ("9", "A tokozás, az üvegrészek és az üveg-fém részek tömítései és/vagy tömítőanyagai kielégítők", new[] { "X","X","" }),
                    ("10", "Vezetékbevezető eszközök (közvetlen és közvetett) és lezáró elemek megfelelő típusúak, sértetlenek és nincsenek kilazulva", Array.Empty<string>()),
                    ("", "- fizikai ellenőrzés", new[] { "X","X","X" }),
                    ("", "- szemrevételezés", new[] { "","","" }),
                    ("11", "Nincs jogosulatlan módosítás", new[] { "X","","" }),
                    ("12", "Nincs szemmel látható jogosulatlan módosítás", new[] { "","X","" }),
                    ("13", "A gyújtószikragátak, galvanikus leválasztók, relék és más energiahatároló eszközök típusa azonos a jóváhagyottal, a létesítésük megfelel a tanúsítási követelményeknek és szükség szerint biztonságosan földeltek", new[] { "X","X","" }),
                    ("14", "A tokozat tömítései kielégítők", new[] { "X","","" }),
                    ("15", "A villamos csatlakozások tiszták", new[] { "X","","" }),
                    ("16", "A nyomtatott áramköri kártyák tiszták és sértetlenek", new[] { "X","","" }),
                    ("17", "A csatlakozó berendezés az Um maximális feszültséget nem haladja meg", new[] { "X","X","" }),
                    ("B", "Telepítés", Array.Empty<string>()),
                    ("1", "A vezetékek létesítése megfelel a dokumentációnak", new[] { "X","","" }),
                    ("2", "A vezetékárnyékolások a dokumentációnak megfelelően földeltek", new[] { "X","","" }),
                };

                foreach (var (szam, leiras, jelek) in exiSorok)
                {
                    var szekcioFejlec = szam is "A" or "B";
                    if (szekcioFejlec)
                    {
                        table.Cell().ColumnSpan(5).Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Medium).Padding(3)
                            .Text(t => { t.Span(szam + " ").Bold().FontSize(8.5f); t.Span(leiras).Bold().FontSize(8.5f); });
                        continue;
                    }

                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignMiddle().Text(szam).FontSize(8);
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignMiddle().Text(leiras).FontSize(8);

                    for (var i = 0; i < 3; i++)
                    {
                        var jel = i < jelek.Length ? jelek[i] : "";
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text(jel).FontSize(8).Bold();
                    }
                }
            });
        });
    }

    /// <summary>Egy Igen/Nem kérdés megjelenítése checkbox-szerű jelöléssel, a jegyzőkönyv-minta szerint.</summary>
    private void IgenNemKerdes(IContainer container, string kerdes, bool igen)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                text.Span("-  ");
                text.Span(kerdes).FontSize(9);
            });
            row.ConstantItem(70).Row(r2 =>
            {
                r2.ConstantItem(14).Height(14).Border(1).BorderColor(Colors.Black)
                    .AlignCenter().AlignMiddle().Text(igen ? "X" : "").FontSize(9).Bold();
                r2.ConstantItem(10);
                r2.AutoItem().AlignMiddle().Text("Igen").FontSize(9);
            });
            row.ConstantItem(70).Row(r2 =>
            {
                r2.ConstantItem(14).Height(14).Border(1).BorderColor(Colors.Black)
                    .AlignCenter().AlignMiddle().Text(!igen ? "X" : "").FontSize(9).Bold();
                r2.ConstantItem(10);
                r2.AutoItem().AlignMiddle().Text("Nem").FontSize(9);
            });
        });
    }

    private const string AlapertelmezettBevezetes =
        "Ezen jegyzőkönyv a felülvizsgálat tárgyát képező tartály/berendezés elemeiként felszerelt robbanásbiztos villamos és nem-villamos berendezések robbanásbiztonság-technikai felülvizsgálatát foglalja össze.\n\n" +
        "A vizsgálat célja annak megállapítása, hogy a robbanásbiztos készülékek teljesítik-e az adattáblájukon, a gyártói nyilatkozatokban rögzített műszaki paramétereket.\n\n" +
        "A vizsgálat tárgyát a jegyzőkönyv végén található táblázatban szereplő készülékek egyedi felülvizsgálata, illetve a hozzá tartozó installáció jelentette.";

    private const string AlapertelmezettTalaltAllapotok =
        "Vizsgálatunk tárgyát a robbanásbiztos kivitelű berendezések robbanásbiztonság-technikai felülvizsgálata képezte.\n\n" +
        "A készülékek védelmi módja, alkalmazási csoportja és hőmérsékleti osztálya megfelel az adattáblán, a tanúsítványban és a gyártói nyilatkozatban rögzített feltételeknek.\n\n" +
        "A berendezések adatait a melléklet „Robbanásbiztonság-technikai felülvizsgálati jegyzőkönyv összesítő” tartalmazza.";

    /// <summary>Egy rendelet/szabvány tétel megjelenítése: vastag szám, alatta a cím/leírás.</summary>
    private void JogszabalyTetel(IContainer container, string szam, string cim)
    {
        container.Column(col =>
        {
            col.Item().Text(szam).Bold().FontSize(10);
            col.Item().PaddingLeft(15).Text(cim).FontSize(9);
        });
    }

    /// <summary>Az összefoglaló (hibalista / hiányosság lista) oldalak közös fejléce: cégnév/telephely balra, cím középen, jegyzőkönyv sorszám jobbra.</summary>
    private void OsszefoglaloFejlec(IContainer container, string cegNev, string? targyLeiras, string jegyzokonyvSzam, string cim)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(cegNev).FontSize(10);
                    if (!string.IsNullOrWhiteSpace(targyLeiras))
                        c.Item().Text(targyLeiras).FontSize(9);
                });
                row.RelativeItem().AlignCenter().Text(cim).Bold().FontSize(14);
                row.RelativeItem().AlignRight().Text($"Jegyzőkönyv sorszám: {jegyzokonyvSzam}").FontSize(9);
            });

            col.Item().PaddingVertical(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    /// <summary>A "4 Minősítés" fejezet 8 szempontjának megnevezése (a mintaképen látható sorrendben).</summary>
    private static readonly (string Kulcs, string Cim)[] MinositesSzempontok =
    {
        ("1", "Tűz és robbanásveszély elleni védelem"),
        ("2", "A tokozások épsége, zárhatósága"),
        ("3", "Kúszóáramutak, légközök"),
        ("4", "Csavarok és biztosításaik"),
        ("5", "Kábelek, vezetékek szerelése"),
        ("6", "Jelölések, felirati táblák"),
        ("7", "Túlmelegedés, felületi hőmérséklet"),
        ("8", "Az adattábla szerinti védettség a beépítési hely követelményeinek"),
    };

    private const string AlapertelmezettMinositesMegjegyzes =
        "A jelen felülvizsgálati jegyzőkönyv a készítésének dátuma szerinti állapotot rögzíti. A megfelelő állapot fenntartása a mindenkori üzemeltető felelőssége. " +
        "Amennyiben módosítás vagy kiegészítés (bővítés) történik a jegyzőkönyvben rögzített állapothoz képest, akkor a mindenkori üzemeltető felelőssége, hogy a jegyzőkönyv is megfelelően kiegészítésre kerüljön az érvényes és vonatkozó szabványoknak, előírásoknak megfelelően.";

    /// <summary>
    /// A "4 Minősítés" fejezet oldala: a 8 rész-szempont (1-7. alapból "Megfelelő", 8. a berendezések zóna-megfelelősségéből
    /// számítva), az összesített ("FŐ") minősítés, mindkettő kézzel felülbírálható, valamint egy előre kitöltött,
    /// szerkeszthető megjegyzés szöveg.
    /// </summary>
    private void MinositesOldal(IContainer container, List<RbSor> sorok, Dictionary<string, bool>? felulbiralas, bool? foFelulbiralas, string? megjegyzes)
    {
        // 8. szempont alapértéke: a berendezések zóna-megfelelőségéből számítva. Ha bármelyik sor egyértelműen nem
        // felel meg a térségbesorolásnak, vagy van bármilyen nem megfelelő checklist tétel, a számított alapérték "nem megfelelő".
        var vanNemMegfeloZona = sorok.Any(s => s.ZonaMegfelelo == false);
        var vanNemMegfeloChecklistTetel = ChecklistHibakOsszegyujtese(sorok).Count > 0;
        var nyolcadikSzempontSzamitott = !(vanNemMegfeloZona || vanNemMegfeloChecklistTetel);

        var szamitottErtekek = new Dictionary<string, bool>();
        foreach (var (kulcs, _) in MinositesSzempontok)
            szamitottErtekek[kulcs] = kulcs == "8" ? nyolcadikSzempontSzamitott : true;

        bool VegsoErtek(string kulcs) => felulbiralas != null && felulbiralas.TryGetValue(kulcs, out var felulbiraltErtek)
            ? felulbiraltErtek
            : szamitottErtekek[kulcs];

        var reszEredmenyek = MinositesSzempontok.Select(sz => (sz.Cim, Megfelelt: VegsoErtek(sz.Kulcs))).ToList();

        var foSzamitott = reszEredmenyek.All(r => r.Megfelelt);
        var foVegso = foFelulbiralas ?? foSzamitott;

        container.Column(col =>
        {
            col.Item().AlignCenter().Text("4 Minősítés").Bold().FontSize(16).Italic();

            col.Item().PaddingTop(10).Column(lista =>
            {
                foreach (var (cim, megfelelt) in reszEredmenyek)
                {
                    lista.Item().PaddingVertical(2).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("-  ");
                            text.Span($"{cim}:").FontSize(10);
                        });
                        row.ConstantItem(110).AlignRight().Text(megfelelt ? "Megfelelő" : "Nem megfelelő")
                            .Italic().FontSize(10).FontColor(megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
                    });
                }
            });

            col.Item().PaddingTop(16).Background(foVegso ? Colors.Green.Lighten4 : Colors.Red.Lighten4)
                .Border(1).BorderColor(foVegso ? Colors.Green.Darken2 : Colors.Red.Darken2).Padding(8)
                .Column(fo =>
                {
                    fo.Item().AlignCenter().Text("A felülvizsgálat során megállapítást nyert, hogy a berendezések:").FontSize(9);
                    fo.Item().AlignCenter().PaddingTop(4).Text(foVegso ? "MEGFELELNEK" : "NEM FELELNEK MEG").Bold().FontSize(16)
                        .FontColor(foVegso ? Colors.Green.Darken3 : Colors.Red.Darken3);
                });

            col.Item().PaddingTop(16).Text("Megjegyzés:").Bold().FontSize(10);
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(megjegyzes) ? AlapertelmezettMinositesMegjegyzes : megjegyzes).FontSize(9);
        });
    }

    /// <summary>A valós hibalista táblázata: csak a meg nem felelő checklist tételeket tartalmazó berendezések kerülnek bele, a minősítés oszlopban a hiba leírásával.</summary>
    private void HibalistaTablazat(IContainer container, List<RbSor> sorok)
    {
        var hibak = ChecklistHibakOsszegyujtese(sorok);

        if (hibak.Count == 0)
        {
            container.PaddingTop(10).AlignCenter().Text("Nincs rögzített hiba.").FontSize(10).Italic();
            return;
        }

        OsszefoglaloTablazatKozos(container, hibak, "Minősítés");
    }

    /// <summary>A hiányosság lista táblázata: csak a hiányzó kötelező adattal rendelkező berendezések kerülnek bele, az utolsó oszlopban a hiányosság megjegyzésével.</summary>
    private void HianyossagTablazat(IContainer container, List<RbSor> sorok)
    {
        var hianyossagok = HianyossagokOsszegyujtese(sorok);

        if (hianyossagok.Count == 0)
        {
            container.PaddingTop(10).AlignCenter().Text("Nincs rögzített hiányosság.").FontSize(10).Italic();
            return;
        }

        OsszefoglaloTablazatKozos(container, hianyossagok, "Megjegyzés");
    }

    /// <summary>Közös táblázat renderelő a hibalista és a hiányosság lista oldalakhoz, csak az utolsó oszlop fejléce ("Minősítés" / "Megjegyzés") tér el.</summary>
    private void OsszefoglaloTablazatKozos(IContainer container, List<(RbSor Sor, string Leiras)> sorok, string utolsoOszlopFejlec)
    {
        container.PaddingTop(4).Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(28);  // Sorsz.
                c.ConstantColumn(40);  // Címke
                c.RelativeColumn(2);   // Objektum
                c.RelativeColumn(2.5f);// Hely
                c.RelativeColumn(2.5f);// Megnevezés
                c.RelativeColumn(1.5f);// Körszám
                c.RelativeColumn(1.7f);// Gyártó
                c.RelativeColumn(2f);  // Típus
                c.RelativeColumn(2f);  // Gyári szám
                c.ConstantColumn(35);  // IP védelem
                c.RelativeColumn(3f);  // Rb védelmi mód
                c.RelativeColumn(1.8f);// V.Á. eng. szám
                c.RelativeColumn(3.2f);// Minősítés / Megjegyzés
            });

            table.Header(header =>
            {
                FejlecCella(header, "Sorsz.");
                FejlecCella(header, "Címke");
                FejlecCella(header, "Objektum");
                FejlecCella(header, "Hely");
                FejlecCella(header, "Megnevezés");
                FejlecCella(header, "Körszám");
                FejlecCella(header, "Gyártó");
                FejlecCella(header, "Típus");
                FejlecCella(header, "Gyári szám");
                FejlecCella(header, "IP védelem");
                FejlecCella(header, "Rb védelmi mód");
                FejlecCella(header, "V.Á. eng. szám");
                FejlecCella(header, utolsoOszlopFejlec);
            });

            foreach (var (sor, leiras) in sorok)
            {
                var szovegSzin = Colors.Black;

                SorCella(table, sor.Sorsz.ToString(), szovegSzin);
                SorCella(table, sor.CimkeSorszam, szovegSzin);
                SorCella(table, sor.Objektum, szovegSzin);
                SorCella(table, sor.Elhelyezes, szovegSzin);
                SorCella(table, sor.Megnevezes, szovegSzin);
                SorCella(table, sor.AramkoriJel, szovegSzin);
                SorCella(table, sor.Gyarto, szovegSzin);
                SorCella(table, sor.Tipus, szovegSzin);
                SorCella(table, sor.GyariSzam, szovegSzin);
                SorCella(table, sor.IpVedelem, szovegSzin);
                SorCella(table, sor.VedelmiMod, szovegSzin);
                SorCella(table, sor.EngSzam, szovegSzin);
                SorCella(table, leiras, Colors.Red.Darken2);
            }
        });
    }

    /// <summary>Összegyűjti a meg nem felelő checklist tételeket (valós hiba), soronként/tételenként egy bejegyzésként.</summary>
    private static List<(RbSor Sor, string Leiras)> ChecklistHibakOsszegyujtese(List<RbSor> sorok)
    {
        var eredmeny = new List<(RbSor, string)>();

        foreach (var sor in sorok)
        {
            foreach (var tetel in ChecklistTetelek(sor))
            {
                if (!tetel.Megfelelt)
                {
                    eredmeny.Add((sor, string.IsNullOrWhiteSpace(tetel.Megjegyzes)
                        ? $"{tetel.Szoveg} nem megfelelő!"
                        : $"{tetel.Szoveg} nem megfelelő! ({tetel.Megjegyzes})"));
                }
            }
        }

        return eredmeny;
    }

    /// <summary>Összegyűjti a hiányzó kötelező adatokkal rendelkező berendezéseket (hiányosság, de a felülvizsgálat egyébként megfelelő).</summary>
    private static List<(RbSor Sor, string Leiras)> HianyossagokOsszegyujtese(List<RbSor> sorok)
    {
        var eredmeny = new List<(RbSor, string)>();

        foreach (var sor in sorok)
        {
            if (sor.VanHianyzoAdat)
            {
                eredmeny.Add((sor, $"Hiányzó kötelező adat(ok): {string.Join(", ", sor.HianyzoKotelezoMezok)}"));
            }
        }

        return eredmeny;
    }

    private static IEnumerable<RbCheckTetel> ChecklistTetelek(RbSor sor)
    {
        foreach (var tetel in sor.Kornyezeti) yield return tetel;
        foreach (var tetel in sor.KeszulekAllapota) yield return tetel;
        if (sor.VanExI) foreach (var tetel in sor.ExI) yield return tetel;
        if (sor.VanExD) foreach (var tetel in sor.ExD) yield return tetel;
        if (sor.VanExM) foreach (var tetel in sor.ExM) yield return tetel;
        if (sor.VanExE) foreach (var tetel in sor.ExE) yield return tetel;
        if (sor.VanExP) foreach (var tetel in sor.ExP) yield return tetel;
    }

    private void FejlecCella(TableCellDescriptor header, string szoveg)
    {
        header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Grey.Medium).Padding(3)
            .Text(szoveg).Bold().FontSize(8);
    }

    private void SorCella(TableDescriptor table, string? szoveg, string szovegSzin)
    {
        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3)
            .Text(szoveg ?? "").FontSize(7.5f).FontColor(szovegSzin);
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
        byte[]? alairasKep = null;
        if (_felulvizsgaloAlairasKepek != null && !string.IsNullOrWhiteSpace(sor.VizsgalatotVegezte))
            _felulvizsgaloAlairasKepek.TryGetValue(sor.VizsgalatotVegezte, out alairasKep);

        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                if (alairasKep != null)
                {
                    col.Item().AlignCenter().Height(30).Image(alairasKep).FitArea();
                }
                else
                {
                    col.Item().AlignCenter().PaddingBottom(15).Text("________________________");
                }
                col.Item().AlignCenter().Text(string.IsNullOrWhiteSpace(sor.VizsgalatotVegezte) ? "A vizsgálatot végezte" : sor.VizsgalatotVegezte);
                col.Item().AlignCenter().Text("felülvizsgáló").FontSize(8).FontColor(Colors.Grey.Darken1);
                if (_cegBelyegzoKep != null)
                {
                    col.Item().AlignCenter().PaddingTop(6).Height(30).Image(_cegBelyegzoKep).FitArea();
                }
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
