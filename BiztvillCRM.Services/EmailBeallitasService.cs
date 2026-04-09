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
        await _context.EmailBeallitasok.FirstOrDefaultAsync(e => e.CegId == cegId);

    public async Task<EmailBeallitas> SaveAsync(EmailBeallitas beallitas)
    {
        if (beallitas.Id == 0)
        {
            beallitas.Letrehozva = DateTime.UtcNow;
            _context.EmailBeallitasok.Add(beallitas);
        }
        else
        {
            beallitas.Modositva = DateTime.UtcNow;
            _context.EmailBeallitasok.Update(beallitas);
        }

        await _context.SaveChangesAsync();
        return beallitas;
    }

    public async Task<List<EmailBeallitas>> GetAllAktivAsync() =>
        await _context.EmailBeallitasok.Where(e => e.Aktiv).ToListAsync();

    // === Alapértelmezett beállítások ===

    public async Task<AlapertelmezettEmailBeallitas> GetAlapertelmezettAsync()
    {
        var beallitas = await _context.AlapertelmezettEmailBeallitasok.FirstOrDefaultAsync();
        return beallitas ?? new AlapertelmezettEmailBeallitas { Aktiv = true };
    }

    public async Task<AlapertelmezettEmailBeallitas> SaveAlapertelmezettAsync(AlapertelmezettEmailBeallitas beallitas)
    {
        var existing = await _context.AlapertelmezettEmailBeallitasok.FirstOrDefaultAsync();

        if (existing is null)
        {
            beallitas.Letrehozva = DateTime.UtcNow;
            _context.AlapertelmezettEmailBeallitasok.Add(beallitas);
        }
        else
        {
            existing.ErtesitesTipusok = beallitas.ErtesitesTipusok;
            existing.CimzettTipusok = beallitas.CimzettTipusok;
            existing.Aktiv = beallitas.Aktiv;
            existing.Modositva = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existing ?? beallitas;
    }

    public async Task<EmailBeallitas> CreateForNewCegAsync(int cegId)
    {
        var alapertelmezett = await GetAlapertelmezettAsync();

        var ujBeallitas = new EmailBeallitas
        {
            CegId = cegId,
            ErtesitesTipusok = alapertelmezett.ErtesitesTipusok,
            CimzettTipusok = alapertelmezett.CimzettTipusok,
            Aktiv = alapertelmezett.Aktiv,
            Letrehozva = DateTime.UtcNow
        };

        _context.EmailBeallitasok.Add(ujBeallitas);
        await _context.SaveChangesAsync();

        return ujBeallitas;
    }
}