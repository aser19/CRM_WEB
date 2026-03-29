using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class GyartoService : IGyartoService
{
    private readonly CrmDbContext _context;

    public GyartoService(CrmDbContext context) => _context = context;

    public async Task<List<Gyarto>> GetAllAsync() =>
        await _context.Gyartok.OrderBy(g => g.Nev).ToListAsync();

    public async Task<Gyarto?> GetByIdAsync(int id) =>
        await _context.Gyartok.FindAsync(id);

    public async Task<Gyarto> CreateAsync(Gyarto gyarto)
    {
        gyarto.Letrehozva = DateTime.UtcNow;
        _context.Gyartok.Add(gyarto);
        await _context.SaveChangesAsync();
        return gyarto;
    }

    // GyartoService.UpdateAsync
    public async Task<Gyarto> UpdateAsync(Gyarto gyarto)
    {
        var existing = await _context.Gyartok.FindAsync(gyarto.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = gyarto.Nev;
        existing.Orszag = gyarto.Orszag;
        existing.Weboldal = gyarto.Weboldal;
        existing.Aktiv = gyarto.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var gyarto = await _context.Gyartok.FindAsync(id);
        if (gyarto is not null)
        {
            _context.Gyartok.Remove(gyarto);
            await _context.SaveChangesAsync();
        }
    }
}