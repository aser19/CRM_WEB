using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IFelulvizsgaloService
{
    // Felülvizsgálók
    Task<List<Felulvizsgalo>> GetAllAsync();
    Task<List<Felulvizsgalo>> GetAllAdminAsync();
    Task<Felulvizsgalo?> GetByIdAsync(int id);
    Task<Felulvizsgalo?> GetByIdWithDetailsAsync(int id);
    Task<Felulvizsgalo> CreateAsync(Felulvizsgalo felulvizsgalo);
    Task<Felulvizsgalo> UpdateAsync(Felulvizsgalo felulvizsgalo);
    Task DeleteAsync(int id);
    
    // Képzések
    Task<FelulvizsgaloKepzes> AddKepzesAsync(int felulvizsgaloId, FelulvizsgaloKepzes kepzes);
    Task<FelulvizsgaloKepzes> UpdateKepzesAsync(FelulvizsgaloKepzes kepzes);
    Task DeleteKepzesAsync(int kepzesId);
    
    // Továbbképzések
    Task<KepzesTovabbkepzes> AddTovabbkepzesAsync(int kepzesId, KepzesTovabbkepzes tovabbkepzes);
    Task DeleteTovabbkepzesAsync(int tovabbkepzesId);
    
    // Lekérdezések
    Task<List<Felulvizsgalo>> GetLejaroKepzesekkelAsync(int naponBelul = 90);
}