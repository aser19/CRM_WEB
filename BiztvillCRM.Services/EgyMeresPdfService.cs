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
        }).GeneratePdf();
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
                FejlecCella(header, "MÉRÉSI PONT HELYE, MEGNEVEZÉSE, EGYÉB KÖZLENDŐ ADAT\n(vezeték adatai, áramkör tervjele stb.)");
                FejlecCella(header, "MÓD/OSZT");
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nHelye");
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nTípus (In, kar.)");
                FejlecCella(header, "ÁVK");
                FejlecCella(header, "PE folyt.");
                FejlecCella(header, "ÉRTÉK [Ω]");
                FejlecCella(header, "MINŐSÍTÉS");
            });

            string? aktualisHelyiseg = null;
            foreach (var mp in meresiPontok.OrderBy(p => p.HelyisegNev).ThenBy(p => p.Sorszam))
            {
                if (!string.Equals(aktualisHelyiseg, mp.HelyisegNev, StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(mp.HelyisegNev))
                {
                    aktualisHelyiseg = mp.HelyisegNev;
                    table.Cell().ColumnSpan(8).Background(Colors.Grey.Lighten2).BorderTop(1).BorderBottom(1)
                        .BorderColor(Colors.Grey.Darken1).Padding(3)
                        .Text($"Helyiség: {aktualisHelyiseg}").Bold().FontSize(8);
                }

                var megfelelt = string.Equals(mp.Minosites, "MEGFELELT", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(mp.Minosites, "MEGFELEL", StringComparison.OrdinalIgnoreCase);
                var hatterSzin = megfelelt ? Colors.White : Colors.Red.Lighten4;

                SorCella(table, mp.MeresiPontHelye, hatterSzin);
                SorCella(table, mp.Modszer, hatterSzin);
                SorCella(table, mp.TularamvedelemHelye, hatterSzin);
                SorCella(table, mp.TularamvedelemTipusa, hatterSzin);
                SorCella(table, mp.AVKCsatolva ? "✓" : "✗", hatterSzin);
                SorCella(table, mp.PEFolytMegfelelt ? "✓" : "✗", hatterSzin);
                SorCella(table, mp.MertHurokimpedancia?.ToString("F2") ?? mp.ErtekOhm ?? "", hatterSzin);
                table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3)
                    .Text(mp.Minosites)
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

    private void FejlecCella(TableCellDescriptor header, string szoveg)
    {
        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(szoveg).Bold().FontSize(7);
    }

    private void SorCella(TableDescriptor table, string szoveg, string hatterSzin)
    {
        table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(3).Text(szoveg ?? "").FontSize(7);
    }
}
