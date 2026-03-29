using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJogszabalyService
{
    Task<List<Jogszabaly>> GetAllAsync();
    Task<Jogszabaly?> GetByIdAsync(int id);
    Task<Jogszabaly> CreateAsync(Jogszabaly jogszabaly);
    Task<Jogszabaly> UpdateAsync(Jogszabaly jogszabaly);
    Task DeleteAsync(int id);
}