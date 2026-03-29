using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKarbantartasService
{
    Task<List<Karbantartas>> GetAllAsync();
    Task<Karbantartas?> GetByIdAsync(int id);
    Task<Karbantartas> CreateAsync(Karbantartas karbantartas);
    Task<Karbantartas> UpdateAsync(Karbantartas karbantartas);
    Task DeleteAsync(int id);
}