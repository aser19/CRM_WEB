using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ISugoService
{
    Task<List<SugoKategoria>> GetAllWithTemakAsync();
    Task<SugoKategoria?> GetByIdAsync(int id);
    Task<int> CreateKategoriaAsync(SugoKategoria kategoria);
    Task UpdateKategoriaAsync(SugoKategoria kategoria);
    Task DeleteKategoriaAsync(int id);

    Task<SugoTema?> GetTemaByIdAsync(int id);
    Task<int> CreateTemaAsync(SugoTema tema);
    Task UpdateTemaAsync(SugoTema tema);
    Task DeleteTemaAsync(int id);

    Task SeedIfEmptyAsync();
}
