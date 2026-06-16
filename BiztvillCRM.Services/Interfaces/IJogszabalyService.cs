using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJogszabalyService
{
    Task<List<Jogszabaly>> GetAllAsync();
    Task<Jogszabaly?> GetByIdAsync(int id);
    Task<Jogszabaly> CreateAsync(Jogszabaly jogszabaly);
    Task<Jogszabaly> UpdateAsync(Jogszabaly jogszabaly);
    Task DeleteAsync(int id);

    // --- Tagek ---
    Task<List<JogszabalyTag>> GetAllTagekAsync();
    Task<JogszabalyTag> CreateTagAsync(JogszabalyTag tag);
    Task DeleteTagAsync(int id);
    Task SetTagekAsync(int jogszabalyId, List<int> tagIds);
}