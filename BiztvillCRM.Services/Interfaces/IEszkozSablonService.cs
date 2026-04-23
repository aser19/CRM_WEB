using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IEszkozSablonService
{
    Task<List<EszkozSablon>> GetAllAsync();
    Task<EszkozSablon?> GetByIdAsync(int id);
    Task<EszkozSablon?> GetByMegnevezesAsync(string megnevezes);
    Task<EszkozSablon> CreateAsync(EszkozSablon sablon);
    Task<EszkozSablon> UpdateAsync(EszkozSablon sablon);
    Task DeleteAsync(int id);
}