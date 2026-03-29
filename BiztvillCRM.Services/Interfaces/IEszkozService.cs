using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IEszkozService
{
    Task<List<Eszkoz>> GetAllAsync();
    Task<Eszkoz?> GetByIdAsync(int id);
    Task<Eszkoz> CreateAsync(Eszkoz eszkoz);
    Task<Eszkoz> UpdateAsync(Eszkoz eszkoz);
    Task DeleteAsync(int id);
}