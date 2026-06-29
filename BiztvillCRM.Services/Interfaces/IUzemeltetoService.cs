using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Üzemeltető modul service interfész - admin által létrehozott sablonok és üzemeltető által rögzített adatok kezelése.
/// </summary>
public interface IUzemeltetoService
{
    // --- Sablonok (admin műveletei) ---

    /// <summary>Összes aktív sablon lekérdezése (tenant szűréssel)</summary>
    Task<List<UzemeltetoSablon>> GetSablonokAsync();

    /// <summary>Inaktív sablonok lekérdezése</summary>
    Task<List<UzemeltetoSablon>> GetInaktivSablonokAsync();

    /// <summary>Sablon lekérdezése ID alapján (mezőkkel együtt)</summary>
    Task<UzemeltetoSablon?> GetSablonByIdAsync(int id);

    /// <summary>Új sablon létrehozása</summary>
    Task<UzemeltetoSablon> CreateSablonAsync(UzemeltetoSablon sablon);

    /// <summary>Sablon frissítése</summary>
    Task<UzemeltetoSablon> UpdateSablonAsync(UzemeltetoSablon sablon);

    /// <summary>Sablon törlése (inaktívvá tétel)</summary>
    Task DeleteSablonAsync(int id);

    /// <summary>Sablon mező hozzáadása</summary>
    Task<UzemeltetoSablonMezo> AddSablonMezoAsync(UzemeltetoSablonMezo mezo);

    /// <summary>Sablon mező frissítése</summary>
    Task<UzemeltetoSablonMezo> UpdateSablonMezoAsync(UzemeltetoSablonMezo mezo);

    /// <summary>Sablon mező törlése</summary>
    Task DeleteSablonMezoAsync(int mezoId);

    // --- Adatok (üzemeltető műveletei) ---

    /// <summary>Összes adat lekérdezése (tenant szűréssel, csak aktív adatok)</summary>
    Task<List<UzemeltetoAdat>> GetAdatokAsync();

    /// <summary>Adatok lekérdezése sablon szerint</summary>
    Task<List<UzemeltetoAdat>> GetAdatokBySablonIdAsync(int sablonId);

    /// <summary>Adat lekérdezése ID alapján</summary>
    Task<UzemeltetoAdat?> GetAdatByIdAsync(int id);

    /// <summary>Új adat rögzítése</summary>
    Task<UzemeltetoAdat> CreateAdatAsync(UzemeltetoAdat adat);

    /// <summary>Adat frissítése</summary>
    Task<UzemeltetoAdat> UpdateAdatAsync(UzemeltetoAdat adat);

    /// <summary>Adat törlése (inaktívvá tétel)</summary>
    Task DeleteAdatAsync(int id);

    /// <summary>Lejáró adatok lekérdezése (következő X nap)</summary>
    Task<List<UzemeltetoAdat>> GetLejaroAdatokAsync(int napok = 30);
}
