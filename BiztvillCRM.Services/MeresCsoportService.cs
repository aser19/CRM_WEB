using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresCsoportService : IMeresCsoportService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public MeresCsoportService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<MeresCsoport>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresCsoportok
            .Include(c => c.FoMeresTipus)
            .Include(c => c.Tagok)
                .ThenInclude(t => t.MeresTipus)
            .Where(c => c.Aktiv)
            .OrderBy(c => c.Nev)
            .ToListAsync();
    }

    public async Task<MeresCsoport?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MeresCsoportok
            .Include(c => c.FoMeresTipus)
            .Include(c => c.Tagok)
                .ThenInclude(t => t.MeresTipus)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateAsync(MeresCsoport csoport)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        csoport.Letrehozva = DateTime.UtcNow;
        context.MeresCsoportok.Add(csoport);
        await context.SaveChangesAsync();
        return csoport.Id;
    }

    public async Task UpdateAsync(MeresCsoport csoport)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.MeresCsoportok
            .Include(c => c.Tagok)
            .FirstOrDefaultAsync(c => c.Id == csoport.Id)
            ?? throw new InvalidOperationException("Mérés csoport nem található");

        existing.Nev = csoport.Nev;
        existing.Leiras = csoport.Leiras;
        existing.FoMeresTipusId = csoport.FoMeresTipusId;
        existing.Aktiv = csoport.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        // Tagok cseréje
        context.MeresCsoportTagok.RemoveRange(existing.Tagok);
        existing.Tagok.Clear();

        foreach (var tag in csoport.Tagok)
        {
            existing.Tagok.Add(new MeresCsoportTag
            {
                MeresTipusId = tag.MeresTipusId,
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
        var csoport = await context.MeresCsoportok.FindAsync(id);
        if (csoport is not null)
        {
            csoport.Aktiv = false; // soft delete
            csoport.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}