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

    public async Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync() =>
        await _context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
                .ThenInclude(k => k.KepzesTipus)
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
        existing.SablonId = meresTipus.SablonId;
        existing.JegyzokonyvPrefix = meresTipus.JegyzokonyvPrefix;
        existing.Aktiv = meresTipus.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task UpdateWithKovetelemenyekAsync(MeresTipus tipus)
    {
        var existing = await _context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
            .FirstOrDefaultAsync(m => m.Id == tipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        // Alap adatok frissítése
        existing.Nev = tipus.Nev;
        existing.Leiras = tipus.Leiras;
        existing.SablonId = tipus.SablonId;
        existing.JegyzokonyvPrefix = tipus.JegyzokonyvPrefix;
        existing.Modositva = DateTime.UtcNow;
        
        // Régi követelmények törlése
        _context.MeresTipusKepzesKovetelemenyei.RemoveRange(existing.KepzesKovetelemenyei);
        
        // Új követelmények hozzáadása
        foreach (var kov in tipus.KepzesKovetelemenyei)
        {
            _context.MeresTipusKepzesKovetelemenyei.Add(new MeresTipusKepzesKovetelemeny
            {
                MeresTipusId = tipus.Id,
                KepzesTipusId = kov.KepzesTipusId,
                SablonLabel = kov.SablonLabel,
                AlternativaCsoport = kov.AlternativaCsoport,
                Prioritas = kov.Prioritas,
                Kotelezo = true
            });
        }
        
        await _context.SaveChangesAsync();
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