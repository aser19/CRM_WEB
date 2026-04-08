using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BiztvillCRM.Services;

public class LejaratErtesitoService : ILejaratErtesitoService
{
    private readonly CrmDbContext _context;
    private readonly IEmailKuldoService _emailKuldo;
    private readonly ILogger<LejaratErtesitoService> _logger;

    public LejaratErtesitoService(
        CrmDbContext context,
        IEmailKuldoService emailKuldo,
        ILogger<LejaratErtesitoService> logger)
    {
        _context = context;
        _emailKuldo = emailKuldo;
        _logger = logger;
    }

    public async Task<ErtesitesFeldolgozasEredmeny> FeldolgozasAsync()
    {
        var eredmeny = new ErtesitesFeldolgozasEredmeny();
        var ma = DateTime.Today;

        _logger.LogInformation("Lejárat értesítés feldolgozás indítása: {Datum}", ma);

        // Aktív email beállítások lekérése
        var beallitasok = await _context.EmailBeallitasok
            .Include(e => e.Ceg)
            .Where(e => e.Aktiv && e.ErtesitesTipusok != EmailErtesitesTipus.Nincs)
            .ToListAsync();

        foreach (var beallitas in beallitasok)
        {
            try
            {
                await FeldolgozCegAsync(beallitas, ma, eredmeny);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba a cég feldolgozása közben: {CegId}", beallitas.CegId);
                eredmeny.Hibak.Add($"Cég {beallitas.CegId}: {ex.Message}");
            }
        }

        _logger.LogInformation(
            "Lejárat értesítés feldolgozás befejezve. Hitelesítések: {H}, Mérések: {M}, Küldött: {K}, Sikertelen: {S}",
            eredmeny.FeldolgozottHitelesitesek,
            eredmeny.FeldolgozottMeresek,
            eredmeny.KuldottEmailek,
            eredmeny.SikertelenEmailek);

        return eredmeny;
    }

    private async Task FeldolgozCegAsync(EmailBeallitas beallitas, DateTime ma, ErtesitesFeldolgozasEredmeny eredmeny)
    {
        var cegId = beallitas.CegId;
        var cegNev = beallitas.Ceg?.Nev ?? "N/A";
        var cegEmail = beallitas.Ceg?.Email;

        // Egyedi címek JSON-ból
        var egyediCimek = new List<string>();
        if (!string.IsNullOrEmpty(beallitas.EgyediEmailCimek))
        {
            try
            {
                egyediCimek = JsonSerializer.Deserialize<List<string>>(beallitas.EgyediEmailCimek) ?? new();
            }
            catch { }
        }

        // === HITELESÍTÉSEK ===
        if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.HitelesitesLejarat90Nap) ||
            HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.HitelesitesLejarat30Nap))
        {
            var hitelesitesek = await _context.Hitelesitesek
                .Include(h => h.Ugyfel)
                .Include(h => h.Telephely)
                .Include(h => h.EszkozTipus)
                .Where(h => h.Ugyfel != null && h.Ugyfel.CegId == cegId)
                .Where(h => h.HitelesitesStatusz == HitelesitesStatusz.Sikeres)
                .Where(h => h.LejaratDatum >= ma) // Még nem járt le
                .ToListAsync();

            foreach (var h in hitelesitesek)
            {
                if (!h.LejaratDatum.HasValue) continue;  // <-- ÚJ SOR
    
                var napokLejaratig = (h.LejaratDatum.Value.Date - ma).Days;  // <-- .Value hozzáadva
                EmailErtesitesTipus? tipus = null;

                // 90 napos értesítés (89-91 nap között)
                if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.HitelesitesLejarat90Nap) &&
                    napokLejaratig >= 89 && napokLejaratig <= 91)
                {
                    // Ellenőrizzük, nem küldtünk-e már
                    if (!await MarKuldtunkAsync(EmailErtesitesTipus.HitelesitesLejarat90Nap, hitelesitesId: h.Id))
                    {
                        tipus = EmailErtesitesTipus.HitelesitesLejarat90Nap;
                    }
                }
                // 30 napos értesítés (29-31 nap között)
                else if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.HitelesitesLejarat30Nap) &&
                         napokLejaratig >= 29 && napokLejaratig <= 31)
                {
                    if (!await MarKuldtunkAsync(EmailErtesitesTipus.HitelesitesLejarat30Nap, hitelesitesId: h.Id))
                    {
                        tipus = EmailErtesitesTipus.HitelesitesLejarat30Nap;
                    }
                }

                if (tipus.HasValue)
                {
                    eredmeny.FeldolgozottHitelesitesek++;
                    var placeholderek = KeszitPlaceholderek(h.Ugyfel, h.Telephely, h.EszkozTipus?.Nev, null, h.LejaratDatum.Value, cegNev);
                    var cimzettek = GyujtCimzetteket(beallitas, h.Ugyfel, h.Telephely, cegEmail, egyediCimek);

                    foreach (var cimzett in cimzettek)
                    {
                        var siker = await _emailKuldo.KuldSablonbolAsync(
                            tipus.Value, cimzett, placeholderek, cegId, hitelesitesId: h.Id);
                        
                        if (siker) eredmeny.KuldottEmailek++;
                        else eredmeny.SikertelenEmailek++;
                    }
                }
            }
        }

        // === MÉRÉSEK ===
        if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.MeresLejarat90Nap) ||
            HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.MeresLejarat30Nap))
        {
            var meresek = await _context.Meresek
                .Include(m => m.Ugyfel)
                .Include(m => m.Telephely)
                .Include(m => m.MeresTipus)
                .Where(m => m.Ugyfel != null && m.Ugyfel.CegId == cegId)
                .Where(m => m.KovetkezoDatum.HasValue && m.KovetkezoDatum >= ma)
                .ToListAsync();

            foreach (var m in meresek)
            {
                if (!m.KovetkezoDatum.HasValue) continue;
                
                var napokLejaratig = (m.KovetkezoDatum.Value.Date - ma).Days;
                EmailErtesitesTipus? tipus = null;

                // 90 napos értesítés
                if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.MeresLejarat90Nap) &&
                    napokLejaratig >= 89 && napokLejaratig <= 91)
                {
                    if (!await MarKuldtunkAsync(EmailErtesitesTipus.MeresLejarat90Nap, meresId: m.Id))
                    {
                        tipus = EmailErtesitesTipus.MeresLejarat90Nap;
                    }
                }
                // 30 napos értesítés
                else if (HasFlag(beallitas.ErtesitesTipusok, EmailErtesitesTipus.MeresLejarat30Nap) &&
                         napokLejaratig >= 29 && napokLejaratig <= 31)
                {
                    if (!await MarKuldtunkAsync(EmailErtesitesTipus.MeresLejarat30Nap, meresId: m.Id))
                    {
                        tipus = EmailErtesitesTipus.MeresLejarat30Nap;
                    }
                }

                if (tipus.HasValue)
                {
                    eredmeny.FeldolgozottMeresek++;
                    var placeholderek = KeszitPlaceholderek(m.Ugyfel, m.Telephely, null, m.MeresTipus?.Nev, m.KovetkezoDatum.Value, cegNev);
                    var cimzettek = GyujtCimzetteket(beallitas, m.Ugyfel, m.Telephely, cegEmail, egyediCimek);

                    foreach (var cimzett in cimzettek)
                    {
                        var siker = await _emailKuldo.KuldSablonbolAsync(
                            tipus.Value, cimzett, placeholderek, cegId, meresId: m.Id);
                        
                        if (siker) eredmeny.KuldottEmailek++;
                        else eredmeny.SikertelenEmailek++;
                    }
                }
            }
        }
    }

    /// <summary>Ellenőrzi, hogy küldtünk-e már értesítést ehhez a tételhez.</summary>
    private async Task<bool> MarKuldtunkAsync(EmailErtesitesTipus tipus, int? hitelesitesId = null, int? meresId = null)
    {
        return await _context.EmailKuldesNaplok.AnyAsync(n =>
            n.Tipus == tipus &&
            n.Sikeres &&
            (hitelesitesId == null || n.HitelesitesId == hitelesitesId) &&
            (meresId == null || n.MeresId == meresId));
    }

    /// <summary>Placeholder dictionary készítése.</summary>
    private static Dictionary<string, string> KeszitPlaceholderek(
        Ugyfel? ugyfel,
        Telephely? telephely,
        string? eszkozTipus,
        string? meresTipus,
        DateTime lejaratDatum,
        string cegNev)
    {
        return new Dictionary<string, string>
        {
            ["UgyfelNev"] = ugyfel?.Nev ?? "N/A",
            ["TelephelyNev"] = telephely?.Nev ?? "N/A",
            ["EszkozTipus"] = eszkozTipus ?? "N/A",
            ["MeresTipus"] = meresTipus ?? "N/A",
            ["LejaratDatum"] = lejaratDatum.ToString("yyyy.MM.dd"),
            ["CegNev"] = cegNev
        };
    }

    /// <summary>Címzettek összegyűjtése a beállítások alapján.</summary>
    private static List<string> GyujtCimzetteket(
        EmailBeallitas beallitas,
        Ugyfel? ugyfel,
        Telephely? telephely,
        string? cegEmail,
        List<string> egyediCimek)
    {
        var cimzettek = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (HasFlag(beallitas.CimzettTipusok, EmailCimzettTipus.Sajat) && !string.IsNullOrWhiteSpace(cegEmail))
            cimzettek.Add(cegEmail);

        if (HasFlag(beallitas.CimzettTipusok, EmailCimzettTipus.UgyfelEmail) && !string.IsNullOrWhiteSpace(ugyfel?.Email))
            cimzettek.Add(ugyfel.Email);

        if (HasFlag(beallitas.CimzettTipusok, EmailCimzettTipus.TelephelyEmail) && !string.IsNullOrWhiteSpace(telephely?.Email))
            cimzettek.Add(telephely.Email);

        if (HasFlag(beallitas.CimzettTipusok, EmailCimzettTipus.EgyediCimek))
        {
            foreach (var cim in egyediCimek.Where(c => !string.IsNullOrWhiteSpace(c)))
                cimzettek.Add(cim);
        }

        return cimzettek.ToList();
    }

    private static bool HasFlag(EmailErtesitesTipus flags, EmailErtesitesTipus flag) => (flags & flag) == flag;
    private static bool HasFlag(EmailCimzettTipus flags, EmailCimzettTipus flag) => (flags & flag) == flag;
}