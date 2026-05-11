using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresCsoportService
{
    Task<List<MeresCsoport>> GetAllAsync();
    Task<MeresCsoport?> GetByIdAsync(int id);
    Task<int> CreateAsync(MeresCsoport csoport);
    Task UpdateAsync(MeresCsoport csoport);
    Task DeleteAsync(int id);
}