using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Munkaszám törzsadatok kezelése (céghez kötött nyilvántartás).
/// </summary>
public interface IMunkaszamTorzsService
{
    Task<List<Munkaszam>> GetAllAsync();
    Task<Munkaszam?> GetByIdAsync(int id);
    Task<Munkaszam> CreateAsync(Munkaszam munkaszam);
    Task<Munkaszam> UpdateAsync(Munkaszam munkaszam);
    Task DeleteAsync(int id);

    /// <summary>
    /// Munkaszámok keresése az aktuális céghez, a begépelt szöveg alapján (autocomplete-hez).
    /// </summary>
    Task<List<Munkaszam>> KeresAsync(string kereses, int maxEredmeny = 20);
}
