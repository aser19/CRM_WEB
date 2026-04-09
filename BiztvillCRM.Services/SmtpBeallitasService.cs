using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Sockets;

namespace BiztvillCRM.Services;

public class SmtpBeallitasService : ISmtpBeallitasService
{
    private readonly CrmDbContext _context;
    private readonly ILogger<SmtpBeallitasService> _logger;

    public SmtpBeallitasService(CrmDbContext context, ILogger<SmtpBeallitasService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SmtpBeallitas?> GetAsync() =>
        await _context.SmtpBeallitasok.FirstOrDefaultAsync();

    public async Task<SmtpBeallitas> SaveAsync(SmtpBeallitas beallitas)
    {
        var existing = await _context.SmtpBeallitasok.FirstOrDefaultAsync();
        
        if (existing is null)
        {
            beallitas.Letrehozva = DateTime.UtcNow;
            _context.SmtpBeallitasok.Add(beallitas);
        }
        else
        {
            existing.SzerverCim = beallitas.SzerverCim;
            existing.Port = beallitas.Port;
            existing.TitkositasTipus = beallitas.TitkositasTipus;
            existing.FelhasznaloNev = beallitas.FelhasznaloNev;
            existing.Jelszo = beallitas.Jelszo;
            existing.KuldoNev = beallitas.KuldoNev;
            existing.KuldoEmail = beallitas.KuldoEmail;
            existing.Aktiv = beallitas.Aktiv;
            existing.Modositva = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existing ?? beallitas;
    }

    public async Task<bool> TesztKuldesAsync(string cimzett)
    {
        var smtp = await GetAsync();
        var naplo = new EmailKuldesNaplo
        {
            Kuldve = DateTime.UtcNow,
            Tipus = EmailErtesitesTipus.SmtpTeszt,
            Cimzett = cimzett,
            Targy = "BiztvillCRM - SMTP Teszt"
        };

        if (smtp is null)
        {
            naplo.Sikeres = false;
            naplo.Hiba = "SMTP beállítások nem találhatók.";
            await MentesNaploAsync(naplo);
            return false;
        }

        if (!smtp.Aktiv)
        {
            naplo.Sikeres = false;
            naplo.Hiba = "SMTP nincs engedélyezve.";
            await MentesNaploAsync(naplo);
            return false;
        }

        // 🔑 SecureSocketOptions meghatározása az enum alapján
        var secureSocketOptions = smtp.TitkositasTipus switch
        {
            SmtpTitkositasTipus.Nincs => SecureSocketOptions.None,
            SmtpTitkositasTipus.Auto => SecureSocketOptions.Auto,
            SmtpTitkositasTipus.StartTls => SecureSocketOptions.StartTls,
            SmtpTitkositasTipus.SslOnConnect => SecureSocketOptions.SslOnConnect,
            _ => SecureSocketOptions.Auto
        };

        _logger.LogInformation(
            "SMTP teszt: {Server}:{Port}, Titkosítás: {SecureSocket}, Címzett: {Cimzett}",
            smtp.SzerverCim, smtp.Port, secureSocketOptions, cimzett);

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.KuldoNev, smtp.KuldoEmail));
            message.To.Add(MailboxAddress.Parse(cimzett));
            message.Subject = "BiztvillCRM - SMTP Teszt";
            message.Body = new TextPart("plain")
            {
                Text = $"""
                    Ez egy teszt email a BiztvillCRM rendszerből.
                    
                    Küldés időpontja: {DateTime.Now:yyyy.MM.dd HH:mm:ss}
                    SMTP szerver: {smtp.SzerverCim}:{smtp.Port}
                    Titkosítás: {smtp.TitkositasTipus}
                    """
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.SzerverCim, smtp.Port, secureSocketOptions);
            
            _logger.LogInformation("SMTP kapcsolódás sikeres, hitelesítés...");
            
            if (!string.IsNullOrEmpty(smtp.FelhasznaloNev))
            {
                await client.AuthenticateAsync(smtp.FelhasznaloNev, smtp.Jelszo);
            }
            
            _logger.LogInformation("SMTP hitelesítés sikeres, küldés...");
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            naplo.Sikeres = true;
            await MentesNaploAsync(naplo);
            _logger.LogInformation("SMTP teszt sikeres!");
            return true;
        }
        catch (AuthenticationException ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = $"Hitelesítési hiba: {ex.Message}";
            await MentesNaploAsync(naplo);
            _logger.LogError(ex, "SMTP hitelesítési hiba");
            return false;
        }
        catch (SslHandshakeException ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = $"SSL hiba: Próbálj másik titkosítási módot. Részletek: {ex.Message}";
            await MentesNaploAsync(naplo);
            _logger.LogError(ex, "SMTP SSL hiba");
            return false;
        }
        catch (SocketException ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = $"Kapcsolódási hiba: {smtp.SzerverCim}:{smtp.Port} nem elérhető. ({ex.Message})";
            await MentesNaploAsync(naplo);
            _logger.LogError(ex, "SMTP socket hiba");
            return false;
        }
        catch (Exception ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = $"{ex.GetType().Name}: {ex.Message}";
            await MentesNaploAsync(naplo);
            _logger.LogError(ex, "SMTP ismeretlen hiba");
            return false;
        }
    }

    private async Task MentesNaploAsync(EmailKuldesNaplo naplo)
    {
        _context.EmailKuldesNaplok.Add(naplo);
        await _context.SaveChangesAsync();
    }
}