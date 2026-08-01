using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class RbVedelmiModService : IRbVedelmiModService
{
    private readonly CrmDbContext _context;

    public RbVedelmiModService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<RbVedelmiMod>> GetAllAsync()
    {
        return await _context.RbVedelmiModok
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<List<RbVedelmiMod>> GetAllActiveAsync()
    {
        return await _context.RbVedelmiModok
            .Where(t => t.Aktiv)
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<RbVedelmiMod?> GetByIdAsync(int id)
    {
        return await _context.RbVedelmiModok.FindAsync(id);
    }

    public async Task<RbVedelmiMod?> GetByNevAsync(string nev)
    {
        return await _context.RbVedelmiModok
            .FirstOrDefaultAsync(t => t.Nev == nev);
    }

    public async Task<RbVedelmiMod> GetOrCreateAsync(string nev)
    {
        if (string.IsNullOrWhiteSpace(nev))
        {
            throw new ArgumentException("A védelmi mód neve nem lehet üres!", nameof(nev));
        }

        var meglevo = await GetByNevAsync(nev);
        if (meglevo != null)
        {
            return meglevo;
        }

        var ujTipus = new RbVedelmiMod
        {
            Nev = nev,
            Aktiv = true,
            FelulvizsgalasraVar = true,
            Leiras = "Automatikusan létrehozott védelmi mód - felülvizsgálatra vár",
            Letrehozva = DateTime.Now
        };

        _context.RbVedelmiModok.Add(ujTipus);
        await _context.SaveChangesAsync();

        return ujTipus;
    }

    public async Task<int> CreateAsync(RbVedelmiMod tipus)
    {
        if (await _context.RbVedelmiModok.AnyAsync(t => t.Nev == tipus.Nev))
        {
            throw new InvalidOperationException($"A '{tipus.Nev}' védelmi mód már létezik!");
        }

        tipus.Letrehozva = DateTime.Now;
        _context.RbVedelmiModok.Add(tipus);
        await _context.SaveChangesAsync();
        return tipus.Id;
    }

    public async Task UpdateAsync(RbVedelmiMod tipus)
    {
        if (await _context.RbVedelmiModok
            .AnyAsync(t => t.Nev == tipus.Nev && t.Id != tipus.Id))
        {
            throw new InvalidOperationException($"A '{tipus.Nev}' védelmi mód már létezik!");
        }

        _context.RbVedelmiModok.Update(tipus);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tipus = await _context.RbVedelmiModok.FindAsync(id);
        if (tipus != null)
        {
            _context.RbVedelmiModok.Remove(tipus);
            await _context.SaveChangesAsync();
        }
    }
}
