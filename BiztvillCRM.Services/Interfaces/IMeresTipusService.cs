using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresTipusService
{
    Task<List<MeresTipus>> GetAllAsync();
    Task<MeresTipus?> GetByIdAsync(int id);
    Task<MeresTipus> CreateAsync(MeresTipus meresTipus);
    Task<MeresTipus> UpdateAsync(MeresTipus meresTipus);
    Task DeleteAsync(int id);
}