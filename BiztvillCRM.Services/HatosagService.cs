using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HatosagService : IHatosagService
{
    private readonly CrmDbContext _context;

    public HatosagService(CrmDbContext context) => _context = context;

    public async Task<List<Hatosag>> GetAllAsync() =>
        await _context.Hatosagok.OrderBy(h => h.Nev).ToListAsync();

    public async Task<Hatosag?> GetByIdAsync(int id) =>
        await _context.Hatosagok.FindAsync(id);

    public async Task<Hatosag> CreateAsync(Hatosag hatosag)
    {
        hatosag.Letrehozva = DateTime.UtcNow;
        _context.Hatosagok.Add(hatosag);
        await _context.SaveChangesAsync();
        return hatosag;
    }

    public async Task<Hatosag> UpdateAsync(Hatosag hatosag)
    {
        hatosag.Modositva = DateTime.UtcNow;
        _context.Entry(hatosag).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return hatosag;
    }

    public async Task DeleteAsync(int id)
    {
        var hatosag = await _context.Hatosagok.FindAsync(id);
        if (hatosag is not null)
        {
            _context.Hatosagok.Remove(hatosag);
            await _context.SaveChangesAsync();
        }
    }
}