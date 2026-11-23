using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class UgyfelCsoportService : IUgyfelCsoportService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public UgyfelCsoportService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UgyfelCsoport>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UgyfelCsoportok
            .OrderBy(c => c.Nev)
            .ToListAsync();
    }

    public async Task<UgyfelCsoport> CreateAsync(string nev)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var csoport = new UgyfelCsoport { Nev = nev.Trim(), Letrehozva = DateTime.UtcNow };
        context.UgyfelCsoportok.Add(csoport);
        await context.SaveChangesAsync();
        return csoport;
    }

    public async Task LevalasztasAsync(int ugyfelId)
    {
        // Csak a hozzárendelést szünteti meg (UgyfelCsoportId = null), az ügyfél és a mérései
        // teljesen érintetlenek maradnak - így bármikor, kockázatmentesen visszavonható/
        // újra szétválasztható, pl. ha a cégek a jövőben külön ügyintézőhöz kerülnek.
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ugyfel = await context.Ugyfelek.FindAsync(ugyfelId);
        if (ugyfel is not null)
        {
            ugyfel.UgyfelCsoportId = null;
            ugyfel.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}
