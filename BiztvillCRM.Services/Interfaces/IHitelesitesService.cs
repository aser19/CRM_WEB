using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHitelesitesService
{
    Task<List<Hitelesites>> GetAllAsync();
    Task<Hitelesites?> GetByIdAsync(int id);
    Task<Hitelesites> CreateAsync(Hitelesites hitelesites);
    Task<Hitelesites> UpdateAsync(Hitelesites hitelesites);
    Task DeleteAsync(int id);
}