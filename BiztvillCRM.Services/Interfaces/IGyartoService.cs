using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IGyartoService
{
    Task<List<Gyarto>> GetAllAsync();
    Task<Gyarto?> GetByIdAsync(int id);
    Task<Gyarto> CreateAsync(Gyarto gyarto);
    Task<Gyarto> UpdateAsync(Gyarto gyarto);
    Task DeleteAsync(int id);
}