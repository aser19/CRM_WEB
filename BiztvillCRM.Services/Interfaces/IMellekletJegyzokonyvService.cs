using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMellekletJegyzokonyvService
{
    Task<List<MellekletJegyzokonyv>> GetByMeresIdAsync(int meresId);
    Task<MellekletJegyzokonyv?> GetByIdAsync(int id);
    Task<MellekletJegyzokonyv> LetrehozVagyFrissitAsync(int meresId, string tipus, string szam);
    Task MentAdatokAsync(int id, string adatokJson, bool kesz = false);
    Task<bool> MindenKeszeE(int meresId);
    Task<int> MellekletMeresLetrehozAsync(int mellekletId, int meresTipusId, JegyzokonyvAdatok foAdatok);
    Task MentAvkAdatokAsync(int mellekletId, JegyzokonyvAdatok adatok);
    /// <summary>Visszaadja azon Meres.Id-k halmazát, amelyek melléklet-mérések (nem jelennek meg a főlistában).</summary>
    Task<HashSet<int>> GetMellekletMeresIdsAsync();
    /// <summary>Visszaadja azon Meres.Id-k halmazát, amelyekhez van legalább 1 melléklet.</summary>
    Task<HashSet<int>> GetMeresIdsWithMellekletAsync();
    Task StatuszFrissitesAsync(int mellekletId, string ujStatusz);

    /// <summary>
    /// Törli a megadott melléklet-jegyzőkönyvet, valamint a hozzá tartozó önálló melléklet-mérést (Meres), ha van.
    /// </summary>
    Task TorlesAsync(int mellekletId);

    /// <summary>
    /// Törli a mérés adott típusú melléklet-jegyzőkönyvét (ha létezik), a hozzá tartozó melléklet-méréssel együtt.
    /// Akkor hasznos, amikor a főjegyzőkönyv szerkesztésekor a felhasználó kiveszi a pipát egy melléklet elől.
    /// </summary>
    Task TorlesTipusAlapjanAsync(int meresId, string tipus);
}