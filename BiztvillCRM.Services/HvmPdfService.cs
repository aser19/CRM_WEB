using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// Hibavédelmi mérési jegyzőkönyv (HVM) PDF generálása.
/// Az "Egy mérés" jegyzőkönyv (EgyMeresPdfService) vizuális stílusát követi: fektetett A4, bekeretezett
/// blokkos elrendezés, aláírás/bélyegző, három dátumsoros lábléc, dinamikus sortördelés.
/// Kivétel: ÁVK oszlop/blokk itt sosem jelenik meg, mert az külön jegyzőkönyv típus.
/// </summary>
public class HvmPdfService : IHvmPdfService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly IFileStorageService _fileStorageService;
    private byte[]? _cegBelyegzoKep;
    private byte[]? _felulvizsgaloAlairasKep;

    public HvmPdfService(IMeresService meresService, ITenantService tenantService, ICegService cegService,
        IFelulvizsgaloService felulvizsgaloService, IFileStorageService fileStorageService)
    {
        _meresService = meresService;
        _tenantService = tenantService;
        _cegService = cegService;
        _felulvizsgaloService = felulvizsgaloService;
        _fileStorageService = fileStorageService;
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

        var cegNev = adatok.CegNev;
        var cegCim = adatok.CegCim;
        var cegId = _tenantService.GetCurrentCegId();
        if (string.IsNullOrWhiteSpace(cegNev) || string.IsNullOrWhiteSpace(cegCim))
        {
            try
            {
                var ceg = await _cegService.GetByIdAsync(cegId);
                cegNev = string.IsNullOrWhiteSpace(cegNev) ? ceg?.Nev ?? "" : cegNev;
                cegCim = string.IsNullOrWhiteSpace(cegCim) ? ceg?.Cim ?? "" : cegCim;
            }
            catch { /* nincs tenant kontextus */ }
        }

        var munkaszam = !string.IsNullOrWhiteSpace(adatok.Munkaszam) ? adatok.Munkaszam : $"HVM-{meresId:D6}/{DateTime.Now:yyyy}";
        var vizsgalatHelye = !string.IsNullOrWhiteSpace(adatok.MeresHelye) ? adatok.MeresHelye : meres?.Telephely?.Cim ?? "";
        var meresIdeje = meres?.Datum ?? adatok.MeresIdeje;
        var jegyzokonyvKeszultDatum = meres?.Letrehozva ?? adatok.KeszitesDatum;
        var meresiPontok = adatok.MeresiPontok ?? new List<MeresiPontSor>();

        try
        {
            var ceg = await _cegService.GetByIdAsync(cegId);
            if (!string.IsNullOrWhiteSpace(ceg?.BelyegzoPath))
            {
                _cegBelyegzoKep = await _fileStorageService.GetFileBytesAsync(ceg.BelyegzoPath);
            }
        }
        catch { /* nincs tenant kontextus vagy nincs bélyegző */ }

        var jogosultsagIgazolas = "";
        var felulvizsgaloNev = !string.IsNullOrWhiteSpace(adatok.FelulvizsgaloNev) ? adatok.FelulvizsgaloNev : adatok.FelelosNev;
        if (!string.IsNullOrWhiteSpace(felulvizsgaloNev))
        {
            try
            {
                var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();
                var felulvizsgalo = felulvizsgalok.FirstOrDefault(f => f.Nev == felulvizsgaloNev);
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

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Element(c => Tartalom(c, meresiPontok, adatok, munkaszam, cegNev, cegCim,
                    vizsgalatHelye, meresIdeje, jegyzokonyvKeszultDatum, jogosultsagIgazolas, felulvizsgaloNev));
            });
        }).GeneratePdf();
    }

    private void Tartalom(IContainer container, List<MeresiPontSor> meresiPontok, HvmAdatok adatok,
        string munkaszam, string cegNev, string cegCim, string vizsgalatHelye,
        DateTime meresIdeje, DateTime jegyzokonyvKeszultDatum, string jogosultsagIgazolas, string felulvizsgaloNev)
    {
        container.Border(1).BorderColor(Colors.Black).Column(col =>
        {
            // === CÍM BLOKK ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Column(c =>
            {
                c.Item().AlignCenter().Text("VILLAMOS BERENDEZÉS FELÜLVIZSGÁLATÁNAK JELENTÉSE").Bold().FontSize(11);
                c.Item().AlignCenter().Text("MSZ HD 60364-6:2017").Bold().FontSize(9);
            });

            // === FELÜLVIZSGÁLAT HELYE + KAPCSOLATTARTÓ ===
            col.Item().BorderBottom(1).BorderColor(Colors.Black).Row(row =>
            {
                row.RelativeItem(3).BorderRight(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("A felülvizsgálat helye: ").SemiBold();
                        text.Span($"{cegNev} – Székhely – {cegCim}".Trim(' ', '–'));
                    });
                    c.Item().PaddingTop(2).Text(text =>
                    {
                        text.Span("Jelentés típusa: ").SemiBold();
                        text.Span(adatok.JelentesTipus);
                    });
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
                        text.Span(felulvizsgaloNev ?? "");
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
                    var tipus = muszer?.Tipus ?? adatok.MuszerTipus ?? "";
                    var gyariSzam = muszer?.GyariSzam ?? adatok.MuszerGyariSzam ?? "";
                    var kalibralas = muszer?.Kalibralas ?? adatok.MuszerKalibralasStr ?? "";
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
                    .Text($"Munkaszám: {munkaszam}");
                row.RelativeItem(1).BorderRight(1).BorderColor(Colors.Black).Padding(4)
                    .Text($"Minősítés: {minosites}");
                row.RelativeItem(1.4f).Padding(4).Column(c =>
                {
                    c.Item().Text("Felelős felülvizsgáló:").SemiBold();
                    c.Item().Text(felulvizsgaloNev ?? "");
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
                FejlecCella(header, "Sor-szám", cellaPadding, betuMeret);
                FejlecCella(header, "MÉRÉSI PONT HELYE, MEGNEVEZÉSE, EGYÉB KÖZLENDŐ ADAT\n(vezeték adatai, áramkör tervjele stb.)", cellaPadding, betuMeret);
                FejlecCella(header, "MÓD OSZT.", cellaPadding, betuMeret);
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nHelye", cellaPadding, betuMeret);
                FejlecCella(header, "KIOLDÓSZERV-TÚLÁRAMVÉDELMI SZERV\nTípus (In, kar.)", cellaPadding, betuMeret);
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

                SorCella(table, $"{mp.Sorszam}.", hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.MeresiPontHelye, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.Modszer, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.TularamvedelemHelye, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.TularamvedelemTipusa, hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.PEFolytMegfelelt ? "✓" : "✗", hatterSzin, cellaPadding, betuMeret);
                SorCella(table, mp.MertHurokimpedancia?.ToString("F2") ?? mp.ErtekOhm ?? "", hatterSzin, cellaPadding, betuMeret);
                table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(cellaPadding)
                    .Text(mp.Minosites).FontSize(betuMeret)
                    .FontColor(megfelelt ? Colors.Green.Darken2 : Colors.Red.Darken2);
            }
        });
    }

    private void FejlecCella(TableCellDescriptor header, string szoveg, float padding = 3f, float betuMeret = 7f)
    {
        header.Cell().Background(Colors.Grey.Lighten2).Padding(padding).Text(szoveg).Bold().FontSize(betuMeret);
    }

    private void SorCella(TableDescriptor table, string? szoveg, string hatterSzin, float padding = 3f, float betuMeret = 7f)
    {
        table.Cell().Background(hatterSzin).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(padding).Text(szoveg ?? "").FontSize(betuMeret);
    }
}
