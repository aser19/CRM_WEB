using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IErintesvedelmiModOsztalyService
{
    Task<List<ErintesvedelmiModOsztaly>> GetAllAsync();
    Task<List<ErintesvedelmiModOsztaly>> GetAllActiveAsync();
    Task<ErintesvedelmiModOsztaly?> GetByIdAsync(int id);
    Task<int> CreateAsync(ErintesvedelmiModOsztaly modOsztaly);
    Task UpdateAsync(ErintesvedelmiModOsztaly modOsztaly);
    Task DeleteAsync(int id);
}