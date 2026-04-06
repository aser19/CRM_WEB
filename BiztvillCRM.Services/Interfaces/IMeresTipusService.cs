using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresTipusService
{
    Task<List<MeresTipus>> GetAllAsync();
    Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync();
    Task<MeresTipus?> GetByIdAsync(int id);
    Task<MeresTipus> CreateAsync(MeresTipus tipus);
    Task<MeresTipus> UpdateAsync(MeresTipus tipus);
    Task UpdateWithKovetelemenyekAsync(MeresTipus tipus);
    Task DeleteAsync(int id);
}