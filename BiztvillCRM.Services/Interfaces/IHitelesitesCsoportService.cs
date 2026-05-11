using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHitelesitesCsoportService
{
    Task<List<HitelesitesCsoport>> GetAllAsync();
    Task<HitelesitesCsoport?> GetByIdAsync(int id);
    Task<int> CreateAsync(HitelesitesCsoport csoport);
    Task UpdateAsync(HitelesitesCsoport csoport);
    Task DeleteAsync(int id);
}