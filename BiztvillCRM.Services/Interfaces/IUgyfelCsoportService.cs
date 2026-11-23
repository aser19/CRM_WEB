using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IUgyfelCsoportService
{
    Task<List<UgyfelCsoport>> GetAllAsync();
    Task<UgyfelCsoport> CreateAsync(string nev);

    /// <summary>Leválasztja az ügyfelet a csoportjáról (pl. ha külön ügyintézőhöz kerül, és újra szét kell választani).</summary>
    Task LevalasztasAsync(int ugyfelId);
}
