using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ITerminalService
{
    Task<List<Terminal>> GetAllAsync();
    Task<Terminal?> GetByIdAsync(int id);
    Task<Terminal> CreateAsync(Terminal terminal);
    Task<Terminal> UpdateAsync(Terminal terminal);
    Task DeleteAsync(int id);
}