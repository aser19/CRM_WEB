using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ITanusitvanyService
{
    Task<List<Tanusitvany>> GetAllAsync();
    Task<Tanusitvany?> GetByIdAsync(int id);
    Task<Tanusitvany> CreateAsync(Tanusitvany tanusitvany);
    Task<Tanusitvany> UpdateAsync(Tanusitvany tanusitvany);
    Task DeleteAsync(int id);
}