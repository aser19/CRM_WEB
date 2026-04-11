using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Email sablonok kezelése.</summary>
public interface IEmailSablonService
{
    Task<List<EmailSablon>> GetAllAsync();
    Task<List<EmailSablon>> GetAllFilteredByModulokAsync(ModulJogosultsag aktivModulok);
    Task<EmailSablon?> GetByIdAsync(int id);
    Task<EmailSablon?> GetByTipusAsync(EmailErtesitesTipus tipus, int? cegId = null);
    Task<EmailSablon> CreateAsync(EmailSablon sablon);
    Task<EmailSablon> UpdateAsync(EmailSablon sablon);
    Task DeleteAsync(int id);
}