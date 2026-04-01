using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IEszkozTipusService
{
    Task<List<EszkozTipus>> GetAllAsync();
    Task<EszkozTipus?> GetByIdAsync(int id);
    Task<EszkozTipus> CreateAsync(EszkozTipus eszkozTipus);
    Task<EszkozTipus> UpdateAsync(EszkozTipus eszkozTipus);
    Task DeleteAsync(int id);
}