using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMunkavedelmiOktatasService
{
    Task<List<MunkavedelmiOktatas>> GetAllAsync(bool mindenCegre = false);
    Task<MunkavedelmiOktatas?> GetByIdAsync(int id, bool mindenCegre = false);
    Task<MunkavedelmiOktatas?> GetByIdWithResztvevokAsync(int id);
    Task<MunkavedelmiOktatas> CreateAsync(MunkavedelmiOktatas oktatas);
    Task<MunkavedelmiOktatas> UpdateAsync(MunkavedelmiOktatas oktatas);
    Task DeleteAsync(int id, bool mindenCegre = false);
    
    // Résztvevők
    Task<List<MunkavedelmiOktatasResztvevo>> GetResztvevokAsync(int oktatasId);
    Task<MunkavedelmiOktatasResztvevo> AddResztvevoAsync(int oktatasId, MunkavedelmiOktatasResztvevo resztvevo);
    Task RemoveResztvevoAsync(int resztvevoId);
    Task UpdateResztvevokAsync(int oktatasId, List<MunkavedelmiOktatasResztvevo> resztvevok);

    // Duplikáció és inaktív kezelés
    Task<List<MunkavedelmiOktatas>> GetInaktivakAsync();
    Task<MunkavedelmiOktatas?> EllenorizDuplikaciot(int ugyfelId, int? telephelyId, string megnevezes, DateTime ujDatum);
    Task InaktivvaTesz(int oktatasId);
}
