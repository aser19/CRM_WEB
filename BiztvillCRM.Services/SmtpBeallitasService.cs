using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;

namespace BiztvillCRM.Services;

public class SmtpBeallitasService : ISmtpBeallitasService
{
    private readonly CrmDbContext _context;

    public SmtpBeallitasService(CrmDbContext context) => _context = context;

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
            existing.SslHasznalata = beallitas.SslHasznalata;
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
        if (smtp is null || !smtp.Aktiv) return false;

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.KuldoNev, smtp.KuldoEmail));
            message.To.Add(MailboxAddress.Parse(cimzett));
            message.Subject = "BiztvillCRM - SMTP Teszt";
            message.Body = new TextPart("plain")
            {
                Text = $"Ez egy teszt email a BiztvillCRM rendszerből.\n\nKüldés időpontja: {DateTime.Now:yyyy.MM.dd HH:mm:ss}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.SzerverCim, smtp.Port, smtp.SslHasznalata);
            await client.AuthenticateAsync(smtp.FelhasznaloNev, smtp.Jelszo);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch
        {
            return false;
        }
    }
}