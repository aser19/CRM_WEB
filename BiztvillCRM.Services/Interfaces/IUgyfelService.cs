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

public record KapcsolodoAdatok(
    int Telephelyek, 
    int Meresek, 
    int Eszkozok, 
    int Tanusitvanyok,
    int Hitelesitesek = 0,
    int MunkavedelmiOktatasok = 0,
    int Karbantartasok = 0
)
{
    public int Osszes => Telephelyek + Meresek + Eszkozok + Tanusitvanyok + Hitelesitesek + MunkavedelmiOktatasok + Karbantartasok;
    public bool VanKapcsolodoAdat => Osszes > 0;
}
