using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Ügyfél CRUD műveletek interfésze.</summary>
public interface IUgyfelService
{
    /// <summary>Az összes ügyfél lekérése.</summary>
    Task<List<Ugyfel>> GetAllAsync();

    /// <summary>Egy ügyfél lekérése azonosító alapján.</summary>
    Task<Ugyfel?> GetByIdAsync(int id);

    /// <summary>Új ügyfél létrehozása.</summary>
    Task<Ugyfel> CreateAsync(Ugyfel ugyfel);

    /// <summary>Meglévő ügyfél módosítása.</summary>
    Task<Ugyfel> UpdateAsync(Ugyfel ugyfel);

    /// <summary>Ügyfél törlése azonosító alapján.</summary>
    Task DeleteAsync(int id);
}
