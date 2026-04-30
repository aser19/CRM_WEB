using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ITularamvedelemTipusService
{
    Task<List<TularamvedelemTipus>> GetAllAsync();
    Task<List<TularamvedelemTipus>> GetAllActiveAsync();
    Task<TularamvedelemTipus?> GetByIdAsync(int id);
    Task<TularamvedelemTipus?> GetByNevAsync(string nev);
    Task<int> CreateAsync(TularamvedelemTipus tipus);
    Task UpdateAsync(TularamvedelemTipus tipus);
    Task DeleteAsync(int id);

    /// <summary>Számítja a minősítést a mért értékek alapján</summary>
    Task<string> SzamolMinositestAsync(string tipusNev, decimal mertHurokimpedancia);
}