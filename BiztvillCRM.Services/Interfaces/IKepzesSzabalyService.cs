// BiztvillCRM.Services/Interfaces/IKepzesSzabalyService.cs
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKepzesSzabalyService
{
    // CRUD
    Task<List<KepzesSzabaly>> GetAllAsync();
    Task<List<KepzesSzabaly>> GetByTipusAsync(KepzesSzabalyTipus tipus);
    Task<List<KepzesSzabaly>> GetByKepzesTipusIdAsync(int kepzesTipusId);
    Task<KepzesSzabaly?> GetByIdAsync(int id);
    Task<KepzesSzabaly> CreateAsync(KepzesSzabaly szabaly);
    Task UpdateAsync(KepzesSzabaly szabaly);
    Task DeleteAsync(int id);
    
    // Üzleti logika
    /// <summary>Egy képzés megújításakor mely más képzéseket kell automatikusan megújítani?</summary>
    Task<List<int>> GetMegujitandoKepzesTipusIdkAsync(int tovabbkepzesTipusId);
    
    /// <summary>Egy felülvizsgáló mentesül-e a továbbképzés alól más képzései alapján?</summary>
    Task<bool> FelmentettETovabbkepzesAlolAsync(int kepzesTipusId, IEnumerable<int> felulvizsgaloOsszesKepzesTipusai);
    
    /// <summary>Egy képzés rögzíthető-e? (előfeltételek ellenőrzése)</summary>
    Task<(bool Engedelyezett, List<string> HianyzoElofeltetelek)> EllenorizElofelteteelketAsync(
        int kepzesTipusId, IEnumerable<int> felulvizsgaloMegleVoKepzesTipusai);
    
    /// <summary>Lejáró képzések lekérdezése figyelmeztetéshez</summary>
    Task<List<LejaroKepzesInfo>> GetLejaroKepzesekAsync(int cegId, int naponBelul = 90);
}

public record LejaroKepzesInfo(
    int KepzesId,
    string FelulvizsgaloNev,
    string KepzesTipusNev,
    DateTime LejaratDatum,
    int HatralevoNapok,
    bool FelmentettTovabbkepzesAlol);