using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKepzesTipusService
{
    Task<List<KepzesTipus>> GetAllAsync();
    Task<List<KepzesTipus>> GetAllActiveAsync();
    Task<KepzesTipus?> GetByIdAsync(int id);
    Task<KepzesTipus> CreateAsync(KepzesTipus tipus);
    Task<KepzesTipus> UpdateAsync(KepzesTipus tipus);
    Task DeleteAsync(int id);
}