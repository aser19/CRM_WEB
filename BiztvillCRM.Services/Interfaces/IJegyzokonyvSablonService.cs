using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJegyzokonyvSablonService
{
    /// <summary>Visszaadja az adott méréstype + oldal aktív tételeit sorrendben.</summary>
    Task<List<JegyzokonyvSablonTetel>> GetTetelek(int meresTipusId, int oldalSzam);

    /// <summary>Összes tétel (admin felületre).</summary>
    Task<List<JegyzokonyvSablonTetel>> GetOsszesTetelek(int? meresTipusId = null);

    Task<JegyzokonyvSablonTetel?> GetByIdAsync(int id);
    Task<JegyzokonyvSablonTetel> CreateAsync(JegyzokonyvSablonTetel tetel);
    Task<JegyzokonyvSablonTetel> UpdateAsync(JegyzokonyvSablonTetel tetel);
    Task DeleteAsync(int id);

    /// <summary>Tömeges import (pl. OCR-ből érkező tételek).</summary>
    Task<List<JegyzokonyvSablonTetel>> ImportAlakAsync(int meresTipusId, int oldalSzam,
        string kategoria, List<string> feliratok, string ertekek = "MF;NMF;NA");
}