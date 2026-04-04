using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ISablonService
{
    Task<List<JegyzokonyvSablon>> GetAllAsync();
    Task<JegyzokonyvSablon?> GetByIdAsync(string id);
    string GetSablonPath(string fajlNev);
}