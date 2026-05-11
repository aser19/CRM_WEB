using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HitelesitesCsoportService : IHitelesitesCsoportService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public HitelesitesCsoportService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<HitelesitesCsoport>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.HitelesitesCsoportok
            .Include(c => c.FoEszkozTipus)
            .Include(c => c.Tagok)
                .ThenInclude(t => t.EszkozTipus)
            .Where(c => c.Aktiv)
            .OrderBy(c => c.Nev)
            .ToListAsync();
    }

    public async Task<HitelesitesCsoport?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.HitelesitesCsoportok
            .Include(c => c.FoEszkozTipus)
            .Include(c => c.Tagok)
                .ThenInclude(t => t.EszkozTipus)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateAsync(HitelesitesCsoport csoport)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        csoport.Letrehozva = DateTime.UtcNow;
        context.HitelesitesCsoportok.Add(csoport);
        await context.SaveChangesAsync();
        return csoport.Id;
    }

    public async Task UpdateAsync(HitelesitesCsoport csoport)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.HitelesitesCsoportok
            .Include(c => c.Tagok)
            .FirstOrDefaultAsync(c => c.Id == csoport.Id)
            ?? throw new InvalidOperationException("Hitelesítés csoport nem található");

        existing.Nev = csoport.Nev;
        existing.Leiras = csoport.Leiras;
        existing.FoEszkozTipusId = csoport.FoEszkozTipusId;
        existing.Aktiv = csoport.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        context.HitelesitesCsoportTagok.RemoveRange(existing.Tagok);
        existing.Tagok.Clear();

        foreach (var tag in csoport.Tagok)
        {
            existing.Tagok.Add(new HitelesitesCsoportTag
            {
                EszkozTipusId = tag.EszkozTipusId,
                Kotelezo = tag.Kotelezo,
                Sorrend = tag.Sorrend,
                Megjegyzes = tag.Megjegyzes
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var csoport = await context.HitelesitesCsoportok.FindAsync(id);
        if (csoport is not null)
        {
            csoport.Aktiv = false;
            csoport.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}