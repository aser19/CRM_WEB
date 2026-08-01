using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IRbVedelmiModService
{
    Task<List<RbVedelmiMod>> GetAllAsync();
    Task<List<RbVedelmiMod>> GetAllActiveAsync();
    Task<RbVedelmiMod?> GetByIdAsync(int id);
    Task<RbVedelmiMod?> GetByNevAsync(string nev);
    Task<RbVedelmiMod> GetOrCreateAsync(string nev);
    Task<int> CreateAsync(RbVedelmiMod tipus);
    Task UpdateAsync(RbVedelmiMod tipus);
    Task DeleteAsync(int id);
}
