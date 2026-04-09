using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IZonaterkepService
{
    Task<List<Zonaterkep>> GetAllAsync();
    Task<Zonaterkep?> GetByIdAsync(int id);
    Task<Zonaterkep> CreateAsync(Zonaterkep zonaterkep);
    Task<Zonaterkep> UpdateAsync(Zonaterkep zonaterkep);
    Task DeleteAsync(int id);
}