using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresTipusService
{
    Task<List<MeresTipus>> GetAllAsync();
    Task<MeresTipus?> GetByIdAsync(int id);
    
    // ✅ ÚJ METÓDUS
    Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync();
    Task UpdateWithKovetelemenyekAsync(MeresTipus tipus);
    
    Task<int> CreateAsync(MeresTipus tipus);
    Task UpdateAsync(MeresTipus tipus);
    Task DeleteAsync(int id);

    Task<List<MeresTipusJogszabaly>> GetJogszabalyokByTipusIdAsync(int meresTipusId);
    Task MentJogszabalyHozzarendelesekAsync(int meresTipusId, List<int> jogszabalyIds);
}