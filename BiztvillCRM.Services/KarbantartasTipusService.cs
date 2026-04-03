using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KarbantartasTipusService : IKarbantartasTipusService
{
    private readonly CrmDbContext _context;

    public KarbantartasTipusService(CrmDbContext context) => _context = context;

    public async Task<List<KarbantartasTipus>> GetAllAsync() =>
        await _context.KarbantartasTipusok.OrderBy(t => t.Nev).ToListAsync();

    public async Task<List<KarbantartasTipus>> GetActiveAsync() =>
        await _context.KarbantartasTipusok.Where(t => t.Aktiv).OrderBy(t => t.Nev).ToListAsync();

    public async Task<KarbantartasTipus?> GetByIdAsync(int id) =>
        await _context.KarbantartasTipusok.FindAsync(id);

    public async Task<KarbantartasTipus> CreateAsync(KarbantartasTipus tipus)
    {
        tipus.Letrehozva = DateTime.UtcNow;
        _context.KarbantartasTipusok.Add(tipus);
        await _context.SaveChangesAsync();
        return tipus;
    }

    public async Task<KarbantartasTipus> UpdateAsync(KarbantartasTipus tipus)
    {
        var existing = await _context.KarbantartasTipusok.FindAsync(tipus.Id)
            ?? throw new InvalidOperationException("Nem található.");
        
        existing.Nev = tipus.Nev;
        existing.Leiras = tipus.Leiras;
        existing.IsmetlodesHonap = tipus.IsmetlodesHonap;
        existing.Aktiv = tipus.Aktiv;
        existing.Modositva = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var tipus = await _context.KarbantartasTipusok.FindAsync(id);
        if (tipus is not null)
        {
            _context.KarbantartasTipusok.Remove(tipus);
            await _context.SaveChangesAsync();
        }
    }
}