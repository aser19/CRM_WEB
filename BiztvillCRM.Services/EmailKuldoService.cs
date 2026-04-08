using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BiztvillCRM.Services;

public class EmailKuldoService : IEmailKuldoService
{
    private readonly CrmDbContext _context;
    private readonly IEmailSablonService _sablonService;
    private readonly ILogger<EmailKuldoService> _logger;

    public EmailKuldoService(
        CrmDbContext context,
        IEmailSablonService sablonService,
        ILogger<EmailKuldoService> logger)
    {
        _context = context;
        _sablonService = sablonService;
        _logger = logger;
    }

    public async Task<bool> KuldAsync(string cimzett, string targy, string szoveg, int? cegId = null)
    {
        var smtp = await _context.SmtpBeallitasok.FirstOrDefaultAsync();
        if (smtp is null || !smtp.Aktiv)
        {
            _logger.LogWarning("SMTP nincs konfigurálva vagy inaktív.");
            return false;
        }

        var naplo = new EmailKuldesNaplo
        {
            Kuldve = DateTime.UtcNow,
            CegId = cegId,
            Cimzett = cimzett,
            Targy = targy
        };

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.KuldoNev, smtp.KuldoEmail));
            message.To.Add(MailboxAddress.Parse(cimzett));
            message.Subject = targy;
            message.Body = new TextPart("html") { Text = szoveg };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.SzerverCim, smtp.Port, smtp.SslHasznalata);
            await client.AuthenticateAsync(smtp.FelhasznaloNev, smtp.Jelszo);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            naplo.Sikeres = true;
            _logger.LogInformation("Email sikeresen elküldve: {Cimzett}", cimzett);
        }
        catch (Exception ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = ex.Message;
            _logger.LogError(ex, "Email küldés sikertelen: {Cimzett}", cimzett);
        }

        _context.EmailKuldesNaplok.Add(naplo);
        await _context.SaveChangesAsync();

        return naplo.Sikeres;
    }

    public async Task<bool> KuldSablonbolAsync(
        EmailErtesitesTipus tipus,
        string cimzett,
        Dictionary<string, string> placeholderek,
        int? cegId = null,
        int? hitelesitesId = null,
        int? meresId = null)
    {
        var sablon = await _sablonService.GetByTipusAsync(tipus, cegId);
        if (sablon is null)
        {
            _logger.LogWarning("Email sablon nem található: {Tipus}", tipus);
            return false;
        }

        // Placeholder-ek helyettesítése
        var targy = HelyettesitPlaceholderek(sablon.Targy, placeholderek);
        var szoveg = HelyettesitPlaceholderek(sablon.Szoveg, placeholderek);

        // HTML formázás (egyszerű newline -> br)
        szoveg = szoveg.Replace("\n", "<br/>");

        var smtp = await _context.SmtpBeallitasok.FirstOrDefaultAsync();
        if (smtp is null || !smtp.Aktiv) return false;

        var naplo = new EmailKuldesNaplo
        {
            Kuldve = DateTime.UtcNow,
            CegId = cegId,
            Tipus = tipus,
            Cimzett = cimzett,
            Targy = targy,
            HitelesitesId = hitelesitesId,
            MeresId = meresId
        };

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.KuldoNev, smtp.KuldoEmail));
            message.To.Add(MailboxAddress.Parse(cimzett));
            message.Subject = targy;
            message.Body = new TextPart("html") { Text = szoveg };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.SzerverCim, smtp.Port, smtp.SslHasznalata);
            await client.AuthenticateAsync(smtp.FelhasznaloNev, smtp.Jelszo);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            naplo.Sikeres = true;
        }
        catch (Exception ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = ex.Message;
            _logger.LogError(ex, "Sablon email küldés sikertelen: {Cimzett}", cimzett);
        }

        _context.EmailKuldesNaplok.Add(naplo);
        await _context.SaveChangesAsync();

        return naplo.Sikeres;
    }

    private static string HelyettesitPlaceholderek(string szoveg, Dictionary<string, string> placeholderek)
    {
        foreach (var (kulcs, ertek) in placeholderek)
        {
            szoveg = szoveg.Replace($"{{{kulcs}}}", ertek ?? "");
        }
        return szoveg;
    }
}