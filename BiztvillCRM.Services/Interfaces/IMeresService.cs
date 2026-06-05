using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresService
{
    Task<List<Meres>> GetAllAsync();
    Task<Meres?> GetByIdAsync(int id);
    Task<Meres> CreateAsync(Meres meres);
    Task<Meres> UpdateAsync(Meres meres);
    Task DeleteAsync(int id);

    Task<JegyzokonyvAdatok?> BetoltJegyzokonyvAdatokAsync(int meresId);
    Task MentesJegyzokonyvAdatokkalAsync(int meresId, JegyzokonyvAdatok adatok);
    Task MentesJegyzokonyvAdatokkalEsStatuszAsync(int meresId, JegyzokonyvAdatok adatok, MeresStatusz statusz, string? eredmeny);
    Task StatuszFrissitesAsync(int meresId, MeresStatusz statusz, string? eredmeny);
}