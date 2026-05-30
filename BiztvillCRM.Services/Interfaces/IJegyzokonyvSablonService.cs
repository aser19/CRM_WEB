using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJegyzokonyvSablonService
{
    /// <summary>Visszaadja az adott méréstype + oldal aktív tételeit (admin + cég saját sablonjai).</summary>
    Task<List<JegyzokonyvSablonTetel>> GetTetelek(int meresTipusId, int oldalSzam, int? cegId = null);

    /// <summary>Összes tétel (admin felületre), szűrhető mérés típus és cég szerint.</summary>
    Task<List<JegyzokonyvSablonTetel>> GetOsszesTetelek(int? meresTipusId = null, int? cegId = null, bool adminSablonokIs = true);

    Task<JegyzokonyvSablonTetel?> GetByIdAsync(int id);
    Task<JegyzokonyvSablonTetel> CreateAsync(JegyzokonyvSablonTetel tetel);
    Task<JegyzokonyvSablonTetel> UpdateAsync(JegyzokonyvSablonTetel tetel);
    Task DeleteAsync(int id);

    /// <summary>Admin sablon(ok) klónozása egy adott céghez.</summary>
    Task<List<JegyzokonyvSablonTetel>> KlonozasCegnek(int meresTipusId, int celCegId);

    /// <summary>Egyetlen tétel klónozása egy céghez.</summary>
    Task<JegyzokonyvSablonTetel> EgyTetelKlonozasa(int tetelId, int celCegId);

    /// <summary>Tömeges import (pl. OCR-ből érkező tételek).</summary>
    Task<List<JegyzokonyvSablonTetel>> ImportAlakAsync(int meresTipusId, int oldalSzam,
        string kategoria, List<string> feliratok, string ertekek = "MF;NMF;NA");
}