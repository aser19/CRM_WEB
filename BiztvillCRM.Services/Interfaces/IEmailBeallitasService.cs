using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Cég email beállítások kezelése.</summary>
public interface IEmailBeallitasService
{
    Task<EmailBeallitas?> GetByCegIdAsync(int cegId);
    Task<EmailBeallitas> SaveAsync(EmailBeallitas beallitas);
    Task<List<EmailBeallitas>> GetAllAktivAsync();
    
    // Alapértelmezett beállítások (Admin)
    Task<AlapertelmezettEmailBeallitas> GetAlapertelmezettAsync();
    Task<AlapertelmezettEmailBeallitas> SaveAlapertelmezettAsync(AlapertelmezettEmailBeallitas beallitas);
    
    /// <summary>Új cég létrehozásakor hívjuk - lemásolja az alapértelmezett beállításokat.</summary>
    Task<EmailBeallitas> CreateForNewCegAsync(int cegId);
}