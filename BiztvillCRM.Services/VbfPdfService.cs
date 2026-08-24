using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Az Időszakos VBF ("Használatbavételt megelőző") fő jegyzőkönyv PDF generálása.
/// A "VBF_KIF_MINTA.docx" Word sablon oldalstruktúráját és tartalmát követi:
/// címlap, minősítő irat (1-3. oldal), minősítési alapadatok + jogszabályok/szabványok,
/// vizsgálati eredmények összefoglalása (1/2, 2/2), OTSZ és VMBSZ ellenőrzések,
/// a védelmek ellenőrzése, áramkörök leírása helyiségenként, útmutató a megrendelőnek.
/// </summary>
public class VbfPdfService : IVbfPdfService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private byte[]? _cegBelyegzoKep;
    private Dictionary<string, byte[]>? _felulvizsgaloAlairasKepek;

    public VbfPdfService(IMeresService meresService, ITenantService tenantService, ICegService cegService)
    {
        _meresService = meresService;
        _tenantService = tenantService;
        _cegService = cegService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok, string sablonId = "VBF_KIF_MINTA",
        byte[]? cegBelyegzoKep = null, Dictionary<string, byte[]>? felulvizsgaloAlairasKepek = null)
    {
        _cegBelyegzoKep = cegBelyegzoKep;
        _felulvizsgaloAlairasKepek = felulvizsgaloAlairasKepek;

        Meres? meres = null;
        if (meresId > 0)
        {
            meres = await _meresService.GetByIdAsync(meresId);
        }

        adatok ??= new JegyzokonyvAdatok();

        Ceg? ceg = null;
        try
        {
            var cegId = _tenantService.GetCurrentCegId();
            ceg = await _cegService.GetByIdAsync(cegId);
        }
        catch
        {
            // ha nincs tenant kontextus, a formAdatok cégadataira hagyatkozunk
        }

        var cegNev = !string.IsNullOrEmpty(adatok.CegNev) ? adatok.CegNev : ceg?.Nev ?? "";
        var cegCim = !string.IsNullOrEmpty(adatok.CegCim) ? adatok.CegCim : ceg?.Cim ?? "";
        var jkvSzam = !string.IsNullOrEmpty(adatok.JegyzokonyvSzam) ? adatok.JegyzokonyvSzam : $"VBF-{meresId:D6}/{DateTime.Now:yyyy}";
        var vizsgalatHelye = !string.IsNullOrEmpty(adatok.VizsgalatHelye) ? adatok.VizsgalatHelye : meres?.Telephely?.Cim ?? "";
        var meresIdeje = meres?.Datum ?? DateTime.Today;

        return Document.Create(container =>
        {
            // 1. oldal – Címlap (adatlap + tartalomjegyzék + aláírások, önálló keretes elrendezés)
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Content().Element(c => Cimlap(c, cegNev, cegCim, jkvSzam, adatok, vizsgalatHelye, meresIdeje));
            });

            // 2. oldal – Minősítő irat 1/2
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Border(1).Padding(10).Element(c => MinositoIrat1(c, adatok));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 3. oldal – Minősítő irat 2/2
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Border(1).Padding(10).Element(c => MinositoIrat2(c, adatok, meresIdeje));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 4. oldal – Minősítési alapadatok, jogszabályok / szabványok
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Border(1).Padding(10).Element(c => MinositesiAlapadatok(c, adatok));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 5. oldal – Vizsgálati eredmények összefoglalása 1/2
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => VizsgalatiEredmenyek1(c, adatok, jkvSzam));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 5b. oldal – Vizsgálati eredmények összefoglalása 2/2
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => VizsgalatiEredmenyek2(c, adatok, jkvSzam));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 6. oldal – OTSZ ellenőrzés
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => OtszEllenorzes(c, adatok, jkvSzam));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 6b. oldal – VMBSZ ellenőrzés
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => VmbszEllenorzes(c, adatok, jkvSzam));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 7. oldal – A védelmek ellenőrzése
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => VedelmekEllenorzese(c, adatok, jkvSzam));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 8. oldal – Áramkörök leírása helyiségenként
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Element(c => AramkorokLeirasa(c, adatok));
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });

            // 9. oldal – Útmutató a megrendelőnek
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));
                page.Header().Element(c => Fejlec(c, jkvSzam));
                page.Content().Border(1).Padding(10).Element(Utmutato);
                page.Footer().Element(c => Lablec(c, jkvSzam));
            });
        }).GeneratePdf();
    }

    // ============================================================
    // FEJLÉC / LÁBLÉC
    // ============================================================

    private void Fejlec(IContainer container, string jkvSzam)
    {
        container.AlignRight().Text($"Azonosító: {jkvSzam}").FontSize(8);
    }

    private void OldalFejlecDoboz(ColumnDescriptor col, string alcim)
    {
        col.Item().Border(1).Column(inner =>
        {
            inner.Item().PaddingVertical(4).AlignCenter().Text("KISFESZÜLTSÉGŰ ENERGETIKAI BERENDEZÉS\nVILLAMOS BIZTONSÁGI FELÜLVIZSGÁLATA").Bold().FontSize(12);
            inner.Item().BorderTop(1).PaddingVertical(4).AlignCenter().Text(alcim).Bold().FontSize(12);
        });
    }

    private void Lablec(IContainer container, string jkvSzam)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(jkvSzam).FontSize(7);
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Oldal ").FontSize(7);
                text.CurrentPageNumber().FontSize(7);
                text.Span(" / ").FontSize(7);
                text.TotalPages().FontSize(7);
            });
        });
    }

    // ============================================================
    // 1. OLDAL – CÍMLAP / ADATLAP
    // Egyetlen kereten belüli táblázat: alapadatok (vizsgálat helye, időpontja, száma, tárgya,
    // berendezés, végző, megrendelő, üzemi kísérő, kapcsolattartó, időtartama), majd felülvizsgáló /
    // segítő / ellenőr adatai, fix tartalomjegyzék, és alul kelt + aláírás/bélyegző sor.
    // ============================================================

    private void Cimlap(IContainer container, string cegNev, string cegCim, string jkvSzam, JegyzokonyvAdatok a, string vizsgalatHelye, DateTime meresIdeje)
    {
        container.Border(1).Padding(10).Column(col =>
        {
            col.Item().AlignCenter().Text("KISFESZÜLTSÉGŰ ENERGETIKAI BERENDEZÉS\nVILLAMOS BIZTONSÁGI FELÜLVIZSGÁLATA")
                .Bold().FontSize(14);

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(190);
                    c.RelativeColumn();
                });

                void Sor(string cimke, string ertek, bool vastag = false)
                {
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3)
                        .Text(cimke).SemiBold();
                    var cella = table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(ertek ?? "");
                    if (vastag) cella.SemiBold();
                }

                Sor("A vizsgálat helye:", vizsgalatHelye);
                Sor("A vizsgálat időpontja:", meresIdeje.ToString("yyyy.MM.dd"));
                Sor("A vizsgálat száma:", jkvSzam);
                Sor("A vizsgálat tárgya:", a.VizsgalatTargya);
                Sor("A vizsgált berendezés:", a.VizsgaltBerendezes);
                Sor("A vizsgálatot végző vállalkozás neve:", cegNev);
                Sor("A vizsgálatot végző vállalkozás címe:", cegCim);
                Sor("A vizsgálatot megrendelte:", string.IsNullOrEmpty(a.Megrendelo) ? cegNev : a.Megrendelo);
                Sor("Üzemi kísérő a megrendelő részéről:", a.UzemiKisero);
                Sor("Kapcsolattartó a megrendelő részéről:", a.KapcsolatTarto);
                Sor("A vizsgálat időtartama:", a.VizsgalatIdotartama);
            });

            col.Item().PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(190);
                    c.RelativeColumn();
                });

                void Sor(string cimke, string ertek, bool cim = false)
                {
                    var cimkeCella = table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(cimke);
                    if (cim) cimkeCella.Bold(); else cimkeCella.SemiBold();
                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(ertek ?? "");
                }

                Sor("Felelős felülvizsgáló:", a.FelulvizsgaloNev, true);
                Sor("Vizsgabizonyítvány száma:", a.FelulvizsgaloBizonyitvany);
                Sor("Felújító képzés száma:", a.FelulvizsgaloKepzes);

                Sor("Segítő felülvizsgáló:", a.SegitoFelulvizsgalo, true);
                Sor("Vizsgabizonyítvány száma:", a.SegitoBizonyitvany);
                Sor("Felújító képzés száma:", a.SegitoKepzes);

                Sor("A vizsgálatot ellenőrizte:", a.Ellenor, true);
                Sor("Vizsgabizonyítvány száma:", a.EllenorBizonyitvany);
                Sor("Felújító képzés száma:", a.EllenorKepzes);
            });

            col.Item().PaddingTop(8).BorderTop(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Column(c =>
            {
                c.Item().Text("Tartalom:").SemiBold();
                c.Item().PaddingLeft(10).Text("1. → Minősítő irat, Minősítési alapadatok");
                c.Item().PaddingLeft(10).Text("2. → A vizsgálati eredmények összefoglalása");
                c.Item().PaddingLeft(10).Text("3. → Részletes felülvizsgálati jegyzőkönyv");
                c.Item().PaddingLeft(10).Text("4. → Mellékletek");
            });

            col.Item().PaddingTop(10).Text($"Kelt: {DateTime.Today:yyyy.MM.dd}");

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    if (_cegBelyegzoKep != null)
                        c.Item().AlignCenter().Height(28).Image(_cegBelyegzoKep).FitArea();
                    else
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");
                    c.Item().AlignCenter().Text("cégszerű aláírás").FontSize(8);
                });
                row.RelativeItem().Column(c =>
                {
                    byte[]? kep = null;
                    if (_felulvizsgaloAlairasKepek != null && !string.IsNullOrWhiteSpace(a.FelulvizsgaloNev))
                        _felulvizsgaloAlairasKepek.TryGetValue(a.FelulvizsgaloNev, out kep);
                    if (kep != null)
                        c.Item().AlignCenter().Height(28).Image(kep).FitArea();
                    else
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");
                    c.Item().AlignCenter().Text("felelős felülvizsgáló").FontSize(8);
                });
                row.RelativeItem().Column(c =>
                {
                    byte[]? kep = null;
                    if (_felulvizsgaloAlairasKepek != null && !string.IsNullOrWhiteSpace(a.SegitoFelulvizsgalo))
                        _felulvizsgaloAlairasKepek.TryGetValue(a.SegitoFelulvizsgalo, out kep);
                    if (kep != null)
                        c.Item().AlignCenter().Height(28).Image(kep).FitArea();
                    else
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");
                    c.Item().AlignCenter().Text("segítő felülvizsgáló").FontSize(8);
                });
                row.RelativeItem().Column(c =>
                {
                    byte[]? kep = null;
                    if (_felulvizsgaloAlairasKepek != null && !string.IsNullOrWhiteSpace(a.Ellenor))
                        _felulvizsgaloAlairasKepek.TryGetValue(a.Ellenor, out kep);
                    if (kep != null)
                        c.Item().AlignCenter().Height(28).Image(kep).FitArea();
                    else
                        c.Item().AlignCenter().PaddingBottom(4).Text("……………………………..");
                    c.Item().AlignCenter().Text("ellenőrizte").FontSize(8);
                });
            });
        });
    }

    // ============================================================
    // 2. OLDAL – MINŐSÍTŐ IRAT 1/2
    // ============================================================

    private void MinositoIrat1(IContainer container, JegyzokonyvAdatok a)
    {
        container.Column(col =>
        {
            OldalFejlecDoboz(col, "MINŐSÍTŐ IRAT · 1/2");

            col.Item().PaddingTop(10).Text(
                "A villamos energetikai kisfeszültségű berendezésen elvégeztük a 40/2017. (XII.4.) NGM rendelet (VMBSZ) és az " +
                "54/2014. (XII.5.) BM rendelet (OTSZ 5.2) előírásai alapján az időszakos villamos biztonsági felülvizsgálatot.");

            col.Item().PaddingTop(10).Text("A vizsgálat során megállapításra került:").SemiBold();

            col.Item().PaddingTop(6).Text("A) A vizsgált berendezés általánosan:").SemiBold();
            col.Item().PaddingTop(2).Element(c => CheckboxPar(c, "MEGFELELT", "NEM FELELT MEG", a.Eredmeny));

            col.Item().PaddingTop(10).Text("B) Közvetlen élet-, illetve tűzveszélyt okozó hiba:").SemiBold();
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(a.HibakB) ? " " : a.HibakB);

            col.Item().PaddingTop(10).Text("C) Súlyos, soron kívül javítandó hibák:").SemiBold();
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(a.HibakC) ? " " : a.HibakC);

            col.Item().PaddingTop(10).Text("D) A szokásos karbantartások során célszerű a következő hibákat kijavítani:").SemiBold();
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(a.HibakD) ? " " : a.HibakD);

            col.Item().PaddingTop(10).Text("E) Legkésőbb a villamos berendezés következő felújításakor célszerű kijavítani a következő hibákat:").SemiBold();
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(a.HibakE) ? " " : a.HibakE);

            col.Item().PaddingTop(10).Text(
                "A hibaelhárításra vonatkozó ütemezés a felülvizsgáló javaslata, a vonatkozó rendeletek (OTSZ / VMBSZ) alapján. " +
                "Ettől eltérő javítási ütemezés az üzemeltetési vezető felelősségére történhet.").FontSize(8).Italic();

            col.Item().PaddingTop(10).Text("A vizsgálat eredményeként a minősítés:").SemiBold();
            col.Item().PaddingTop(2).Element(c => CheckboxPar(c, "MEGFELELT", "NEM FELELT MEG", a.VegsoMinosites));

            col.Item().PaddingTop(10).Text(
                "Jelen minősítést a felülvizsgálati dokumentáció további fejezetei és mellékletei alapozzák meg a Minősítő Irat " +
                "azokkal együtt érvényes!").FontSize(8).Italic();

            col.Item().PaddingTop(10).Element(c => MellekletekTablazat(c, a));
        });
    }

    /// <summary>Egy MEGFELELT / NEM FELELT MEG (vagy más két érték) párt jelenít meg pipa (☑) / üres (☐) jelöléssel, az aktuális érték alapján.</summary>
    private void CheckboxPar(IContainer container, string igenErtek, string nemErtek, string aktualisErtek)
    {
        var igen = string.Equals(aktualisErtek, igenErtek, StringComparison.OrdinalIgnoreCase);
        var nem = string.Equals(aktualisErtek, nemErtek, StringComparison.OrdinalIgnoreCase);
        container.Row(row =>
        {
            row.AutoItem().Text((igen ? "☑ " : "☐ ") + igenErtek).SemiBold();
            row.ConstantItem(30);
            row.AutoItem().Text((nem ? "☑ " : "☐ ") + nemErtek).SemiBold();
        });
    }

    /// <summary>A kijelölt mellékletek darabszámát és jegyzőkönyv-azonosítóit megjelenítő táblázat (a docx sablon alsó táblázatának megfelelője).
    /// A tartalom mindig a JegyzokonyvAdatok aktuális állapotából épül fel, így utólagos módosítás esetén is naprakész marad.</summary>
    private void MellekletekTablazat(IContainer container, JegyzokonyvAdatok a)
    {
        var sorok = new List<(string Kod, string Szam)>();
        if (a.MellekletHibavedelem) sorok.Add(("HVM", a.HibavedelmiJkv));
        if (a.MellekletAvk) sorok.Add(("AVK", a.AvkJegyzokonyv));
        if (a.MellekletSzigeteles) sorok.Add(("SZI", a.SzigetelesiJkv));
        if (a.MellekletVillam) sorok.Add(("VV", a.VillamJkv));
        if (a.MellekletVillamNem) sorok.Add(("VV_NN", a.VillamNemJkv));

        var sorokSzama = Math.Max(1, (int)Math.Ceiling(sorok.Count / 2.0));

        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(150);
                c.RelativeColumn();
                c.RelativeColumn();
            });

            for (int i = 0; i < sorokSzama; i++)
            {
                if (i == 0)
                {
                    table.Cell().RowSpan((uint)sorokSzama).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4)
                        .Text($"Mellékletek száma: {a.MellekletekSzama} db.\nAzonosítóik:").Italic();
                }

                var elso = i * 2 < sorok.Count ? sorok[i * 2] : (Kod: "", Szam: "");
                var masodik = i * 2 + 1 < sorok.Count ? sorok[i * 2 + 1] : (Kod: "", Szam: "");

                table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignCenter().AlignMiddle()
                    .Text(string.IsNullOrEmpty(elso.Szam) ? "" : elso.Szam).Italic();
                table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignCenter().AlignMiddle()
                    .Text(string.IsNullOrEmpty(masodik.Szam) ? "" : masodik.Szam).Italic();
            }
        });
    }

    // ============================================================
    // 3. OLDAL – MINŐSÍTŐ IRAT 2/2
    // ============================================================

    private void MinositoIrat2(IContainer container, JegyzokonyvAdatok a, DateTime meresIdeje)
    {
        var harom3Eves = a.HataridoTipus is "307" or "308" or "309";
        var ervenyessegDatum = a.KovetkezoFelulvizsgalatDatum ?? meresIdeje.AddYears(harom3Eves ? 6 : 3);

        container.Column(col =>
        {
            OldalFejlecDoboz(col, "MINŐSÍTŐ IRAT · 2/2");

            col.Item().PaddingTop(12).Text("Érvényességi feltételek:").SemiBold();
            col.Item().PaddingTop(4).Text("• A vizsgálat kizárólag a megrendelésben részletezett, a kísérő által bemutatott villamos szerkezetekre terjedt ki.");
            col.Item().PaddingTop(2).Text("• A felhasználói belső szabályzatban részletezett rendeltetésszerű használat.");

            col.Item().PaddingTop(6).Text(text =>
            {
                text.Span("Jelen minősítő irat érvényes: ");
                text.Span($"{ervenyessegDatum:yyyy.MM.dd}").BackgroundColor(Colors.Yellow.Lighten2).SemiBold();
                text.Span("-ig, vagy a berendezésen történt első beavatkozásig!");
            });

            col.Item().PaddingTop(14).Text(
                "Az érvényességi feltételek betartása esetén a villamos berendezés következő időszakos villamos biztonsági felülvizsgálatát:").SemiBold();

            col.Item().PaddingTop(4).Element(c => FelulvizsgalatCheckbox(c, a.KovetkezoFelulvizsgalatTipus == "50kW",
                "az épületnek nem minősülő műtárgy 50 kW-ot meghaladó csatlakozási teljesítményű villamos berendezésén"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.KovetkezoFelulvizsgalatTipus == "32A",
                "a fázisonként 32 A-nél nagyobb névleges áramerősségű túláramvédelemmel korlátozott villamos berendezésen"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.KovetkezoFelulvizsgalatTipus == "VMBSZ",
                "a VMBSZ szerint lakóépület, kommunális épület, valamint egyéb épület villamos berendezésén, ha az munkahelynek minősül"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.KovetkezoFelulvizsgalatTipus == "RV300",
                ">300 kg/l robbanásveszélyes (RV) osztályú anyagot tartalmazó helyiségekben és szabadtéren (korábban: A és B osztályú helyiségek)"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.KovetkezoFelulvizsgalatTipus == "egyeb305",
                "egyéb, itt nem szereplő esetekben: " + (a.KovetkezoFelulvizsgalatTipus == "egyeb305" ? a.KovetkezoFelulvizsgalatEgyeb : "")));

            if (a.KovetkezoFelulvizsgalatTipus is "50kW" or "32A" or "VMBSZ" or "RV300" or "egyeb305")
            {
                col.Item().PaddingTop(6).Text($"a kiadási dátumtól számított 3 éven belül, legkésőbb: {meresIdeje.AddYears(3):yyyy.MM.dd}-ig kell elvégezni.");
            }

            col.Item().PaddingTop(12);

            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.HataridoTipus == "307",
                "a lakóépületek villamos berendezésén (kivéve: lakások, ha Ib ≤ 32 A és IΔn ≤ 30 mA)"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.HataridoTipus == "308",
                "egyéb (robbanásveszélyes /RV/ osztályú anyagot NEM tartalmazó) helyiségekben és térségekben (korábban: C, D és E osztályú helyiségek)"));
            col.Item().PaddingTop(2).Element(c => FelulvizsgalatCheckbox(c, a.HataridoTipus == "309",
                "egyéb, itt nem szereplő esetekben: " + (a.HataridoTipus == "309" ? a.HataridoEgyeb : "")));

            if (a.HataridoTipus is "307" or "308" or "309")
            {
                col.Item().PaddingTop(6).Text($"a kiadási dátumtól számított 6 éven belül, legkésőbb: {meresIdeje.AddYears(6):yyyy.MM.dd}-ig kell elvégezni.");
            }

            col.Item().PaddingTop(14).Text("Megjegyzések, észrevételek, javaslatok:").SemiBold();
            col.Item().PaddingTop(2).Text(string.IsNullOrWhiteSpace(a.MinositoIratMegjegyzes) ? " " : a.MinositoIratMegjegyzes);
        });
    }

    /// <summary>Egy pipa (☑) / üres (☐) jelölésű felülvizsgálati feltétel sor, a felületről érkező kiválasztás alapján.</summary>
    private void FelulvizsgalatCheckbox(IContainer container, bool kivalasztva, string szoveg)
    {
        container.Row(row =>
        {
            row.AutoItem().Text(kivalasztva ? "☑ " : "☐ ");
            row.RelativeItem().Text(szoveg);
        });
    }

    // ============================================================
    // 4. OLDAL – MINŐSÍTÉSI ALAPADATOK + JOGSZABÁLYOK/SZABVÁNYOK
    // ============================================================

    private void MinositesiAlapadatok(IContainer container, JegyzokonyvAdatok a)
    {
        container.Column(col =>
        {
            OldalFejlecDoboz(col, "MINŐSÍTÉSI ALAPADATOK");

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(180);
                    c.RelativeColumn();
                });

                void Sor(string cimke, string ertek)
                {
                    table.Cell().Padding(3).Text(cimke).SemiBold();
                    table.Cell().Padding(3).Text(ertek ?? "");
                }

                var nevlegesFeszultsegSzoveg = a.NevlegesFeszultsegTipus == "3fazis" ? "3×230 V / 400 V" : "230 V";
                var foldelesiTipusSzoveg = a.FoldelesiTipusKod switch
                {
                    "szonda" => "A – Szonda (függőleges)",
                    "vizszintes" => "B – Vízszintes",
                    "mindketto" => "A + B – Mindkettő",
                    _ => a.FoldelesiTipus
                };

                Sor("I. Névleges feszültség:", nevlegesFeszultsegSzoveg);
                Sor("II. Földelési típus:", foldelesiTipusSzoveg);
                Sor("III. Alapvető érintésvédelmi mód:", a.ErintesvedelmiMod);
            });

            col.Item().PaddingTop(15).Text("Áramütés-elleni védelmi módok").SemiBold();
            col.Item().Column(c =>
            {
                void Chk(bool v, string szoveg) => c.Item().Text((v ? "☑ " : "☐ ") + szoveg);
                Chk(a.Vedelem404, "A táplálás önműködő lekapcsolása (TN-/TT-/IT-rendszer)");
                Chk(a.Vedelem405, "Kettős vagy megerősített szigetelés");
                Chk(a.Vedelem406, "Villamos elválasztás");
                Chk(a.Vedelem407, "SELV/PELV törpefeszültség");
                Chk(a.Vedelem408, "Védő egyenpotenciáli összekötés, védővezetők, védőösszekötő-vezetők");
                Chk(a.Vedelem409, "Védelem földeletlen helyi egyenpotenciáli összekötéssel");
            });

            col.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(180);
                    c.RelativeColumn();
                });

                void Sor(string cimke, string ertek)
                {
                    table.Cell().Padding(3).Text(cimke).SemiBold();
                    table.Cell().Padding(3).Text(ertek ?? "");
                }

                Sor("Betáplálás:", a.Betaplalas);
                Sor("Tartalék energia:", a.TartalekEnergia);
                Sor("Legutolsó felújítás:", a.LegutolsoFelujitas);
                Sor("Dokumentációk:", a.Dokumentaciok);
            });

            var jogszabalyok = a.KijeloltJogszabalyok?.Where(j => !j.IsSzabvany && j.Kivalasztva).OrderBy(j => j.Szam).ToList() ?? new();
            var szabvanyok = a.KijeloltJogszabalyok?.Where(j => j.IsSzabvany && j.Kivalasztva).OrderBy(j => j.Szam).ToList() ?? new();

            col.Item().PaddingTop(15).Text("Jogszabályok").SemiBold();
            col.Item().Column(c =>
            {
                foreach (var j in jogszabalyok)
                    c.Item().Text($"• {j.Szam} {j.Cim}".Trim());
            });

            col.Item().PaddingTop(10).Text("Szabványok").SemiBold();
            col.Item().Column(c =>
            {
                foreach (var j in szabvanyok)
                    c.Item().Text($"• {j.Szam} {j.Cim}".Trim());
            });
        });
    }

    // ============================================================
    // ELLENŐRZÉSI TÁBLÁZAT – GENERIKUS SEGÉDFÜGGVÉNY
    // ============================================================

    private sealed record EllenorzesSor(string Cimke, string Minosites, string Megjegyzes);

    private void EllenorzesTablazat(IContainer container, string jkvSzam, string cim, string alcim, List<EllenorzesSor> sorok, string? megjegyzesek = null)
    {
        container.Column(col =>
        {
            OldalFejlecDoboz(col, cim);

            col.Item().BorderLeft(1).BorderRight(1).Row(row =>
            {
                row.RelativeItem(2).Padding(4).Text("Azonosító adatok:").SemiBold();
                row.RelativeItem(3).Padding(4).Text(jkvSzam ?? "");
            });

            if (!string.IsNullOrEmpty(alcim))
            {
                col.Item().BorderLeft(1).BorderRight(1).BorderTop(1).Row(row =>
                {
                    row.RelativeItem(2).Padding(4).Text("Vizsgálatok:").SemiBold();
                    row.RelativeItem(3).Padding(4).Text(alcim);
                });
            }

            col.Item().Border(1).Padding(4).Row(row =>
            {
                row.AutoItem().Text("Jelmagyarázat:").SemiBold();
                row.RelativeItem().PaddingLeft(8).Text("MF: Megfelelő");
                row.RelativeItem().Text("NEM: Nem felel meg");
                row.RelativeItem().Text("NA: A vizsgálat nem alkalmazható");
            });

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(5);
                    c.ConstantColumn(60);
                    c.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(3).Text("").SemiBold();
                    header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text("Minősítés").SemiBold();
                    header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text("Megjegyzés").SemiBold();
                });

                foreach (var sor in sorok)
                {
                    table.Cell().Border(1).Padding(3).Text(sor.Cimke);
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(sor.Minosites);
                    table.Cell().Border(1).Padding(3).Text(sor.Megjegyzes);
                }
            });

            col.Item().Border(1).Padding(4).Text($"Megjegyzések, észrevételek: {megjegyzesek}");
        });
    }

    // ============================================================
    // 5. OLDAL – VIZSGÁLATI EREDMÉNYEK ÖSSZEFOGLALÁSA 1/2
    // ============================================================

    private void VizsgalatiEredmenyek1(IContainer container, JegyzokonyvAdatok a, string jkvSzam)
    {
        var sorok = new List<EllenorzesSor>
        {
            new("a) Rögzített villamos berendezés szerkezetei megfelelnek a termékszabvány biztonsági követelményeinek", a.Ellen5_Sz_A, a.Ellen5_Sz_A_M),
            new("b) MSZ HD 60364 szabványsorozat és gyártó előírásai szerinti kiválasztás és szerelés", a.Ellen5_Sz_B, a.Ellen5_Sz_B_M),
            new("c) Épek, nincs látható sérülésük, amely csökkentené a biztonságot", a.Ellen5_Sz_C, a.Ellen5_Sz_C_M),
            new("a) Áramütés elleni védelem kialakítása (védelmi mód, nullavezetők, védővezetők)", a.Ellen5_Me_A, a.Ellen5_Me_A_M),
            new("b) Tűzvédelmi óvintézkedések és hőhatás elleni védelmek", a.Ellen5_Me_B, a.Ellen5_Me_B_M),
            new("c) Vezetők megfelelő megválasztása", a.Ellen5_Me_C, a.Ellen5_Me_C_M),
            new("d) Védelmi és ellenőrző eszközök kiválasztása és beállítása", a.Ellen5_Me_D, a.Ellen5_Me_D_M),
            new("e) Túlfeszültségvédelmi eszközök megléte és elhelyezése", a.Ellen5_Me_E, a.Ellen5_Me_E_M),
            new("f) Leválasztó és kapcsoló eszközök megléte és elhelyezése", a.Ellen5_Me_F, a.Ellen5_Me_F_M),
            new("g) Villamos szerkezetek és védelmi módok kiválasztása", a.Ellen5_Me_G, a.Ellen5_Me_G_M),
            new("h) N- és PE-vezető jelölése", a.Ellen5_Me_H, a.Ellen5_Me_H_M),
            new("i) Kapcsolási rajzok és figyelmeztető feliratok megléte", a.Ellen5_Me_I, a.Ellen5_Me_I_M),
            new("j) Áramkörök, készülékek, csatlakozó- és sorozatkapcsok jelölése", a.Ellen5_Me_J, a.Ellen5_Me_J_M),
            new("k) Vezetők csatlakozásainak megfelelősége", a.Ellen5_Me_K, a.Ellen5_Me_K_M),
            new("l) Földelők, PE-vezetők és csatlakozásuk", a.Ellen5_Me_L, a.Ellen5_Me_L_M),
            new("m) Szerkezetek könnyen azonosíthatók és jól hozzáférhetők", a.Ellen5_Me_M, a.Ellen5_Me_M_M),
            new("n) EMC elleni intézkedések", a.Ellen5_Me_N, a.Ellen5_Me_N_M),
            new("o) Megérinthető fémrészek csatlakoznak a földelő rendszerhez", a.Ellen5_Me_O, a.Ellen5_Me_O_M),
            new("p) Vezetékek, huzalozás", a.Ellen5_Me_P, a.Ellen5_Me_P_M),
        };

        EllenorzesTablazat(container, jkvSzam, "A vizsgálati eredmények összefoglalása 1/2", "az MSZ HD 60364-6:2017 szabvány szerint", sorok, a.Ellen5_Megjegyzes);
    }

    // ============================================================
    // 5b. OLDAL – VIZSGÁLATI EREDMÉNYEK ÖSSZEFOGLALÁSA 2/2
    // ============================================================

    private void VizsgalatiEredmenyek2(IContainer container, JegyzokonyvAdatok a, string jkvSzam)
    {
        var sorok = new List<EllenorzesSor>
        {
            new("a) Vezetők folytonossága", a.Ellen5_Mr_A, a.Ellen5_Mr_A_M),
            new("b) Villamos berendezés szigetelési ellenállása", a.Ellen5_Mr_B, a.Ellen5_Mr_B_M),
            new("c) SELV és PELV és villamos elválasztás megvalósítása", a.Ellen5_Mr_C, a.Ellen5_Mr_C_M),
            new("d) Padlózat és fal ellenállása/impedanciája", a.Ellen5_Mr_D, a.Ellen5_Mr_D_M),
            new("e) Polaritás ellenőrzése", a.Ellen5_Mr_E, a.Ellen5_Mr_E_M),
            new("f) Táplálás önműködő lekapcsolása", a.Ellen5_Mr_F, a.Ellen5_Mr_F_M),
            new("g) Kiegészítő védelmek ellenőrzése", a.Ellen5_Mr_G, a.Ellen5_Mr_G_M),
            new("h) Fázissorrend ellenőrzése", a.Ellen5_Mr_H, a.Ellen5_Mr_H_M),
            new("i) Üzemszerű funkciók és működés ellenőrzése", a.Ellen5_Mr_I, a.Ellen5_Mr_I_M),
            new("j) Feszültségesés ellenőrzése", a.Ellen5_Mr_J, a.Ellen5_Mr_J_M),
        };

        EllenorzesTablazat(container, jkvSzam, "A vizsgálati eredmények összefoglalása 2/2", "Műszeres vizsgálatok az MSZ HD 60364-6:2017 szabvány szerint", sorok, a.Ellen5_Megjegyzes);
    }

    // ============================================================
    // 6. OLDAL – OTSZ ELLENŐRZÉS
    // ============================================================

    private void OtszEllenorzes(IContainer container, JegyzokonyvAdatok a, string jkvSzam)
    {
        var sorok = new List<EllenorzesSor>
        {
            new("a) Érvényes tűzveszélyességi vagy kockázati osztályba sorolás megléte", a.Ellen6_A, a.Ellen6_A_M),
            new("b) Gépészeti és villamos átvezetések tömítése", a.Ellen6_B, a.Ellen6_B_M),
            new("c) Biztonsági világítás megléte", a.Ellen6_C, a.Ellen6_C_M),
            new("d) Napelemek: kapcsolók, feliratok megléte", a.Ellen6_D, a.Ellen6_D_M),
            new("e) Hő és füst elleni védelem vezérlőtábla megléte", a.Ellen6_E, a.Ellen6_E_M),
            new("f) Alagutak és felszín alatti vasutak speciális előírásainak teljesítése", a.Ellen6_F, a.Ellen6_F_M),
            new("g) Központi ill. részegységenkénti tűzeseti lekapcsolás kialakítása", a.Ellen6_G, a.Ellen6_G_M),
            new("h) Középfeszültségről táplált KK/MK osztályú épületek követelményei", a.Ellen6_H, a.Ellen6_H_M),
            new("i) Tűzeseti fogyasztók kialakítása, működőképessége", a.Ellen6_I, a.Ellen6_I_M),
            new("j) Villámvédelem NV vagy 274V megléte, ellenőrzése, dokumentálása", a.Ellen6_J, a.Ellen6_J_M),
            new("k) Elektrosztatikus feltöltődés elleni védelem, ellenőrzése, dokumentálása", a.Ellen6_K, a.Ellen6_K_M),
            new("l) Beépített tűzjelző berendezés felülvizsgálata, dokumentálása", a.Ellen6_L, a.Ellen6_L_M),
            new("m) Beépített tűzoltó berendezés felülvizsgálata, dokumentálása", a.Ellen6_M, a.Ellen6_M_M),
            new("n) Villamos berendezés nem okoz gyújtásveszélyt, kikapcsolás, leválasztás", a.Ellen6_N, a.Ellen6_N_M),
            new("o) Egyéb OTSZ előírás", a.Ellen6_O, a.Ellen6_O_M),
            new("p) Egyéb OTSZ előírás", a.Ellen6_P, a.Ellen6_P_M),
        };

        EllenorzesTablazat(container, jkvSzam, "Az OTSZ létesítési előírásainak ellenőrzése", "az 54/2014. (XII.5.) BM rendelet (OTSZ 5.2) alapján", sorok, a.Ellen6_Megjegyzes);
    }

    // ============================================================
    // 6b. OLDAL – VMBSZ ELLENŐRZÉS
    // ============================================================

    private void VmbszEllenorzes(IContainer container, JegyzokonyvAdatok a, string jkvSzam)
    {
        var sorok = new List<EllenorzesSor>
        {
            new("1. A villamos berendezések műszaki biztonsági követelményei", a.Ellen6V_01, a.Ellen6V_01_M),
            new("3. Összekötő berendezések kivitelezése", a.Ellen6V_02, a.Ellen6V_02_M),
            new("3.1. Átkapcsolható vezeték szakaszok: fázissorrend", a.Ellen6V_03, a.Ellen6V_03_M),
            new("3.2. Összekapcsolható vezetékszakaszok: párhuzamos kapcsolása", a.Ellen6V_04, a.Ellen6V_04_M),
            new("3.3. Nem azonos fázishelyzetű hálózatok összekötés tiltása", a.Ellen6V_05, a.Ellen6V_05_M),
            new("3.4. Illetéktelenek működtetésének megakadályozása", a.Ellen6V_06, a.Ellen6V_06_M),
            new("3.8. Életveszélyt okozó üzemzavar esetén szükséges intézkedések", a.Ellen6V_07, a.Ellen6V_07_M),
            new("3.10. Felhasználói belső szabályzatok megléte", a.Ellen6V_08, a.Ellen6V_08_M),
            new("3.16. Összekötő berendezések kezelési utasításának megléte", a.Ellen6V_09, a.Ellen6V_09_M),
            new("3.22. Új összekötő berendezések: védővezető kiépítése", a.Ellen6V_10, a.Ellen6V_10_M),
            new("3.23. Biztosító betétek értékeinek jelölése vagy kapcsolási rajz", a.Ellen6V_11, a.Ellen6V_11_M),
            new("4.1. Felvonulási területek berendezéseinek biztonságos kivitele", a.Ellen6V_12, a.Ellen6V_12_M),
            new("4.2. Rögzítetten bekötött berendezések üzembe helyezése", a.Ellen6V_13, a.Ellen6V_13_M),
            new("4.9. Áram-védőkapcsolók ellenőrzése és bizonylatolása", a.Ellen6V_14, a.Ellen6V_14_M),
            new("4.10. Nem megfelelőségek javítása, dokumentálása és megléte", a.Ellen6V_15, a.Ellen6V_15_M),
            new("6.1.2. Egyedi villamos szerkezetek: kivitelhez szükséges nyilatkozatok", a.Ellen6V_16, a.Ellen6V_16_M),
            new("6.2. Javítás utáni vizsgálatokról készült jegyzőkönyvek ellenőrzése", a.Ellen6V_17, a.Ellen6V_17_M),
            new("Villamos szerkezetek: biztonsági követelmények teljesülésének igazolása", a.Ellen6V_18, a.Ellen6V_18_M),
            new("Kisfeszültségű termékek: vizsgálati tanúsítványok, CE-jel alkalmazása", a.Ellen6V_19, a.Ellen6V_19_M),
            new("Nagyfeszültségű termékek: gyártói nyilatkozat, tanúsítványok", a.Ellen6V_20, a.Ellen6V_20_M),
            new("RB-kivitelű termékek: ATEX tanúsítványok, EU-megfelelőségi nyilatkozat, CE-jel", a.Ellen6V_21, a.Ellen6V_21_M),
        };

        EllenorzesTablazat(container, jkvSzam, "A VMBSZ létesítési előírásainak ellenőrzése", "az MSZ EN 60364 szabványsorozat tárgykörébe tartozó berendezéseken", sorok, a.Ellen6V_Megjegyzes);
    }

    // ============================================================
    // 7. OLDAL – A VÉDELMEK ELLENŐRZÉSE
    // ============================================================

    private void VedelmekEllenorzese(IContainer container, JegyzokonyvAdatok a, string jkvSzam)
    {
        container.Column(col =>
        {
            OldalFejlecDoboz(col, "A védelmek ellenőrzése");

            col.Item().Border(1).Padding(4).Row(row =>
            {
                row.AutoItem().Text("Azonosító adatok:").SemiBold();
                row.RelativeItem().PaddingLeft(8).Text(jkvSzam ?? "");
            });

            col.Item().Border(1).Padding(4).Text("Jelen fejezet a villamos védelmek vizsgálati összefoglalóban nem érintett kérdéseivel foglalkozik, illetve azokat szükség szerint kiegészíti, értékeli.").FontSize(8).Italic();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(140);
                    c.RelativeColumn();
                });

                void Sor(string cimke, string ertek)
                {
                    table.Cell().Border(1).Padding(4).Text(cimke).SemiBold();
                    table.Cell().Border(1).Padding(4).Text(ertek ?? "");
                }

                Sor("Balesetvédelem:", a.Ellen7_Balesetvédelem);
                Sor("Túláramvédelem:", a.Ellen7_TulaAramvedelem);
                Sor("Áramütés elleni védelem:", a.Ellen7_AramutesElleni);
                Sor("Villámvédelem:", a.Ellen7_Villamvedelem);
                Sor("Túlfeszültség-védelem:", a.Ellen7_Tulfeszultseg);
                Sor("Feszültségcsökkenés elleni védelem:", a.Ellen7_Feszultsegcsokkenes);
                Sor("Elektrosztatikus feltöltődés elleni védelem:", a.Ellen7_Elektrosztatikus);
                Sor("Megjegyzések, észrevételek:", a.Ellen7_Megjegyzes);
                Sor("A védelmek átfogó, rendszerszintű értékelése:", a.Ellen7_AtfogoErtekeles);
            });
        });
    }

    // ============================================================
    // 8. OLDAL – ÁRAMKÖRÖK LEÍRÁSA HELYISÉGENKÉNT
    // ============================================================

    private void AramkorokLeirasa(IContainer container, JegyzokonyvAdatok a)
    {
        container.Column(col =>
        {
            OldalFejlecDoboz(col, "Áramkörök leírása helyiségenként");

            col.Item().Border(1).Padding(4).Text("Jelen fejezet rögzíti a helyszíni felülvizsgálat során megtekintett villamos szerkezeteket, áramköröket, illetve helyiségek szerint.").FontSize(8).Italic();

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(120);
                    c.RelativeColumn();
                });

                table.Cell().Border(1).Padding(4).Text("Általános, észrevételek:").SemiBold();
                table.Cell().Border(1).Padding(4).Text(string.IsNullOrWhiteSpace(a.Ellen7_AltalánosEszrevételek)
                    ? "A berendezések és védelmek kialakítása a vonatkozó műszaki előírásoknak és a szabványok követelményeinek maradéktalanul megfelel."
                    : a.Ellen7_AltalánosEszrevételek);

                var aramkorok = a.Aramkorok ?? new List<AramkorSor>();
                if (aramkorok.Any())
                {
                    foreach (var sor in aramkorok)
                    {
                        table.Cell().Border(1).Padding(4).Text("Szabadtér:").SemiBold();
                        table.Cell().Border(1).Padding(4).Text($"{sor.HelyisegNev} {sor.Leiras}".Trim());
                    }
                }
                else
                {
                    table.Cell().Border(1).Padding(4).Text("Szabadtér:").SemiBold();
                    table.Cell().Border(1).Padding(4).Text("Nincs rögzített áramkör.");
                }

                table.Cell().Border(1).Padding(4).Text("Megjegyzések, észrevételek:").SemiBold();
                table.Cell().Border(1).Padding(4).Text(a.Ellen7_MegjegyzesekEszrevételek ?? "");
            });
        });
    }

    // ============================================================
    // 9. OLDAL – ÚTMUTATÓ A MEGRENDELŐNEK
    // ============================================================

    private void Utmutato(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().AlignCenter().Text("ÚTMUTATÓ A MEGRENDELŐNEK").Bold().FontSize(13);

            col.Item().PaddingTop(10).Text("E dokumentáció fontos és értékes dokumentum, amelyet meg kell őrizni.").SemiBold();
            col.Item().PaddingTop(2).Text("Ez a dokumentáció a meglévő villamos berendezések állapotának jegyzőkönyvezésére szolgál.").SemiBold();

            void Pont(int szam, string szoveg)
            {
                col.Item().PaddingTop(8).Row(row =>
                {
                    row.ConstantItem(20).Text($"{szam})").SemiBold();
                    row.RelativeItem().Text(szoveg);
                });
            }

            Pont(1, "Ennek az állapotfelmérő dokumentációnak az a célja, hogy lehetőség szerint megállapítsa azt, hogy az adott villamos berendezés a további működés szempontjából kielégítő állapotban van-e vagy sem. A dokumentáció azonosít minden olyan károsodást, állagromlást, hibát és/vagy állapotot, amely növelheti a veszélyt.");
            Pont(2, "A dokumentációt megrendelő személy kap a dokumentációról egy eredeti példányt és a felülvizsgálatot megtartó egy másodpéldányt.");
            Pont(3, "Az eredeti dokumentációt biztonságos helyen kell őrizni, és minden olyan személynek a rendelkezésére kell bocsátani, aki a jövőben a berendezést felülvizsgálja, vagy azon munkát végez. Ha az ingatlan gazdát vált, akkor ezt a dokumentációt az új tulajdonossal/bérlővel meg kell ismertetni a villamos berendezéseknek a legutóbbi felülvizsgálat idején érvényes állapotának részleteivel együtt.");
            Pont(4, "A dokumentációból mértékben azonosítani lehessen a berendezést és azt a kiterjedést, amelyre a vizsgálat vonatkozik, valamint a felülvizsgálat esetleges korlátozásait. A felülvizsgálónak ezenkívül a dokumentációt megrendelő személlyel és más érdekelt felekkel (engedélyező hatóság, biztosítótársaság, jelzálog-hitelező és hasonlók) meg kell állapodnia még a felülvizsgálat elvégzése előtt.");
            Pont(5, "A felülvizsgálat során előfordulhatnak olyan üzemeltetési korlátozások, mint pl. a berendezés vagy a szerkezetek részeinek szemrevételezése, amely a berendezés vagy szerkezetek részleges vagy teljes üzemen kívül helyezésével jár.");
            Pont(6, "A „Veszély fenn” észrevételek azt jelentik, hogy a berendezés használatra veszélyes, és javasolt, hogy kompetens személy azonnal elvégezze a hibajavítást.");
            Pont(7, "A „Potenciálisan veszélyes” észrevételek azt jelentik, hogy a berendezés használata veszélyes lehet, hogy kompetens személy sürgősen végezze el a hibajavítást.");
            Pont(8, "Ha a felülvizsgálat további vizsgálatot ír elő, mert a felülvizsgáló olyan szempontot emelt ki, amelyet az azonnali szemrevételezés kiterjedése és korlátozásai nem tettek lehetővé teljes mértékben azonosítani, akkor ilyen esetekben haladéktalanul további vizsgálatra van szükség a nyilvánvaló hiányosság természetének és mértékének megállapítására.");
            Pont(9, "Biztonsági okokból a villamos berendezést megfelelő időközönként kompetens személynek ismételten felül kell vizsgálnia. A következő felülvizsgálat javasolt időpontja a dokumentáción fel van tüntetve.");
        });
    }
}
