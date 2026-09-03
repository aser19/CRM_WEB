using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Az "Egy mérés" jegyzőkönyv PDF generálása. A meglévő Word export (JegyzokonyvWordService) mellett
/// egy alternatív, közvetlenül letölthető PDF-et állít elő, amely a "VILLAMOS BIZTONSÁGI FELÜLVIZSGÁLAT /
/// VILLAMOS BERENDEZÉS ELSŐ ELLENŐRZÉSÉNEK JELENTÉSE – MSZ HD 60364-6:2017" mintát követi.
/// </summary>
public class EgyMeresPdfService : IEgyMeresPdfService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly IFileStorageService _fileStorageService;
    private byte[]? _cegBelyegzoKep;
    private byte[]? _felulvizsgaloAlairasKep;

    public EgyMeresPdfService(IMeresService meresService, ITenantService tenantService, ICegService cegService,
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

        var cegNev = adatok.CegNev;
        var cegCim = adatok.CegCim;
        if (string.IsNullOrWhiteSpace(cegNev) || string.IsNullOrWhiteSpace(cegCim))
        {
            try
            {
                var ceg = await _cegService.GetByIdAsync(_tenantService.GetCurrentCegId());
                cegNev = string.IsNullOrWhiteSpace(cegNev) ? ceg?.Nev ?? "" : cegNev;
                cegCim = string.IsNullOrWhiteSpace(cegCim) ? ceg?.Cim ?? "" : cegCim;
            }
            catch { /* nincs tenant kontextus */ }
        }

        var jkvSzam = !string.IsNullOrWhiteSpace(adatok.JegyzokonyvSzam)
            ? adatok.JegyzokonyvSzam
            : $"EGYM-{meresId:D6}/{DateTime.Now:yyyy}";
        var vizsgalatHelye = !string.IsNullOrWhiteSpace(adatok.VizsgalatHelye) ? adatok.VizsgalatHelye : meres?.Telephely?.Cim ?? "";
        var meresIdeje = meres?.Datum ?? DateTime.Today;
        var jegyzokonyvKeszultDatum = meres?.Letrehozva ?? meresIdeje;
        var meresiPontok = adatok.MeresiPontok ?? new List<MeresiPontSor>();
        var avkSorok = adatok.AvkSorok ?? new List<AvkSor>();
        var vanAvk = avkSorok.Any() || meresiPontok.Any(p => p.AVKCsatolva);

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

        var kovetkezoFelulvizsgalatDatum = SzamitottKovetkezoFelulvizsgalat(meresIdeje, adatok.KovetkezoFelulvizsgalatEgyMeresIdoszak);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Element(c => Tartalom(c, meresiPontok, avkSorok, adatok, jkvSzam, cegNev, cegCim,
                    vizsgalatHelye, meresIdeje, jegyzokonyvKeszultDatum, jogosultsagIgazolas, kovetkezoFelulvizsgalatDatum, vanAvk));
            });

            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                page.Content().Element(TajekoztatoOldal);
            });
        }).GeneratePdf();
    }

    private void TajekoztatoOldal(IContainer container)
    {
        container.DefaultTextStyle(x => x.FontSize(6.3f).LineHeight(1.05f)).Column(col =>
        {
            col.Spacing(2);

            col.Item().AlignCenter().Text("ÚTMUTATÓ A MEGRENDELŐNEK").Bold().FontSize(9);
            col.Item().AlignCenter().Text("E dokumentáció fontos és értékes dokumentum, amelyet meg kell őrizni.").Bold();

            col.Item().PaddingTop(2).Text(
                "Jelen dokumentum megfelel a 40/2017. (XII. 4.) NGM rendelet 2. §-33. pontjának, villamos biztonsági felülvizsgálat, a villamos berendezések olyan részletes, a mérésekkel és azok kiszámított eredményének kiértékelésével is alátámasztott, " +
                "különleges erősségű villamos szakképzettséget igénylő ellenőrzése, amely alkalmas arra, hogy kimutassa, teljesíti-e az a vonatkozó szabványok vagy azokkal egyenértékű műszaki megoldásokat tartalmazó műszaki előírások, valamint egyéb " +
                "kritérium, továbbá a villamos berendezések élet- és vagyonbiztonsági szempontból lényeges, teljes körű felülvizsgálat, amely magába foglalja a villamos berendezések szerkezetének elleni védelmének és az általános szabványos " +
                "állapotának (tűzvédelmi jellegű) vizsgálat – 27/2020. (VII. 16.) ITM rendelet általi módosítás.");

            col.Item().PaddingTop(3).Text("TULAJDONOS, ÜZEMELTETŐ, FELELŐS VEZETŐ, FELHASZNÁLÓ KÖTELESSÉGEI:").Bold();

            col.Item().Text("• Tárgyi villamos berendezés rendeltetésszerű használata, illetve ennek biztosítása kötelező");
            col.Item().Text("• Tárgyi villamos berendezés időszakos karbantartása és a tárgyi villamos berendezésre vonatkozó jogszabályok előírt felülvizsgálatok és ellenőrzések elvégzése, illetve elvégeztetése kötelező");
            col.Item().Text("• Tárgyi villamos berendezésen villamos szakképzettséget igénylő beavatkozást, változtatást, felújítást, karbantartást és egyéb tevékenységet csak villamosan szakképzett és arra felhatalmazott személy végezhet a hatályos jogszabályok előírásainak és vonatkozó szabványok betartásával");
            col.Item().Text("• Tárgyi villamos berendezés történt bármilyen szakszerű változtatás után az érintett szakaszon és a változtatás által érintésvédelmi szempontból érintett, már meglévő szakaszon az előírt érintésvédelemmel kapcsolatos vizsgálatokat el kell végezni – az erre vonatkozó jogszabályokat és előírásokat, illetve ezek változását nyomon kell követni és be kell tartani, illetve tartatni.");
            col.Item().Text("• 40/2017. (XII. 4.) NGM rendelet az összekötő és felhasználói berendezésekről, valamint a potenciálisan robbanásveszélyes közegben működő villamos berendezésekről és védelmi rendszerekről");
            col.Item().Text("• 10/2016. (IV. 5.) NGM rendelet 19. § előírásaink betartása – amennyiben vonatkoznak");
            col.Item().Text("• 54/2014. (XII. 5.) BM rendeletben foglalt vonatkozó követelmények, előírások megtartása");
            col.Item().Text("• Létesítmény felelős vezetője pluszkövetelményeket megszabhat");

            col.Item().PaddingTop(3).Text("A felülvizsgálattal kapcsolatos előírások:").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.ConstantColumn(20);
                    c.RelativeColumn(7);
                });
                void Sor(string rendelet, string leiras)
                {
                    table.Cell().Text(rendelet);
                    table.Cell().AlignCenter().Text("→");
                    table.Cell().Text(leiras);
                }
                Sor("1995. évi XXVIII. törvény", "a nemzeti szabványosításról, módosította: 2011. évi CXII. törvény");
                Sor("191/2009. (IX.-15.) Korm. rendelet", "az építőipari kivitelezési tevékenységről");
                Sor("34/2021. (VII.-26.) ITM rendelet", "egyes ipari és kereskedelmi tevékenységek gyakorlásához szükséges képesítésekről, valamint egyes műszaki szabályozási tárgyú miniszteri rendeletek módosításáról");
                Sor("10/2016. (IV.-5.) NGM rendelet", "a munkaeszközök és használatuk biztonsági és egészségügyi követelményeinek minimális szintjéről");
                Sor("40/2017. (XII.-4.) NGM rendelet", "az összekötő és felhasználói berendezésekről, valamint a potenciálisan robbanásveszélyes közegben működő villamos berendezésekről és védelmi rendszerekről");
                Sor("54/2014. (XII.-5.) BM rendelet", "az Országos Tűzvédelmi Szabályzatról");
            });

            col.Item().PaddingTop(3).Text("A felülvizsgálattal kapcsolatos főbb szabványok:").Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.ConstantColumn(20);
                    c.RelativeColumn(7);
                });
                void Sor(string szabvany, string leiras)
                {
                    table.Cell().Text(szabvany);
                    table.Cell().AlignCenter().Text("→");
                    table.Cell().Text(leiras);
                }
                Sor("MSZ 1585:2016", "Villamos berendezések üzemeltetése");
                Sor("MSZ EN 61557-1:2007", "Általános követelmények");
                Sor("MSZ HD 60364-1:2009", "Alapelvek, általános jellemzők elemzése, fogalmak");
                Sor("MSZ EN 61140:2016", "Az áramütés elleni védelem. A villamos berendezésekre és a villamos berendezésekre vonatkozó közös szempontok");
                Sor("MSZ 10900:2009", "Kisfeszültségű villamos berendezések időszakos (tűzvédelmi) ellenőrzése");
            });

            col.Item().PaddingTop(3).Text("1) Ennek az állapotfelmérő dokumentációnak az a célja, hogy lehetőség szerint megállapítsa azt, hogy az adott villamos berendezés a további működés szempontjából kielégítő állapotban van vagy sem. A dokumentáció azonosít minden olyan károsodást, állagromlást, hibát és/vagy vagy állapotot, amely növelheti a veszélyt.");
            col.Item().Text("2) A dokumentáció megrendelő személy kap a dokumentációból egy eredeti példányt és a felülvizsgáló megtart egy másodpéldányt.");
            col.Item().Text("3) Az eredeti dokumentációt biztonságos helyen meg kell őrizni, és minden olyan személynek a rendelkezésre kell bocsátani, aki a jövőben a villamos berendezést felülvizsgálja vagy azon munkát végez. Ha az ingatlan megüresedik, akkor ezt a dokumentációt az új tulajdonosnak/bérlővel meg kell ismertetni a villamos berendezésnek a dokumentáció szerinti állapotának releváns egyveit.");
            col.Item().Text("4) Fontos teljes mértékben azonosítani a berendezésnek azt a kiterjedését, amelyre a dokumentáció vonatkozik, valamint a felülvizsgálat esetleges korlátozásait. A felülvizsgálónak ezekről a dokumentációt megrendelő személlyel és más érdekelt felekkel (engedélyező hatóság, biztosítótársaság, jelzálog-hitelező és hasonlók) meg kell állapodnia még a felülvizsgálat elvégzése előtt.");
            col.Item().Text("5) A felülvizsgálat során előfordulhatnak olyan üzemeltetési korlátozások, mint pl. a berendezés vagy a szerkezetek egyes részeinek a hozzáférhetetlensége. A felülvizsgáló ezeket a dokumentációban feltünteti.");
            col.Item().Text("6) A „Veszély áll fenn” észrevétele azt jelenti, hogy a berendezés használata veszélyes, és javasolt, hogy a kompetens személy sürgősen végezze el a hibajavítást.");
            col.Item().Text("7) A „Potenciálisan veszélyes” észrevétel azt jelenti, hogy a berendezés használata veszélyes lehet, és javasolt, hogy a kompetens személy a hibajavítást megvizsgálja és elvégezze a hibajavítást.");
            col.Item().Text("8) Ha a dokumentáció további vizsgálatot ír elő, mert a felülvizsgálat olyan nyilvánvaló hiányosságot tárt fel, amely azt bizonyítja, hogy egy adott szemrevételezés kiterjedése és korlátozásai miatt nem lehet teljes mértékben azonosítani, akkor ilyen esetekben haladéktalanul további vizsgálatra van szükség a nyilvánvaló hiányosság természetének és mértékének megállapítására.");
            col.Item().Text("9) Biztonsági okokból a villamos berendezés megfelelő időközönként kompetens személynek ismételten felül kell vizsgálnia. A következő felülvizsgálat javasolt időpontja a dokumentációban fel van tüntetve.");

            col.Item().PaddingTop(3).Text(
                "Jelen dokumentum elkészítése során betartotta a 40/2017. (XII. 4.) NGM rendelet Villamos biztonsági szabályzatának 1.13.4. pontját, mely szerint: „A villamos berendezés felülvizsgálata a felülvizsgálat idején érvényes vonatkozó " +
                "műszaki követelmény szerint történik”. A villamos berendezések minősítése a létesítés idején érvényes vonatkozó műszaki követelmény szerint történik.");
            col.Item().Text(
                "A rendelet 1.13.5 pontja kijelenti: „Az e rendelet hatálybalépése előtt létesített – a vizsgálatkor érvényes műszaki előírásoknak meg nem felelő – berendezés esetében a felülvizsgálat során tapasztalt hiányosságot pótolható a villamos " +
                "biztonsági felülvizsgálat végzője által a minősítő iratban meghatározott időpontban, ennek hiányában a villamos berendezés soron következő felülvizsgálata idején érvényes vonatkozó követelményeknek megfelelően végzendő el”.");
        });
    }

    private static DateTime? SzamitottKovetkezoFelulvizsgalat(DateTime meresIdeje, string? idoszak)
    {
        return idoszak switch
        {
            "3honap" => meresIdeje.AddMonths(3),
            "6honap" => meresIdeje.AddMonths(6),
            "9honap" => meresIdeje.AddMonths(9),
            "1ev" => meresIdeje.AddYears(1),
            "3ev" => meresIdeje.AddYears(3),
            "6ev" => meresIdeje.AddYears(6),
            "9ev" => meresIdeje.AddYears(9),
            _ => null
        };
    }

    private void Tartalom(IContainer container, List<MeresiPontSor> meresiPontok, List<AvkSor> avkSorok,
        JegyzokonyvAdatok adatok, string jkvSzam, string cegNev, string cegCim, string vizsgalatHelye,
        DateTime meresIdeje, DateTime jegyzokonyvKeszultDatum, string jogosultsagIgazolas, DateTime? kovetkezoFelulvizsgalat, bool vanAvk)
    {
        container.Border(1).BorderColor(Colors.Black).Column(col =>
        {
            // === CÍM BLOKK ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Column(c =>
            {
                c.Item().AlignCenter().Text("VILLAMOS BIZTONSÁGI FELÜLVIZSGÁLAT").Bold().FontSize(11);
                c.Item().AlignCenter().Text("VILLAMOS BERENDEZÉS ELSŐ RÉSZLEGES ELLENŐRZÉSÉNEK JELENTÉSE – MSZ HD 60364-6:2017").Bold().FontSize(9);
            });

            // === FELÜLVIZSGÁLAT HELYE + BELSŐS KARBANTARTÓ ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(3).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("A felülvizsgálat helye: ").SemiBold();
                        text.Span($"{cegNev} – Székhely – {cegCim}".Trim(' ', '–'));
                    });
                    c.Item().PaddingTop(2).Text("Jelen felülvizsgálat és mérés kizárólag a javítással, helyreállítással vagy karbantartással érintett részletesen felsorolt villamos berendezésre / eszközre terjedt ki.")
                        .Italic().FontSize(7);
                });
                row.RelativeItem(1).Padding(4).Column(c =>
                {
                    c.Item().Text("Kapcsolattartó:").SemiBold();
                    c.Item().Text(adatok.KapcsolatTarto ?? "");
                });
            });

            // === MÉRÉST VÉGEZTE / BELSŐ AZONOSÍTÓ SZÁM / MŰSZER ADATAI ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(1.4f).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text("Mérést végezte:").SemiBold();
                    c.Item().PaddingTop(2).Text(text =>
                    {
                        text.Span("Név: ").SemiBold();
                        text.Span(adatok.FelulvizsgaloNev ?? "");
                    });
                    c.Item().Text("Végzettség: felülvizsgáló");
                    c.Item().Text(text =>
                    {
                        text.Span("Jogosultság igazolása: ").SemiBold();
                        text.Span(jogosultsagIgazolas);
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Hálózat típusa: ").SemiBold();
                        text.Span(adatok.MeresiRendszerTipus ?? "TN");
                    });
                });
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text("Belső azonosító szám:").SemiBold();
                    c.Item().PaddingTop(2).Text(adatok.UzemiKisero ?? "");
                });
                row.RelativeItem(1.2f).Padding(4).Column(c =>
                {
                    c.Item().Text("Mérésnél alkalmazott műszer adatai:").SemiBold();
                    var muszer = adatok.Muszerek?.FirstOrDefault();
                    var tipus = muszer?.Tipus ?? adatok.Muszer1Tipus ?? "";
                    var gyariSzam = muszer?.GyariSzam ?? adatok.Muszer1GyariSzam ?? "";
                    var kalibralas = muszer?.Kalibralas ?? adatok.Muszer1Kalibralas ?? "";
                    c.Item().Text($"Típus: {tipus}");
                    c.Item().Text($"Gyári szám: {gyariSzam}");
                    c.Item().Text($"Kalibrálási adatok: {kalibralas}");
                });
            });

            // === VIZSGÁLT BERENDEZÉS, ILLETVE ÁRAMKÖR CÍM ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(3)
                .AlignCenter().Text("VIZSGÁLT BERENDEZÉS, ILLETVE ÁRAMKÖR").Bold().FontSize(9);

            // === MÉRÉSI PONTOK TÁBLÁZAT ===
            col.Item().Element(c => MeresiPontokTablazat(c, meresiPontok));

            // === KÖVETKEZŐ FELÜLVIZSGÁLAT SOR ===
            col.Item().BorderTop(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(2).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text(text =>
                    {
                        text.Span("Következő felülvizsgálat jogszabály szerinti időpontja: ").SemiBold();
                        text.Span(kovetkezoFelulvizsgalat.HasValue ? kovetkezoFelulvizsgalat.Value.ToString("yyyy.MM.dd") : "-");
                    });
                row.RelativeItem(1).Padding(4).Text("");
            });

            // === KELT / MUNKASZÁM / MINŐSÍTÉS / FELELŐS FELÜLVIZSGÁLÓ ===
            var minosites = meresiPontok.Any(p => !string.Equals(p.Minosites, "MEGFELELT", StringComparison.OrdinalIgnoreCase)
                                                    && !string.Equals(p.Minosites, "MEGFELEL", StringComparison.OrdinalIgnoreCase))
                ? "NEM FELELT MEG"
                : "MEGFELELT";

            col.Item().BorderTop(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text($"Kelt: {jegyzokonyvKeszultDatum:yyyy.MM.dd}");
                    c.Item().Text($"Mérés ideje: {meresIdeje:yyyy.MM.dd}");
                    c.Item().Text($"Letöltés időpontja: {DateTime.Today:yyyy.MM.dd}");
                });
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text($"Munkaszám: {jkvSzam}");
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text($"Minősítés: {minosites}");
                row.RelativeItem(1.4f).Padding(4).Column(c =>
                {
                    c.Item().Text("Felelős felülvizsgáló:").SemiBold();
                    c.Item().Text(adatok.FelulvizsgaloNev ?? "");
                    c.Item().Row(r =>
                    {
                        if (_felulvizsgaloAlairasKep != null)
                        {
                            r.RelativeItem().AlignCenter().Height(25).Image(_felulvizsgaloAlairasKep).FitArea();
                        }
                        if (_cegBelyegzoKep != null)
                        {
                            r.RelativeItem().AlignCenter().Height(30).Image(_cegBelyegzoKep).FitArea();
                        }
                    });
                });
            });

            // === ÁVK BLOKK (feltételes) ===
            if (vanAvk)
            {
                col.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Black).Element(c => AvkTablazat(c, avkSorok));
            }

            if (!string.IsNullOrWhiteSpace(adatok.Megjegyzes))
            {
                col.Item().BorderTop(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text("Megjegyzések, észrevételek:").SemiBold().FontSize(8);
                    c.Item().Text(adatok.Megjegyzes).FontSize(8);
                });
            }
        });
    }

    private void MeresiPontokTablazat(IContainer container, List<MeresiPontSor> meresiPontok)
    {
        // Dinamikus sőrőméretés: sok mérési pont esetén kisebb padding és betűméret, hogy minden ráférjen az oldalra.
        var sorokSzama = meresiPontok.Count + meresiPontok.Select(p => p.HelyisegNev).Distinct().Count(n => !string.IsNullOrWhiteSpace(n));
        float cellaPadding;
        float betuMeret = 7f;
        if (sorokSzama > 40)
        {
            cellaPadding = 1f;
        }
        else if (sorokSzama > 28)
        {
            cellaPadding = 1.5f;
        }
        else if (sorokSzama > 18)
        {
            cellaPadding = 2f;
        }
        else
        {
            cellaPadding = 3f;
        }

        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn(3.2f); // Mérési pont helye, megnevezése, egyéb közlendő adat
                c.RelativeColumn(1.1f); // Mód/Oszt.
                c.RelativeColumn(1.4f); // Kioldószerv - Túláramvédelmi szerv Helye
                c.RelativeColumn(1.4f); // Kioldószerv - Túláramvédelmi szerv Típus
                c.ConstantColumn(28);   // ÁVK
                c.ConstantColumn(35);   // PE folyt.
                c.ConstantColumn(38);   // ÉRTÉK [Ω]
                c.RelativeColumn(1.3f); // MINŐSÍTÉS
            });

            table.Header(header =>
            {
                FejlecCella(header, "MÉRÉSI PONT HELYE, MEGNEVEZÉSE, EGYÉB KÖZLENDŐ ADAT\n(vezeték adatai, áramkör tervjele stb.)", cellaPadding, betuMeret);
                FejlecCella(header, "MÓD/OSZT", cellaPadding, betuMeret);
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nHelye", cellaPadding, betuMeret);
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nTípus (In, kar.)", cellaPadding, betuMeret);
                FejlecCella(header, "ÁVK", cellaPadding, betuMeret);
                FejlecCella(header, "PE folyt.", cellaPadding, betuMeret);
                FejlecCella(header, "ÉRTÉK [Ω]", cellaPadding, betuMeret);
                FejlecCella(header, "MINŐSÍTÉS", cellaPadding, betuMeret);
            });

            string? aktualisHelyiseg = null;
            foreach (var mp in meresiPontok.OrderBy(p => p.HelyisegNev).ThenBy(p => p.Sorszam))
            {
                if (!string.Equals(aktualisHelyiseg, mp.HelyisegNev, StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(mp.HelyisegNev))
                {
                    aktualisHelyiseg = mp.HelyisegNev;
                    table.Cell().ColumnSpan(8).Background(Colors.Grey.Lighten2).BorderTop(1).BorderBottom(1)
                        .BorderColor(Colors.Grey.Darken1).Padding(cellaPadding)
                        .Text($"Helyiség: {aktualisHelyiseg}").Bold().FontSize(betuMeret + 1);
                }

                var megfelelt = string.Equals(mp.Minosites, "MEGFELELT", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(mp.Minosites, "MEGFELEL", StringComparison.OrdinalIgnoreCase);
                var hatterSzin = megfelelt ? Colors.White : Colors.Red.Lighten4;

                SorCella(table, mp.MeresiPontHelye, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.Modszer, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.TularamvedelemHelye, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.TularamvedelemTipusa, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.AVKCsatolva ? "✓" : "✗", hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.PEFolytMegfelelt ? "✓" : "✗", hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.MertHurokimpedancia?.ToString("F2") ?? mp.ErtekOhm ?? "", hatterSzin, cellaPadding, betuMeret);
                table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(cellaPadding)
                    .Text(mp.Minosites).FontSize(betuMeret)
                    .FontColor(megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
            }
        });
    }

    private void AvkTablazat(IContainer container, List<AvkSor> avkSorok)
    {
        container.Column(col =>
        {
            col.Item().Text("ÁVK").Bold().FontSize(10);
            col.Item().PaddingTop(2).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(1.6f); // Helye
                    c.RelativeColumn(1.0f); // Jele
                    c.RelativeColumn(1.6f); // Típus
                    c.RelativeColumn(0.8f); // In
                    c.RelativeColumn(0.9f); // IΔn
                    c.RelativeColumn(0.8f); // Un
                    c.RelativeColumn(0.8f); // Pólussz.
                    c.RelativeColumn(1.2f); // IΔn mért
                    c.RelativeColumn(1.2f); // t x IΔn
                    c.ConstantColumn(24);   // MP
                    c.ConstantColumn(24);   // SZV
                    c.RelativeColumn(1.1f); // Minősítés
                });

                table.Header(header =>
                {
                    FejlecCella(header, "Helye");
                    FejlecCella(header, "Jele");
                    FejlecCella(header, "Típus");
                    FejlecCella(header, "In [A]");
                    FejlecCella(header, "IΔn [mA]");
                    FejlecCella(header, "Un [V]");
                    FejlecCella(header, "Pólussz.");
                    FejlecCella(header, "IΔn mért [mA]");
                    FejlecCella(header, "t [ms] ~IΔn");
                    FejlecCella(header, "MP");
                    FejlecCella(header, "SZV");
                    FejlecCella(header, "MINŐSÍTÉS");
                });

                foreach (var s in avkSorok)
                {
                    var megfelelt = s.MegfeleltE;
                    var hatterSzin = megfelelt ? Colors.White : Colors.Red.Lighten4;

                    SorCella(table, s.Helye, hatterSzin);
                    SorCella(table, s.Jele, hatterSzin);
                    SorCella(table, s.TipusNev, hatterSzin);
                    SorCella(table, s.In, hatterSzin);
                    SorCella(table, s.IDn, hatterSzin);
                    SorCella(table, s.Un, hatterSzin);
                    SorCella(table, s.Polusszam, hatterSzin);
                    SorCella(table, s.IDnMertWord, hatterSzin);
                    SorCella(table, s.T1xWord, hatterSzin);
                    SorCella(table, s.MukodesProba ? "✓" : "✗", hatterSzin);
                    SorCella(table, s.Szemrevetelez ? "✓" : "✗", hatterSzin);
                    table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3)
                        .Text(megfelelt ? "MEGFELELT" : "NEM FELELT MEG")
                        .FontColor(megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
                }
            });
        });
    }

    private void FejlecCella(TableCellDescriptor header, string szoveg, float padding = 3f, float betuMeret = 7f)
    {
        header.Cell().Background(Colors.Grey.Lighten2).Padding(padding).Text(szoveg).Bold().FontSize(betuMeret);
    }

    private void SorCella(TableDescriptor table, string szoveg, string hatterSzin, float padding = 3f, float betuMeret = 7f)
    {
        table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(padding).Text(szoveg ?? "").FontSize(betuMeret);
    }
}
