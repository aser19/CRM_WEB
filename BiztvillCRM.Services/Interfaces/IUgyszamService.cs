using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IUgyszamService
{
    Task<List<Ugyszam>> GetAllAsync();
    Task<Ugyszam?> GetByIdAsync(int id);
    Task<Ugyszam> CreateAsync(Ugyszam ugyszam);
    Task<Ugyszam> UpdateAsync(Ugyszam ugyszam);
    Task DeleteAsync(int id);
}