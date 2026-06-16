using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BiztvillCRM.Services;

public class LejaroOsszefoglaloService : ILejaroOsszefoglaloService
{
    private readonly CrmDbContext _context;
    private readonly IEmailKuldoService _emailKuldo;
    private readonly ILogger<LejaroOsszefoglaloService> _logger;

    public LejaroOsszefoglaloService(
        CrmDbContext context,
        IEmailKuldoService emailKuldo,
        ILogger<LejaroOsszefoglaloService> logger)
    {
        _context = context;
        _emailKuldo = emailKuldo;
        _logger = logger;
    }

    public async Task<LejaroOsszefoglalo> GetOsszefoglaloAsync(int ugyfelId, int napokSzama = 30)
    {
        var ma    = DateTime.Today;
        var hatar = ma.AddDays(napokSzama);

        var ugyfelNev = (await _context.Ugyfelek.FindAsync(ugyfelId))?.Nev ?? "";

        // Melléklet mérés ID-k – ezeket kizárjuk
        var mellekletIds = (await _context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync())
            .ToHashSet();

        var meresek = await _context.Meresek
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Where(m => m.UgyfelId == ugyfelId
                     && !mellekletIds.Contains(m.Id)
                     && m.KovetkezoDatum.HasValue
                     && m.KovetkezoDatum.Value >= ma
                     && m.KovetkezoDatum.Value <= hatar)
            .OrderBy(m => m.KovetkezoDatum)
            .ToListAsync();

        var hitelesitesek = await _context.Hitelesitesek
            .Include(h => h.Telephely)
            .Include(h => h.EszkozTipus)
            .Where(h => h.UgyfelId == ugyfelId
                     && h.LejaratDatum.HasValue
                     && h.LejaratDatum.Value >= ma
                     && h.LejaratDatum.Value <= hatar)
            .OrderBy(h => h.LejaratDatum)
            .ToListAsync();

        var karbantartasok = await _context.Karbantartasok
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .Where(k => k.UgyfelId == ugyfelId
                     && k.KovetkezoDatum.HasValue
                     && k.KovetkezoDatum.Value >= ma
                     && k.KovetkezoDatum.Value <= hatar)
            .OrderBy(k => k.KovetkezoDatum)
            .ToListAsync();

        var kockazatok = await _context.Kockazatertekelesek
            .Include(k => k.Telephely)
            .Where(k => k.UgyfelId == ugyfelId
                     && k.Aktiv
                     && k.KovetkezoFelulvizsgalat.HasValue
                     && k.KovetkezoFelulvizsgalat.Value >= ma
                     && k.KovetkezoFelulvizsgalat.Value <= hatar)
            .OrderBy(k => k.KovetkezoFelulvizsgalat)
            .ToListAsync();

        var zonaterkepek = await _context.Zonaterkepek
            .Include(z => z.Telephely)
            .Where(z => z.UgyfelId == ugyfelId
                     && z.Aktiv
                     && z.ErvenyessegVege.HasValue
                     && z.ErvenyessegVege.Value >= ma
                     && z.ErvenyessegVege.Value <= hatar)
            .OrderBy(z => z.ErvenyessegVege)
            .ToListAsync();

        return new LejaroOsszefoglalo
        {
            UgyfelId      = ugyfelId,
            UgyfelNev     = ugyfelNev,
            NapokSzama    = napokSzama,
            Meresek       = meresek,
            Hitelesitesek = hitelesitesek,
            Karbantartasok = karbantartasok,
            Kockazatok    = kockazatok,
            Zonaterkepek  = zonaterkepek
        };
    }

    public async Task<bool> KuldOsszefoglaloEmailtAsync(
        int ugyfelId, string cimzett, int napokSzama = 30, int? cegId = null)
    {
        var o = await GetOsszefoglaloAsync(ugyfelId, napokSzama);

        if (!o.VanTetel)
        {
            _logger.LogInformation(
                "Nincs lejáró tétel a következő {Nap} napban (UgyfelId={Id})", napokSzama, ugyfelId);
            return false;
        }

        var targy  = $"Lejáró tételek – {o.UgyfelNev} – következő {napokSzama} nap";
        var szoveg = EpitHtmlSzoveg(o);

        return await _emailKuldo.KuldAsync(cimzett, targy, szoveg, cegId);
    }

    // ─── HTML builder ──────────────────────────────────────────────────────────

    private static string EpitHtmlSzoveg(LejaroOsszefoglalo o)
    {
        var sb = new StringBuilder();
        sb.Append($"""
            <html><body style="font-family:Arial,sans-serif;color:#333;max-width:800px;margin:0 auto;">
            <h2 style="color:#1565C0;border-bottom:3px solid #1565C0;padding-bottom:8px;">
                📋 Lejáró tételek összefoglalója
            </h2>
            <p>Tisztelt Ügyfelünk!</p>
            <p>Az alábbi tételek lejárnak a következő <strong>{o.NapokSzama} napban</strong>:</p>
            """);

        if (o.Meresek.Any())
            sb.Append(EpitSzakasz("⚡ Mérések",
                ["Telephely", "Típus", "Következő dátum"],
                o.Meresek.Select(m => new[]
                {
                    m.Telephely?.Nev ?? "-",
                    m.MeresTipus?.Nev ?? "-",
                    m.KovetkezoDatum?.ToString("yyyy.MM.dd") ?? "-"
                })));

        if (o.Hitelesitesek.Any())
            sb.Append(EpitSzakasz("🔬 Hitelesítések",
                ["Telephely", "Eszköz típus", "Lejárat"],
                o.Hitelesitesek.Select(h => new[]
                {
                    h.Telephely?.Nev ?? "-",
                    h.EszkozTipus?.Nev ?? "-",
                    h.LejaratDatum?.ToString("yyyy.MM.dd") ?? "-"
                })));

        if (o.Karbantartasok.Any())
            sb.Append(EpitSzakasz("🔧 Karbantartások",
                ["Telephely", "Típus", "Következő dátum"],
                o.Karbantartasok.Select(k => new[]
                {
                    k.Telephely?.Nev ?? "-",
                    k.KarbantartasTipus?.Nev ?? "-",
                    k.KovetkezoDatum?.ToString("yyyy.MM.dd") ?? "-"
                })));

        if (o.Kockazatok.Any())
            sb.Append(EpitSzakasz("📋 Kockázatértékelések",
                ["Megnevezés", "Telephely", "Következő felülvizsgálat"],
                o.Kockazatok.Select(k => new[]
                {
                    k.Megnevezes,
                    k.Telephely?.Nev ?? "-",
                    k.KovetkezoFelulvizsgalat?.ToString("yyyy.MM.dd") ?? "-"
                })));

        if (o.Zonaterkepek.Any())
            sb.Append(EpitSzakasz("🗺️ Zónatérképek",
                ["Megnevezés", "Telephely", "Érvényesség vége"],
                o.Zonaterkepek.Select(z => new[]
                {
                    z.Megnevezes,
                    z.Telephely?.Nev ?? "-",
                    z.ErvenyessegVege?.ToString("yyyy.MM.dd") ?? "-"
                })));

        sb.Append("""
            <hr style="border:none;border-top:1px solid #ccc;margin:24px 0;"/>
            <p style="color:#888;font-size:12px;">
                Ez az értesítés automatikusan lett küldve a BiztvillCRM rendszerből.
            </p>
            </body></html>
            """);

        return sb.ToString();
    }

    private static string EpitSzakasz(
        string cim, string[] oszlopok, IEnumerable<string[]> sorok)
    {
        var sb = new StringBuilder();
        sb.Append($"""
            <h3 style="color:#1976D2;border-bottom:2px solid #BBDEFB;padding-bottom:4px;margin-top:24px;">
                {cim}
            </h3>
            <table style="width:100%;border-collapse:collapse;margin-bottom:8px;">
            <tr style="background:#E3F2FD;">
            """);

        foreach (var oszlop in oszlopok)
            sb.Append($"""<th style="padding:8px;text-align:left;border:1px solid #BBDEFB;">{oszlop}</th>""");
        sb.Append("</tr>");

        var sorokLista = sorok.ToList();
        for (int i = 0; i < sorokLista.Count; i++)
        {
            var sor = sorokLista[i];
            var hatter = i % 2 == 0 ? "#fff" : "#F5F5F5";
            sb.Append($"""<tr style="background:{hatter};">""");
            for (int j = 0; j < sor.Length; j++)
            {
                // Utolsó oszlop (dátum) piros és félkövér
                var stilus = j == sor.Length - 1
                    ? "padding:8px;border:1px solid #BBDEFB;color:#C62828;font-weight:bold;"
                    : "padding:8px;border:1px solid #BBDEFB;";
                sb.Append($"""<td style="{stilus}">{sor[j]}</td>""");
            }
            sb.Append("</tr>");
        }

        sb.Append("</table>");
        return sb.ToString();
    }
}