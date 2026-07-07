using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Üzemeltető felhasználók és sablonok közötti hozzárendelések kezelése
/// </summary>
public interface IUzemeltetoSablonFelhasznaloService
{
    /// <summary>Sablon összes üzemeltetőjének lekérése</summary>
    Task<List<UzemeltetoSablonFelhasznalo>> GetBySablonIdAsync(int sablonId);

    /// <summary>Felhasználó összes sablonának lekérése (üzemeltető szerepkörben)</summary>
    Task<List<UzemeltetoSablonFelhasznalo>> GetByFelhasznaloIdAsync(string felhasznaloId);

    /// <summary>Üzemeltető hozzárendelése sablonhoz</summary>
    Task<UzemeltetoSablonFelhasznalo> HozzarendelAsync(int sablonId, string felhasznaloId);

    /// <summary>Hozzárendelés eltávolítása</summary>
    Task TorlesAsync(int id);

    /// <summary>Hozzárendelés aktív/inaktív állapotának változtatása</summary>
    Task SetAktivAsync(int id, bool aktiv);

    /// <summary>Ellenőrzi, hogy a felhasználó hozzá van-e rendelve a sablonhoz</summary>
    Task<bool> IsHozzarendelveAsync(int sablonId, string felhasznaloId);

    /// <summary>Összes aktív hozzárendelés lekérése</summary>
    Task<List<UzemeltetoSablonFelhasznalo>> GetAllAktivAsync();
}
