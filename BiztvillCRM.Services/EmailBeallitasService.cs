using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EmailBeallitasService : IEmailBeallitasService
{
    private readonly CrmDbContext _context;

    public EmailBeallitasService(CrmDbContext context) => _context = context;

    public async Task<EmailBeallitas?> GetByCegIdAsync(int cegId) =>
        await _context.EmailBeallitasok
            .Include(e => e.Ceg)
            .FirstOrDefaultAsync(e => e.CegId == cegId);

    public async Task<EmailBeallitas> SaveAsync(EmailBeallitas beallitas)
    {
        var existing = await _context.EmailBeallitasok
            .FirstOrDefaultAsync(e => e.CegId == beallitas.CegId);

        if (existing is null)
        {
            beallitas.Letrehozva = DateTime.UtcNow;
            _context.EmailBeallitasok.Add(beallitas);
            await _context.SaveChangesAsync();
            return beallitas;
        }

        existing.ErtesitesTipusok = beallitas.ErtesitesTipusok;
        existing.CimzettTipusok = beallitas.CimzettTipusok;
        existing.EgyediEmailCimek = beallitas.EgyediEmailCimek;
        existing.Aktiv = beallitas.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<List<EmailBeallitas>> GetAllAktivAsync() =>
        await _context.EmailBeallitasok
            .Include(e => e.Ceg)
            .Where(e => e.Aktiv)
            .ToListAsync();
}