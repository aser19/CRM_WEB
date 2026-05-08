using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IAvkVedelemTipusService
{
    Task<List<AvkVedelemTipus>> GetAktivTipusokAsync();
    Task<AvkVedelemTipus?> GetByIdAsync(int id);
    Task MentesAsync(AvkVedelemTipus tipus);
    Task TorlesAsync(int id);
}