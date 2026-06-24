using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKarbantartasService
{
    Task<List<Karbantartas>> GetAllAsync();
    Task<Karbantartas?> GetByIdAsync(int id);
    Task<Karbantartas> CreateAsync(Karbantartas karbantartas);
    Task<Karbantartas> UpdateAsync(Karbantartas karbantartas);
    Task DeleteAsync(int id);
    Task<List<Karbantartas>> GetInaktivakAsync();
    Task<Karbantartas?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int karbantartasTipusId, DateTime ujDatum);
    Task InaktivvaTesz(int karbantartasId);
    Task StatuszFrissites(int karbantartasId, KarbantartasStatusz ujStatusz);
}
