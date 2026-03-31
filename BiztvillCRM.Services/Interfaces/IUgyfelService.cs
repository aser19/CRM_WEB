using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IUgyfelService
{
    Task<List<Ugyfel>> GetAllAsync();
    Task<Ugyfel?> GetByIdAsync(int id);
    Task<Ugyfel> CreateAsync(Ugyfel ugyfel);
    Task<Ugyfel> UpdateAsync(Ugyfel ugyfel);
    Task DeleteAsync(int id);
    Task<KapcsolodoAdatok> GetKapcsolodoAdatokAsync(int ugyfelId);
    Task DeleteWithRelatedDataAsync(int id);
}

/// <summary>Ügyfélhez kapcsolódó adatok összesítése törlés előtt.</summary>
public record KapcsolodoAdatok(
    int TelephelyekSzama,
    int MeresekSzama,
    int EszkozokSzama,
    int TanusitvanyokSzama
)
{
    public bool VanKapcsolodoAdat => TelephelyekSzama > 0 || MeresekSzama > 0 ||
                                      EszkozokSzama > 0 || TanusitvanyokSzama > 0;

    public string Osszefoglalas()
    {
        var lista = new List<string>();
        if (TelephelyekSzama > 0) lista.Add($"{TelephelyekSzama} telephely");
        if (MeresekSzama > 0) lista.Add($"{MeresekSzama} mérés");
        if (EszkozokSzama > 0) lista.Add($"{EszkozokSzama} eszköz");
        if (TanusitvanyokSzama > 0) lista.Add($"{TanusitvanyokSzama} tanúsítvány");
        return string.Join(", ", lista);
    }
}