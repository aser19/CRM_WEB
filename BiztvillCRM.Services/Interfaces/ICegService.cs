using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ICegService
{
    Task<List<Ceg>> GetAllAsync(bool csakAktiv = true);
    Task<Ceg?> GetByIdAsync(int id);
    Task<Ceg> CreateAsync(Ceg ceg);
    Task<Ceg> UpdateAsync(Ceg ceg);
    Task<bool> SetAktivAsync(int id, bool aktiv);
    Task<int> GetFelhasznalokSzamaAsync(int cegId);
    Task<int> GetUgyfelekSzamaAsync(int cegId);
}