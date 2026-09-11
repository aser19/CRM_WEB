using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHitelesitesService
{
    Task<List<Hitelesites>> GetAllAsync(bool includeInaktivak = false);
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

    /// <summary>
    /// Be- vagy kikapcsolja egy hitelesítés aktív állapotát.
    /// </summary>
    Task AktivAllapotValtasAsync(int hitelesitesId, bool aktiv);

    /// <summary>
    /// Ellenőrzi, hogy létezik-e ugyanazokkal az adatokkal (ügyfél, telephely, eszköztípus, eszközazonosító) rendelkező,
    /// az adott hitelesítésnél frissőbb dátumú aktív hitelesítés.
    /// </summary>
    Task<bool> VanFrissebbAktivAsync(int hitelesitesId);
}
