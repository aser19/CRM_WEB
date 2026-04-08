using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>SMTP beállítások kezelése (csak Admin).</summary>
public interface ISmtpBeallitasService
{
    Task<SmtpBeallitas?> GetAsync();
    Task<SmtpBeallitas> SaveAsync(SmtpBeallitas beallitas);
    Task<bool> TesztKuldesAsync(string cimzett);
}