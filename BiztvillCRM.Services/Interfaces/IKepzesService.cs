using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKepzesService
{
    Task<List<Kepzes>> GetAllAsync();
    Task<Kepzes?> GetByIdAsync(int id);
    Task<Kepzes> CreateAsync(Kepzes kepzes);
    Task<Kepzes> UpdateAsync(Kepzes kepzes);
    Task DeleteAsync(int id);
}