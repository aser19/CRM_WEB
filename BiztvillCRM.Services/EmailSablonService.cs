using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EmailSablonService : IEmailSablonService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public EmailSablonService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<EmailSablon>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        // Admin látja az összeset, egyébként csak a globális + saját cég sablonjait
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await _context.EmailSablonok
                .Include(s => s.Ceg)
                .OrderBy(s => s.Tipus)
                .ThenBy(s => s.CegId)
                .ToListAsync();
        }

        return await _context.EmailSablonok
            .Where(s => s.CegId == null || s.CegId == cegId)
            .OrderBy(s => s.Tipus)
            .ToListAsync();
    }

    public async Task<EmailSablon?> GetByIdAsync(int id) =>
        await _context.EmailSablonok.FindAsync(id);

    public async Task<EmailSablon?> GetByTipusAsync(EmailErtesitesTipus tipus, int? cegId = null)
    {
        // Először cég-specifikus sablon, ha nincs, akkor globális
        var sablon = await _context.EmailSablonok
            .FirstOrDefaultAsync(s => s.Tipus == tipus && s.CegId == cegId && s.Aktiv);

        if (sablon is null && cegId.HasValue)
        {
            sablon = await _context.EmailSablonok
                .FirstOrDefaultAsync(s => s.Tipus == tipus && s.CegId == null && s.Aktiv);
        }

        return sablon;
    }

    public async Task<EmailSablon> CreateAsync(EmailSablon sablon)
    {
        sablon.Letrehozva = DateTime.UtcNow;
        _context.EmailSablonok.Add(sablon);
        await _context.SaveChangesAsync();
        return sablon;
    }

    public async Task<EmailSablon> UpdateAsync(EmailSablon sablon)
    {
        var existing = await _context.EmailSablonok.FindAsync(sablon.Id)
            ?? throw new InvalidOperationException("Sablon nem található.");

        existing.Nev = sablon.Nev;
        existing.Tipus = sablon.Tipus;
        existing.Targy = sablon.Targy;
        existing.Szoveg = sablon.Szoveg;
        existing.Aktiv = sablon.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var sablon = await _context.EmailSablonok.FindAsync(id);
        if (sablon is not null)
        {
            _context.EmailSablonok.Remove(sablon);
            await _context.SaveChangesAsync();
        }
    }
}