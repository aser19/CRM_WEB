using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class GyartoService : IGyartoService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public GyartoService(IDbContextFactory<CrmDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<Gyarto>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Gyartok.OrderBy(g => g.Nev).ToListAsync();
    }

    public async Task<Gyarto?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Gyartok.FindAsync(id);
    }

    public async Task<Gyarto> CreateAsync(Gyarto gyarto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        gyarto.Letrehozva = DateTime.UtcNow;
        context.Gyartok.Add(gyarto);
        await context.SaveChangesAsync();
        return gyarto;
    }

    public async Task<Gyarto> UpdateAsync(Gyarto gyarto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.Gyartok.FindAsync(gyarto.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = gyarto.Nev;
        existing.Orszag = gyarto.Orszag;
        existing.Weboldal = gyarto.Weboldal;
        existing.Aktiv = gyarto.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
 
  {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var gyarto = await context.Gyartok.FindAsync(id);
        if (gyarto is not null)
        {
            context.Gyartok.Remove(gyarto);
            await context.SaveChangesAsync();
        }
    }
}