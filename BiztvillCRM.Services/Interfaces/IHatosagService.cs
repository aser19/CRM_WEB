using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHatosagService
{
    Task<List<Hatosag>> GetAllAsync();
    Task<Hatosag?> GetByIdAsync(int id);
    Task<Hatosag> CreateAsync(Hatosag hatosag);
    Task<Hatosag> UpdateAsync(Hatosag hatosag);
    Task DeleteAsync(int id);
}