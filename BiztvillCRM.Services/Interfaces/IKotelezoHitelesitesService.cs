using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKotelezoHitelesitesService
{
    Task<List<KotelezoHitelesites>> GetAllAsync();
    Task<KotelezoHitelesites?> GetByIdAsync(int id);
    Task<KotelezoHitelesites> CreateAsync(KotelezoHitelesites kotelezoHitelesites);
    Task<KotelezoHitelesites> UpdateAsync(KotelezoHitelesites kotelezoHitelesites);
    Task DeleteAsync(int id);
}