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
}