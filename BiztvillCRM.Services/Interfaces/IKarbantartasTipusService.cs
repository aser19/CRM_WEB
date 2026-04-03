using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKarbantartasTipusService
{
    Task<List<KarbantartasTipus>> GetAllAsync();
    Task<List<KarbantartasTipus>> GetActiveAsync();
    Task<KarbantartasTipus?> GetByIdAsync(int id);
    Task<KarbantartasTipus> CreateAsync(KarbantartasTipus tipus);
    Task<KarbantartasTipus> UpdateAsync(KarbantartasTipus tipus);
    Task DeleteAsync(int id);
}