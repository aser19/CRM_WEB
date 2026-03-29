using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IKalibracioService
{
    Task<List<Kalibracio>> GetAllAsync();
    Task<Kalibracio?> GetByIdAsync(int id);
    Task<Kalibracio> CreateAsync(Kalibracio kalibracio);
    Task<Kalibracio> UpdateAsync(Kalibracio kalibracio);
    Task DeleteAsync(int id);
}