using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IEszkozSablonService
{
    Task<List<EszkozSablon>> GetAllAsync();
    Task<EszkozSablon?> GetByIdAsync(int id);
    Task<EszkozSablon?> GetByEszkozTipusNevAsync(string eszkozTipusNev);
    Task<int> CreateAsync(EszkozSablon sablon);
    Task UpdateAsync(EszkozSablon sablon);
    Task DeleteAsync(int id);
    
    /// <summary>Alkatrészek generálása eszköztípus neve alapján</summary>
    Task<List<HordozhatoEszkozSor>> GeneralAlkatreszekAsync(
        string eszkozTipusNev,
        int parentSorsz, 
        string parentCsoportNev);
}