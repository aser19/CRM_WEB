using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IAvkVedelemTipusService
{
    Task<List<AvkVedelemTipus>> GetAktivTipusokAsync();
    Task<AvkVedelemTipus?> GetByIdAsync(int id);
    Task<AvkVedelemTipus?> GetByNevAsync(string nev);
    Task<AvkVedelemTipus> GetOrCreateAsync(string tipusNev);
    Task MentesAsync(AvkVedelemTipus tipus);
    Task TorlesAsync(int id);
}