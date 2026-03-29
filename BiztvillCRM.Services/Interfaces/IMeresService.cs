using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresService
{
    Task<List<Meres>> GetAllAsync();
    Task<Meres?> GetByIdAsync(int id);
    Task<Meres> CreateAsync(Meres meres);
    Task<Meres> UpdateAsync(Meres meres);
    Task DeleteAsync(int id);
}