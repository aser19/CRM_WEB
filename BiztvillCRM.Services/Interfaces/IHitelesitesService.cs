using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHitelesitesService
{
    Task<List<Hitelesites>> GetAllAsync();
    Task<List<Hitelesites>> GetInaktivakAsync();
    Task<Hitelesites?> GetByIdAsync(int id);
    Task<Hitelesites> CreateAsync(Hitelesites hitelesites);
    Task<Hitelesites> UpdateAsync(Hitelesites hitelesites);
    Task DeleteAsync(int id);

    /// <summary>
    /// Ellenőrzi, hogy létezik-e már hasonló hitelesítés (duplikáció).
    /// </summary>
    Task<Hitelesites?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int eszkozTipusId, string? eszkozAzonosito, DateTime ujHitelesDatum);

    /// <summary>
    /// Inaktívvá tesz egy hitelesítést.
    /// </summary>
    Task InaktivvaTesz(int hitelesitesId);
}
