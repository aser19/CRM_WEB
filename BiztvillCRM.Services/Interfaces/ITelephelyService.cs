using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ITelephelyService
{
    Task<List<Telephely>> GetAllAsync();
    Task<Telephely?> GetByIdAsync(int id);
    Task<Telephely> CreateAsync(Telephely telephely);
    Task<Telephely> UpdateAsync(Telephely telephely);
    Task DeleteAsync(int id);
}