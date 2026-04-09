using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKockazatertekelésService
{
    Task<List<Kockazatertekeles>> GetAllAsync();
    Task<Kockazatertekeles?> GetByIdAsync(int id);
    Task<Kockazatertekeles> CreateAsync(Kockazatertekeles kockazatertekeles);
    Task<Kockazatertekeles> UpdateAsync(Kockazatertekeles kockazatertekeles);
    Task DeleteAsync(int id);
}