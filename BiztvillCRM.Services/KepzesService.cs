using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KepzesService : IKepzesService
{
    private readonly CrmDbContext _context;

    public KepzesService(CrmDbContext context) => _context = context;

    public async Task<List<Kepzes>> GetAllAsync() =>
        await _context.Kepzesek.OrderByDescending(k => k.Datum).ToListAsync();

    public async Task<Kepzes?> GetByIdAsync(int id) =>
        await _context.Kepzesek.FindAsync(id);

    public async Task<Kepzes> CreateAsync(Kepzes kepzes)
    {
        kepzes.Letrehozva = DateTime.UtcNow;
        _context.Kepzesek.Add(kepzes);
        await _context.SaveChangesAsync();
        return kepzes;
    }

    public async Task<Kepzes> UpdateAsync(Kepzes kepzes)
    {
        kepzes.Modositva = DateTime.UtcNow;
        _context.Entry(kepzes).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return kepzes;
    }

    public async Task DeleteAsync(int id)
    {
        var kepzes = await _context.Kepzesek.FindAsync(id);
        if (kepzes is not null)
        {
            _context.Kepzesek.Remove(kepzes);
            await _context.SaveChangesAsync();
        }
    }
}