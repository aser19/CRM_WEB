using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresTipusService : IMeresTipusService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public MeresTipusService(IDbContextFactory<CrmDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<MeresTipus>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusok
            .AsNoTracking()
            .OrderBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
                .ThenInclude(k => k.KepzesTipus)
            .AsNoTracking()
            .OrderBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<MeresTipus?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusok
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MeresTipus> CreateAsync(MeresTipus meresTipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        meresTipus.Letrehozva = DateTime.UtcNow;
        context.MeresTipusok.Add(meresTipus);
        await context.SaveChangesAsync();
        return meresTipus;
    }

    public async Task<MeresTipus> UpdateAsync(MeresTipus meresTipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.MeresTipusok.FindAsync(meresTipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = meresTipus.Nev;
        existing.Leiras = meresTipus.Leiras;
        existing.ErvenyessegHonap = meresTipus.ErvenyessegHonap;
        existing.SablonId = meresTipus.SablonId;
        existing.JegyzokonyvPrefix = meresTipus.JegyzokonyvPrefix;
        existing.Aktiv = meresTipus.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task UpdateWithKovetelemenyekAsync(MeresTipus tipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
            .FirstOrDefaultAsync(m => m.Id == tipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = tipus.Nev;
        existing.Leiras = tipus.Leiras;
        existing.SablonId = tipus.SablonId;
        existing.JegyzokonyvPrefix = tipus.JegyzokonyvPrefix;
        existing.Modositva = DateTime.UtcNow;
        
        context.MeresTipusKepzesKovetelemenyei.RemoveRange(existing.KepzesKovetelemenyei);
        
        foreach (var kov in tipus.KepzesKovetelemenyei)
        {
            context.MeresTipusKepzesKovetelemenyei.Add(new MeresTipusKepzesKovetelemeny
            {
                MeresTipusId = tipus.Id,
                KepzesTipusId = kov.KepzesTipusId,
                SablonLabel = kov.SablonLabel,
                AlternativaCsoport = kov.AlternativaCsoport,
                Prioritas = kov.Prioritas,
                Kotelezo = true
            });
        }
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var meresTipus = await context.MeresTipusok.FindAsync(id);
        if (meresTipus is not null)
        {
            context.MeresTipusok.Remove(meresTipus);
            await context.SaveChangesAsync();
        }
    }
}