using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BiztvillCRM.Services;

/// <summary>
/// A "Túlnyomással működtetett lyukadásjelző időszakos vizsgálatáról" szóló
/// vizsgálati jegyzőkönyv PDF generálása, a mellékelt papír alapú minta szerint.
/// </summary>
public class LyukadasjelzoPdfService : ILyukadasjelzoPdfService
{
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFelulvizsgaloService _felulvizsgaloService;
    private readonly ILyukadasjelzoSzamService _szamService;

    public LyukadasjelzoPdfService(ITenantService tenantService, ICegService cegService, IFileStorageService fileStorageService, IFelulvizsgaloService felulvizsgaloService, ILyukadasjelzoSzamService szamService)
    {
        _tenantService = tenantService;
        _cegService = cegService;
        _fileStorageService = fileStorageService;
        _felulvizsgaloService = felulvizsgaloService;
        _szamService = szamService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> TulnyomasosGeneralasAsync(TulnyomasosLyukadasjelzoAdatok adatok)
    {
        adatok ??= new TulnyomasosLyukadasjelzoAdatok();

        byte[]? belyegzoBytes = null;
        byte[]? logoBytes = null;
        string cegNev = adatok.VizsgalatotVegzo;
        try
        {
            var ceg = await _cegService.GetByIdAsync(_tenantService.GetCurrentCegId());
            if (!string.IsNullOrWhiteSpace(ceg?.BelyegzoPath))
            {
                belyegzoBytes = await _fileStorageService.GetFileBytesAsync(ceg.BelyegzoPath);
            }
            if (!string.IsNullOrWhiteSpace(ceg?.LogoPath))
            {
                logoBytes = await _fileStorageService.GetFileBytesAsync(ceg.LogoPath);
            }
            if (!string.IsNullOrWhiteSpace(ceg?.Nev))
            {
                cegNev = ceg.Nev;
            }
        }
        catch { /* nincs tenant kontextus vagy nincs bélyegző/logó */ }

        byte[]? alairasBytes = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(adatok.JelenlevokNevCeg))
            {
                var felulvizsgalok = await _felulvizsgaloService.GetAllAsync();
                var felulvizsgalo = felulvizsgalok.FirstOrDefault(f =>
                    !string.IsNullOrWhiteSpace(f.Nev) && adatok.JelenlevokNevCeg.Contains(f.Nev, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(felulvizsgalo?.AlairasPath))
                {
                    alairasBytes = await _fileStorageService.GetFileBytesAsync(felulvizsgalo.AlairasPath);
                }
            }
        }
        catch { /* nincs aláírás */ }

        adatok.VizsgalatotVegzo = cegNev;
        if (string.IsNullOrWhiteSpace(adatok.JegyzokonyvSzam))
        {
            // Első kiállítás: cégfüggő, auto-increment sorszám generálása.
            try
            {
                adatok.JegyzokonyvSzam = await _szamService.GeneralKovetkezoSorszamAsync(_tenantService.GetCurrentCegId());
            }
            catch
            {
                adatok.JegyzokonyvSzam = $"LYUK-{DateTime.Now:yyyyMMddHHmmss}";
            }
        }
        else
        {
            // Már létezett jegyzőkönyvszám: ez egy a kiállítás utáni módosítás/újragenerálás.
            adatok.ModositasVerzio++;
        }

        adatok.LetoltesDatum = DateTime.Today;

        return Generalas(adatok, belyegzoBytes, logoBytes, alairasBytes);
    }

    private byte[] Generalas(TulnyomasosLyukadasjelzoAdatok adatok, byte[]? belyegzoKep, byte[]? logoKep, byte[]? alairasKep)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Element(c => Tartalom(c, adatok, belyegzoKep, logoKep, alairasKep));
                page.Footer().Element(c => Lablec(c, adatok));
            });
        }).GeneratePdf();
    }

    private void Tartalom(IContainer container, TulnyomasosLyukadasjelzoAdatok adatok, byte[]? belyegzoKep, byte[]? logoKep, byte[]? alairasKep)
    {
        container.Column(col =>
        {
            col.Spacing(6);

            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    if (logoKep is not null)
                    {
                        c.Item().Height(25).AlignLeft().Image(logoKep).FitArea();
                    }
                });
                row.ConstantItem(150).AlignRight().Text(t =>
                {
                    t.Span("Jegyzőkönyv szám: ").Bold();
                    t.Span(TeljesJegyzokonyvSzam(adatok));
                });
            });

            col.Item().AlignCenter().Text("VIZSGÁLATI JEGYZŐKÖNYV").Bold().FontSize(14);
            col.Item().AlignCenter().Text("Túlnyomással működtetett lyukadásjelző").Bold();
            col.Item().AlignCenter().Text("időszakos vizsgálatáról").Bold();

            if (!string.IsNullOrWhiteSpace(adatok.MegbizoNev))
            {
                col.Item().PaddingTop(10).Text(t =>
                {
                    t.Span("Megbízó: ").Bold();
                    t.Span(adatok.MegbizoNev);
                });
                col.Item().Text(t =>
                {
                    t.Span("Vizsgálat helye: ").Bold();
                    t.Span(adatok.VizsgalatHelye);
                });
            }
            else
            {
                col.Item().PaddingTop(10).Text(t =>
                {
                    t.Span("Vizsgálat helye: ").Bold();
                    t.Span(adatok.VizsgalatHelye);
                });
            }
            col.Item().Text(t =>
            {
                t.Span("Vizsgálat ideje: ").Bold();
                t.Span(adatok.VizsgalatIdeje.ToString("yyyy.MM.dd."));
            });
            if (adatok.KovetkezoVizsgalatIdopontja is not null)
            {
                col.Item().Text(t =>
                {
                    t.Span("Következő vizsgálat időpontja: ").Bold();
                    t.Span(adatok.KovetkezoVizsgalatIdopontja.Value.ToString("yyyy.MM.dd."));
                });
            }
            if (!string.IsNullOrWhiteSpace(adatok.EszkozAzonosito))
            {
                col.Item().Text(t =>
                {
                    t.Span("Eszköz azonosítója: ").Bold();
                    t.Span(adatok.EszkozAzonosito);
                });
            }
            col.Item().Text(t =>
            {
                t.Span("Jelen vannak (név, cég): ").Bold();
                t.Span(adatok.JelenlevokNevCeg);
            });
            col.Item().Text(t =>
            {
                t.Span("Üzemeltető részéről: ").Bold();
                t.Span(adatok.UzemeltetoReszerol);
            });
            col.Item().Text(t =>
            {
                t.Span("Vizsgálatot végző: ").Bold();
                t.Span(adatok.VizsgalatotVegzo);
            });
            col.Item().Text(t =>
            {
                t.Span("Lyukadásjelző típusa: ").Bold();
                t.Span(adatok.LyukadasjelzoTipusa);
            });
            col.Item().Text(t =>
            {
                t.Span("Lyukadásjelző gyári száma: ").Bold();
                t.Span(adatok.LyukadasjelzoGyariszama);
            });

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(5);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    CellaFejlec(header, "Vizsgált berendezés");
                    CellaFejlec(header, "Megfelel");
                    CellaFejlec(header, "Nem felel meg");
                });

                void Sor(string nev, bool? megfelel)
                {
                    table.Cell().Border(0.5f).Padding(4).Text(nev);
                    table.Cell().Border(0.5f).Padding(4).AlignCenter().Text(megfelel == true ? "X" : "");
                    table.Cell().Border(0.5f).Padding(4).AlignCenter().Text(megfelel == false ? "X" : "");
                }

                Sor("Lyukadásjelző berendezés felerősítése.", adatok.BerendezesFelerositeseMegfelel);
                Sor("Egyéb kiegészítő berendezés működő-képessége, tisztasága.", adatok.KiegeszitoBerendezesMegfelel);
                Sor("Lyukadásjelző működőképessége.", adatok.LyukadasjelzoMukodokepesMegfelel);
            });

            col.Item().PaddingTop(10).Text(
                $"A túlnyomással működtetett lyukadásjelzőt és a csővezeték kapcsolatait az ellenőrző térrel " +
                $"{adatok.EllenorzoTerNyomasaBar.ToString("0.###")} bar túlnyomással {adatok.TomorsegiProbaIdotartamPerc} perc időtartamig tömörségi próba alá vettük. " +
                $"Ez idő alatt a nyomás {(adatok.NyomasNemCsokkentAlakvaltozasNemTortent ? "nem csökkent, alakváltozás nem történt." : "csökkent, illetve alakváltozás történt.")}");

            col.Item().Text(t =>
            {
                t.Span("A tömörségi nyomáspróba: ").Bold();
                t.Span(adatok.TomorsegiProbaSikeres ? "sikerrel járt" : "nem járt sikerrel").Bold();
            });

            col.Item().Text($"A vizsgálatokat a {adatok.Rendelet} végeztük el.");

            if (!string.IsNullOrWhiteSpace(adatok.Megjegyzes))
            {
                col.Item().PaddingTop(6).Text(t =>
                {
                    t.Span("Megjegyzés: ").Bold();
                    t.Span(adatok.Megjegyzes);
                });
            }

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().PaddingTop(29).LineHorizontal(0.5f);
                    c.Item().AlignCenter().Text("Üzemeltető");
                });
                row.ConstantItem(40);
                row.RelativeItem().Column(c =>
                {
                    if (alairasKep is not null)
                    {
                        c.Item().AlignCenter().Height(25).Image(alairasKep).FitArea();
                    }
                    c.Item().LineHorizontal(0.5f);
                    c.Item().AlignCenter().Text(adatok.VizsgalatotVegzo);
                    if (belyegzoKep is not null)
                    {
                        c.Item().PaddingTop(4).AlignCenter().Height(30).Image(belyegzoKep).FitArea();
                    }
                });
            });
        });
    }

    private static void CellaFejlec(TableCellDescriptor header, string szoveg)
    {
        header.Cell().Border(0.5f).Background(Colors.Grey.Lighten3).Padding(4).Text(szoveg).Bold();
    }

    private static string TeljesJegyzokonyvSzam(TulnyomasosLyukadasjelzoAdatok adatok)
    {
        var alapSzam = adatok.JegyzokonyvSzam ?? "";
        return adatok.ModositasVerzio > 0 ? $"{alapSzam} modv{adatok.ModositasVerzio}" : alapSzam;
    }

    private void Lablec(IContainer container, TulnyomasosLyukadasjelzoAdatok adatok)
    {
        var letoltve = adatok.LetoltesDatum ?? DateTime.Today;
        container.Row(row =>
        {
            row.RelativeItem().Text($"Jegyzőkönyv letöltve: {letoltve:yyyy.MM.dd.}").FontSize(8).FontColor(Colors.Grey.Darken1);
            row.RelativeItem().AlignRight().Text(t =>
            {
                t.CurrentPageNumber();
                t.Span(" / ");
                t.TotalPages();
            });
        });
    }
}
