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

    public async Task<Gyarto> UpdateAsync(Gyarto gyarto)
    {
        gyarto.Modositva = DateTime.UtcNow;
        _context.Entry(gyarto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return gyarto;
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