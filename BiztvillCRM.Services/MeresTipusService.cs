using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresTipusService : IMeresTipusService
{
    private readonly CrmDbContext _context;

    public MeresTipusService(CrmDbContext context) => _context = context;

    public async Task<List<MeresTipus>> GetAllAsync() =>
        await _context.MeresTipusok
            .AsNoTracking()
            .OrderBy(m => m.Nev)
            .ToListAsync();

    public async Task<MeresTipus?> GetByIdAsync(int id) =>
        await _context.MeresTipusok
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<MeresTipus> CreateAsync(MeresTipus meresTipus)
    {
        meresTipus.Letrehozva = DateTime.UtcNow;
        _context.MeresTipusok.Add(meresTipus);
        await _context.SaveChangesAsync();
        return meresTipus;
    }

    public async Task<MeresTipus> UpdateAsync(MeresTipus meresTipus)
    {
        var existing = await _context.MeresTipusok.FindAsync(meresTipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = meresTipus.Nev;
        existing.Leiras = meresTipus.Leiras;
        existing.ErvenyessegHonap = meresTipus.ErvenyessegHonap;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var meresTipus = await _context.MeresTipusok.FindAsync(id);
        if (meresTipus is not null)
        {
            _context.MeresTipusok.Remove(meresTipus);
            await _context.SaveChangesAsync();
        }
    }
}