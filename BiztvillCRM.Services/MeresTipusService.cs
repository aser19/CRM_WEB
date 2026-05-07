using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresTipusService : IMeresTipusService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public MeresTipusService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<MeresTipus>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusok
            .Where(m => m.Aktiv)
            .OrderBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<MeresTipus?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusok.FindAsync(id);
    }

    public async Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
                .ThenInclude(k => k.KepzesTipus)
            .OrderBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(MeresTipus tipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        tipus.Letrehozva = DateTime.UtcNow;
        context.MeresTipusok.Add(tipus);
        await context.SaveChangesAsync();
        
        return tipus.Id;
    }

    public async Task UpdateAsync(MeresTipus tipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existing = await context.MeresTipusok.FindAsync(tipus.Id)
            ?? throw new InvalidOperationException("Mérés típus nem található");

        existing.Nev = tipus.Nev;
        existing.Leiras = tipus.Leiras;
        existing.JegyzokonyvPrefix = tipus.JegyzokonyvPrefix;
        existing.SablonId = tipus.SablonId;
        existing.OcrModelId = tipus.OcrModelId;
        existing.MellekletTipusKod = tipus.MellekletTipusKod; // ← EZT ADD HOZZÁ
        existing.Aktiv = tipus.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task UpdateWithKovetelemenyekAsync(MeresTipus tipus)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existing = await context.MeresTipusok
            .Include(m => m.KepzesKovetelemenyei)
            .FirstOrDefaultAsync(m => m.Id == tipus.Id)
            ?? throw new InvalidOperationException("Mérés típus nem található");

        existing.Nev = tipus.Nev;
        existing.Leiras = tipus.Leiras;
        existing.JegyzokonyvPrefix = tipus.JegyzokonyvPrefix;
        existing.SablonId = tipus.SablonId;
        existing.OcrModelId = tipus.OcrModelId;
        existing.MellekletTipusKod = tipus.MellekletTipusKod;
        existing.Aktiv = tipus.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        context.MeresTipusKepzesKovetelemenyei.RemoveRange(existing.KepzesKovetelemenyei);
        existing.KepzesKovetelemenyei.Clear();

        foreach (var kov in tipus.KepzesKovetelemenyei)
        {
            existing.KepzesKovetelemenyei.Add(new MeresTipusKepzesKovetelemeny
            {
                MeresTipusId = existing.Id,
                KepzesTipusId = kov.KepzesTipusId,
                SablonLabel = kov.SablonLabel,
                AlternativaCsoport = kov.AlternativaCsoport,
                Prioritas = kov.Prioritas
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var tipus = await context.MeresTipusok.FindAsync(id);
        if (tipus != null)
        {
            context.MeresTipusok.Remove(tipus);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<MeresTipusJogszabaly>> GetJogszabalyokByTipusIdAsync(int meresTipusId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresTipusJogszabalyok
            .Include(x => x.Jogszabaly)
            .Where(x => x.MeresTipusId == meresTipusId)
            .OrderBy(x => x.Sorrend)
            .ToListAsync();
    }

    public async Task MentJogszabalyHozzarendelesekAsync(int meresTipusId, List<int> jogszabalyIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meglevo = context.MeresTipusJogszabalyok
            .Where(x => x.MeresTipusId == meresTipusId);
        context.MeresTipusJogszabalyok.RemoveRange(meglevo);

        var ujak = jogszabalyIds.Select((id, i) => new MeresTipusJogszabaly
        {
            MeresTipusId = meresTipusId,
            JogszabalyId = id,
            Sorrend = i
        });
        await context.MeresTipusJogszabalyok.AddRangeAsync(ujak);
        await context.SaveChangesAsync();
    }
}