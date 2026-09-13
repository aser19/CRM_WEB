using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// "Nem Norma szerinti Villámvédelmi Minősítő Irat" PDF generálása.
/// Oldalstruktúra: 1. Címlap, 2. Főadatok, 3. Tartalomjegyzék (a további oldalak később kerülnek kidolgozásra).
/// </summary>
public class VillamNemNormaPdfService : IVillamNemNormaPdfService
{
    private byte[]? _cegBelyegzoKep;
    private byte[]? _alairoAlairasKep;

    public VillamNemNormaPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generalas(VillamNemNormaAdatok adatok, byte[]? cegBelyegzoKep = null, byte[]? alairoAlairasKep = null)
    {
        adatok ??= new VillamNemNormaAdatok();
        _cegBelyegzoKep = cegBelyegzoKep;
        _alairoAlairasKep = alairoAlairasKep;

        return Document.Create(container =>
        {
            // 1. oldal – Címlap
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => Cimlap(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 2. oldal – Főadatok
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => Foadatok(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 3. oldal – Tartalomjegyzék
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(Tartalomjegyzek);
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 4. oldal – Bevezetés
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => Bevezetes(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 5. oldal – A vizsgálat módszerei
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => VizsgalatModszerei(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 6. oldal – Műszeres mérések (2.3) + Az építmény vizsgálata (3.1)
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => MuszeresMeresekEsEpitmenyVizsgalata(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 7. oldal – Az épület vizsgálata (folytatás): elemenkénti besorolás, hibák, észrevételek
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => EpuletVizsgalataFolytatas(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });

            // 8. oldal – Belső villámvédelem (3.4) + Általános minősítés (4.) + záró adatok
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, adatok));
                page.Content().Element(c => ZaroOldal(c, adatok));
                page.Footer().Element(c => Lablec(c, adatok));
            });
        }).GeneratePdf();
    }

    // ============================================================
    // FEJLÉC / LÁBLÉC
    // ============================================================

    /// <summary>A cég neve dőlt/félkövér stílusban, alatta kettős elválasztó vonal (a minta szerint).</summary>
    private void Fejlec(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().AlignCenter().Text(adatok.CegNev).Italic().Bold().FontSize(13);
            col.Item().PaddingTop(2).LineHorizontal(2).LineColor(Colors.Red.Darken2);
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Red.Darken2);
        });
    }

    /// <summary>A lábléc: cégadatok (cím, web, telefon) balra, oldalszám jobbra.</summary>
    private void Lablec(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.PaddingTop(4).Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                if (!string.IsNullOrWhiteSpace(adatok.CegCim))
                    text.Span($"{adatok.CegCim}; ").FontSize(8);
                if (!string.IsNullOrWhiteSpace(adatok.CegWeb))
                {
                    text.Span("web: ").FontSize(8);
                    text.Span(adatok.CegWeb).FontSize(8).Underline();
                    text.Span(" ").FontSize(8);
                }
                if (!string.IsNullOrWhiteSpace(adatok.CegTelefon))
                    text.Span($"Tel.: {adatok.CegTelefon}").FontSize(8);
            });
            row.ConstantItem(90).AlignRight().Text(text =>
            {
                text.Span("Oldal: ").FontSize(9);
                text.CurrentPageNumber().FontSize(9).Bold();
                text.Span(" / ").FontSize(9);
                text.TotalPages().FontSize(9).Bold();
            });
        });
    }

    // ============================================================
    // 1. OLDAL – CÍMLAP
    // ============================================================

    private void Cimlap(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(60).AlignCenter().Text("NEM NORMA SZERINTI VILLÁMVÉDELMI MINŐSÍTŐ IRAT")
                .FontSize(13);

            col.Item().PaddingTop(60).AlignCenter().Text(adatok.MegrendeloNev).Bold().FontSize(12);

            if (!string.IsNullOrWhiteSpace(adatok.MegrendeloCim))
                col.Item().PaddingTop(10).AlignCenter().Text(adatok.MegrendeloCim).FontSize(11);

            var ev = (adatok.MeresIdeje ?? DateTime.Today).Year;
            col.Item().PaddingTop(60).AlignCenter().Text($"{ev}.").Bold().FontSize(12);
        });
    }

    // ============================================================
    // 2. OLDAL – FŐADATOK
    // ============================================================

    private void Foadatok(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(10).AlignCenter().Text("VILLÁMVÉDELMI MÉRÉSI JEGYZŐKÖNYV").Bold().FontSize(11);

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("MEGBÍZÓ:").Bold();
                    c.Item().PaddingTop(4).Text(adatok.MegrendeloNev);
                    if (!string.IsNullOrWhiteSpace(adatok.MegrendeloCim))
                        c.Item().Text(adatok.MegrendeloCim);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("A MEGBÍZÁS TÁRGYA:").Bold();
                    c.Item().PaddingTop(4).Text(adatok.MegbizasTargya);
                });
            });

            col.Item().PaddingTop(30).Element(c => AdatSor(c, "A mérést végezte:", FelulvizsgaloSzoveg(adatok)));

            if (!string.IsNullOrWhiteSpace(adatok.SegitoFelulvizsgalo))
                col.Item().PaddingTop(10).Element(c => AdatSor(c, "Segítő felülvizsgáló:", NevBizonyitvany(adatok.SegitoFelulvizsgalo, adatok.SegitoBizonyitvany)));

            if (!string.IsNullOrWhiteSpace(adatok.Ellenor))
                col.Item().PaddingTop(10).Element(c => AdatSor(c, "Ellenőr:", NevBizonyitvany(adatok.Ellenor, adatok.EllenorBizonyitvany)));

            col.Item().PaddingTop(10).Element(c => AdatSor(c, "Megbízó képviselője:", adatok.MegrendeloKepviseloje));

            col.Item().PaddingTop(60).Text(adatok.MeresIdeje.HasValue ? $"{adatok.MeresIdeje:yyyy. MMMM d.}" : "").FontSize(10);

            col.Item().PaddingTop(4).Text($"ÉRVÉNYES: {(adatok.FelulvizsgalatErvenyessegeIg.HasValue ? adatok.FelulvizsgalatErvenyessegeIg.Value.ToString("yyyy.MM.dd.") : "")}").Bold();
        });
    }

    private string FelulvizsgaloSzoveg(VillamNemNormaAdatok adatok) =>
        NevBizonyitvany(adatok.FelulvizsgaloNev, adatok.FelulvizsgaloBizonyitvany);

    private string NevBizonyitvany(string? nev, string? bizonyitvany)
    {
        if (string.IsNullOrWhiteSpace(nev)) return "";
        return string.IsNullOrWhiteSpace(bizonyitvany) ? nev : $"{nev} (biz. szám: {bizonyitvany})";
    }

    private void AdatSor(IContainer container, string cimke, string? ertek)
    {
        container.Row(row =>
        {
            row.ConstantItem(140).Text(cimke).Bold();
            row.RelativeItem().Text(ertek ?? "");
        });
    }

    // ============================================================
    // 3. OLDAL – TARTALOMJEGYZÉK
    // ============================================================

    private void Tartalomjegyzek(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(20).Element(c => FejezetCim(c, "1.", "Bevezetés"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "1.1", "Előzmények"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "1.2", "A rendelkezésre bocsátott dokumentációk"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "1.3", "A vizsgálatnál figyelembe vett rendeletek, és szabványok"));

            col.Item().PaddingTop(16).Element(c => FejezetCim(c, "2.", "A vizsgálat módszerei"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "2.1", "A villámvédelmi besorolás ellenőrzése"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "2.2", "Szemrevételezés"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "2.3", "Műszeres mérések"));

            col.Item().PaddingTop(16).Element(c => FejezetCim(c, "3.", "Az épület vizsgálata"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.1", "A szükséges villámvédelmi fokozat meghatározása, és a meglévő villámvédelmi berendezéssel való összehasonlítása"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.2", "A vizsgálat során tapasztalt hibák, hiányosságok"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.3", "A felülvizsgálattal kapcsolatos észrevételek, megjegyzések"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.4", "Belső villámvédelem"));

            col.Item().PaddingTop(16).Element(c => FejezetCim(c, "4.", "A villámvédelem általános minősítése"));
        });
    }

    private void FejezetCim(IContainer container, string szam, string cim)
    {
        container.PaddingBottom(6).Row(row =>
        {
            row.ConstantItem(24).Text(szam).Bold().LineHeight(1.4f);
            row.RelativeItem().Text(cim).Bold().Underline().LineHeight(1.4f);
        });
    }

    private void AlfejezetCim(IContainer container, string szam, string cim)
    {
        container.PaddingTop(2).PaddingBottom(4).Row(row =>
        {
            row.ConstantItem(34).Text(szam).LineHeight(1.4f);
            row.RelativeItem().Text(cim).LineHeight(1.4f);
        });
    }

    // ============================================================
    // 4. OLDAL – BEVEZETÉS
    // ============================================================

    private void Bevezetes(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().Element(c => FejezetCim(c, "1.", "Bevezetés:"));

            col.Item().PaddingTop(10).PaddingLeft(20).Element(c => AlfejezetCim(c, "1.1", "Előzmények:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Text(ElozmenySzoveg(adatok));

            col.Item().PaddingTop(14).PaddingLeft(20).Element(c => AlfejezetCim(c, "1.2", "A rendelkezésre bocsátott dokumentációk:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Text(string.IsNullOrWhiteSpace(adatok.RendelkezesreBocsatottDokumentaciok)
                ? "-" : adatok.RendelkezesreBocsatottDokumentaciok);

            col.Item().PaddingTop(14).PaddingLeft(20).Element(c => AlfejezetCim(c, "1.3", "A vizsgálatnál figyelembe vett rendeletek és dokumentációk:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Column(jsz =>
            {
                var kivalasztottak = (adatok.KijeloltJogszabalyok ?? new())
                    .Where(j => j.Kivalasztva)
                    .OrderBy(j => j.IsSzabvany)
                    .ThenBy(j => j.Szam)
                    .ToList();

                if (!kivalasztottak.Any())
                {
                    jsz.Item().Text("-");
                }
                else
                {
                    foreach (var j in kivalasztottak)
                    {
                        jsz.Item().Row(row =>
                        {
                            row.ConstantItem(140).Text(j.Szam);
                            row.RelativeItem().Text(j.Cim);
                        });
                    }
                }

                if (adatok.NincsEredetiTervdokumentacio)
                {
                    var epitesEv = adatok.LetesitmenyEpitesEve.HasValue ? adatok.LetesitmenyEpitesEve.Value.ToString() : "ismeretlen";
                    jsz.Item().PaddingTop(10).Text(
                        $"A létesítmény eredeti műszaki kiírással nem rendelkezik (a létesítmény építésének éve: {epitesEv}). A kialakítottak villámvédelmi " +
                        "rendszer szabványának való megfelelősége lett vizsgálva.");
                }
            });
        });
    }

    private string ElozmenySzoveg(VillamNemNormaAdatok adatok)
    {
        var keszito = adatok.KikeszitetteSzerep switch
        {
            "Segito" => NevBizonyitvany(adatok.SegitoFelulvizsgalo, adatok.SegitoBizonyitvany),
            "Ellenor" => NevBizonyitvany(adatok.Ellenor, adatok.EllenorBizonyitvany),
            _ => NevBizonyitvany(adatok.FelulvizsgaloNev, adatok.FelulvizsgaloBizonyitvany),
        };

        return $"A vizsgálati jelentést készítette és a minősítő iratot szerkesztette: {keszito}.\n" +
               "A villámvédelmi mérés elvégzése és a jelen irat kiadására időszakos felülvizsgálat alkalmából került sor.";
    }

    // ============================================================
    // 5. OLDAL – A VIZSGÁLAT MÓDSZEREI
    // ============================================================

    private void VizsgalatModszerei(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().Element(c => FejezetCim(c, "2.", "A vizsgálat módszerei:"));

            col.Item().PaddingTop(10).PaddingLeft(20).Element(c => AlfejezetCim(c, "2.1.", "Villámvédelmi besorolás meghatározása:"));

            col.Item().PaddingTop(6).PaddingLeft(40).Element(c => BesorolasTablazat(c, adatok));

            col.Item().PaddingTop(16).PaddingLeft(20).Element(c => AlfejezetCim(c, "2.2", "Szemrevételezés:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.SzemrevetelezesLeiras);

            col.Item().PaddingTop(8).PaddingLeft(40).Text("Szemrevételezéssel vizsgálva lett ezen kívül:");
            col.Item().PaddingTop(4).PaddingLeft(40).Column(list =>
            {
                var tetelek = adatok.SzemrevetelezesTetelek ?? new();
                foreach (var t in tetelek.Where(t => t.Kivalasztva))
                {
                    list.Item().Text($"•  {t.Megnevezes}");
                }
            });
        });
    }

    private void BesorolasTablazat(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Text("Megnevezés:").Bold();
                header.Cell().Text("Besorolás:").Bold();
                header.Cell().Text("Állapot:").Bold();
            });

            var sorok = adatok.VillamBesorolasSorok ?? new();
            foreach (var sor in sorok)
            {
                table.Cell().PaddingTop(6).Text(sor.Megnevezes);
                table.Cell().PaddingTop(6).Text(sor.Besorolas ?? "");
                table.Cell().PaddingTop(6).Text(sor.Allapot ?? "");
            }
        });
    }

    // ============================================================
    // 6. OLDAL – MŰSZERES MÉRÉSEK (2.3) + AZ ÉPÍTMÉNY VIZSGÁLATA (3.1)
    // ============================================================

    private void MuszeresMeresekEsEpitmenyVizsgalata(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "2.3", "Műszeres mérések:"));

            col.Item().PaddingTop(8).PaddingLeft(40).Element(c => AdatSor(c, "A mérés idején az időjárás:", adatok.Idojaras));

            var muszerek = adatok.Muszerek ?? new();
            if (muszerek.Any(m => !string.IsNullOrWhiteSpace(m.Tipus)))
            {
                col.Item().PaddingTop(8).PaddingLeft(40).Text("Az alkalmazott mérőműszer:").Bold();
                col.Item().PaddingLeft(60).Text(string.Join("\n", muszerek.Where(m => !string.IsNullOrWhiteSpace(m.Tipus)).Select(m => m.Tipus)));
                col.Item().PaddingLeft(60).Text($"Gyártási szám: {string.Join(", ", muszerek.Where(m => !string.IsNullOrWhiteSpace(m.GyariSzam)).Select(m => m.GyariSzam))}");
                col.Item().PaddingLeft(60).Text($"Kalibrálva: {string.Join(", ", muszerek.Where(m => !string.IsNullOrWhiteSpace(m.Kalibralas)).Select(m => m.Kalibralas))}");
            }

            col.Item().PaddingTop(10).PaddingLeft(40).Text("A mérés körülményeinek rövid leírása:").Bold();
            col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.MeresKorulmenyeinekLeirasa);

            if (!string.IsNullOrWhiteSpace(adatok.MertErtekSzabvanySzerint))
                col.Item().PaddingTop(6).PaddingLeft(40).Text(adatok.MertErtekSzabvanySzerint);

            if (!string.IsNullOrWhiteSpace(adatok.MertErtek))
            {
                var mertErtekSzoveg = adatok.MertErtek.Contains('Ω') ? adatok.MertErtek : $"{adatok.MertErtek} Ω";
                col.Item().PaddingTop(10).PaddingLeft(40).Text("A felülvizsgálat eredménye:");
                col.Item().PaddingLeft(40).AlignCenter().Text(mertErtekSzoveg).Bold().FontSize(12);
            }

            col.Item().PaddingTop(6).PaddingLeft(40).AlignCenter().Text(adatok.MeresEredmenyeAllapot.ToUpper()).Bold().Underline().FontSize(12);

            col.Item().PaddingTop(20).Element(c => AlfejezetCim(c, "3", "Az építmény vizsgálata:"));
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.1", "A szükséges villámvédelmi fokozat meghatározása, és a meglévő villámvédelmi berendezéssel való összehasonlítása."));

            if (!string.IsNullOrWhiteSpace(adatok.OsszehasonlitasSzabvanySzerint))
                col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.OsszehasonlitasSzabvanySzerint);

            col.Item().PaddingTop(10).PaddingLeft(40).Text("Tűzveszélyességi osztályba sorolás a kapott tájékoztatás alapján:");
            col.Item().PaddingLeft(40).AlignCenter().Text(adatok.TuzveszelyessegiOsztaly).Bold().FontSize(12);

            var besorolas = string.IsNullOrWhiteSpace(adatok.VillamvedelmiBesorolasEredmenye)
                ? adatok.SzamitottVillamvedelmiBesorolas
                : adatok.VillamvedelmiBesorolasEredmenye;

            col.Item().PaddingTop(10).PaddingLeft(40).Text("Az építmény villámvédelmi csoportjai alapján történő besorolás eredménye:");
            col.Item().PaddingLeft(40).AlignCenter().Text(besorolas).Bold().FontSize(12);
        });
    }

    // ============================================================
    // 7. OLDAL – AZ ÉPÜLET VIZSGÁLATA (FOLYTATÁS)
    // ============================================================

    private void EpuletVizsgalataFolytatas(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            var sorok = adatok.ElemBesorolasSorok ?? new();
            foreach (var sor in sorok)
            {
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem(2).Text(sor.Megnevezes);
                    row.RelativeItem(1).Text(sor.Fokozat ?? "");
                    row.RelativeItem(1).Text((sor.Allapot ?? "").ToUpper()).Bold().Underline();
                });
            }

            col.Item().PaddingTop(20).AlignCenter().Text("A felülvizsgálat értékelése:").Bold();
            col.Item().PaddingTop(6).AlignCenter().Text(adatok.FelulvizsgalatErtekelese.ToUpper()).Bold().FontSize(14);

            col.Item().PaddingTop(20).PaddingLeft(20).Element(c => AlfejezetCim(c, "3.2", "A vizsgálat során tapasztalt hibák, hiányosságok:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.HibakHianyossagok);

            col.Item().PaddingTop(16).PaddingLeft(20).Element(c => AlfejezetCim(c, "3.3", "A felülvizsgálattal kapcsolatos észrevételek, megjegyzések:"));
            col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.EszrevetelekMegjegyzesek);
        });
    }

    // ============================================================
    // 8. OLDAL – BELSŐ VILLÁMVÉDELEM (3.4) + ÁLTALÁNOS MINŐSÍTÉS (4.) + ZÁRÓ ADATOK
    // ============================================================

    private void ZaroOldal(IContainer container, VillamNemNormaAdatok adatok)
    {
        container.Column(col =>
        {
            col.Item().PaddingLeft(20).Element(c => AlfejezetCim(c, "3.4", "Belső villámvédelem:"));

            if (adatok.BelsoVillamvedelemTartalmazza)
            {
                col.Item().PaddingLeft(40).PaddingTop(4).Text(adatok.BelsoVillamvedelemLeirasa);
                col.Item().PaddingLeft(40).AlignCenter().Text(adatok.BelsoVillamvedelemAllapot.ToUpper()).Bold().Underline().FontSize(12);
            }
            else
            {
                col.Item().PaddingLeft(40).PaddingTop(4).Text("A megbízás nem tartalmazza a belső villámvédelem vizsgálatát.");
            }

            col.Item().PaddingTop(20).Element(c => FejezetCim(c, "4.", "A villámvédelem általános minősítése:"));
            col.Item().PaddingLeft(20).PaddingTop(4).Text(adatok.AltalanosMinositesSzovege);
            col.Item().PaddingTop(8).PaddingLeft(20).AlignCenter().Text(adatok.AltalanosMinositesAllapot.ToUpper()).Bold().FontSize(14);

            if (adatok.KovetkezoFelulvizsgalatIdopontja.HasValue)
            {
                col.Item().PaddingTop(20).Text(text =>
                {
                    text.Span("A következő villámvédelmi felülvizsgálat időpontja: ").Bold();
                    text.Span($"{adatok.KovetkezoFelulvizsgalatIdopontja:yyyy. MMMM d.}").Bold();
                });
            }

            col.Item().PaddingTop(20).Text($"{adatok.HolKeszult}, {(adatok.MikorKeszult ?? DateTime.Today):yyyy. MMMM d.}.");

            col.Item().PaddingTop(50).Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem().Column(c =>
                {
                    if (_alairoAlairasKep != null)
                        c.Item().AlignCenter().Height(30).Image(_alairoAlairasKep).FitArea();
                    else
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");

                    c.Item().AlignCenter().Text(adatok.FelulvizsgaloNev ?? "").Bold().FontSize(10);

                    if (_cegBelyegzoKep != null)
                        c.Item().AlignCenter().PaddingTop(6).Height(30).Image(_cegBelyegzoKep).FitArea();
                });
            });
        });
    }
}
