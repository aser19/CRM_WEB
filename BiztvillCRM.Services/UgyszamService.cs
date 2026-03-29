using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class UgyszamService : IUgyszamService
{
    private readonly CrmDbContext _context;
    public UgyszamService(CrmDbContext context) => _context = context;

    public async Task<List<Ugyszam>> GetAllAsync() =>
        await _context.Ugyszamok.Include(u => u.Ugyfel).Include(u => u.Hatosag).OrderByDescending(u => u.Beerkezett).ToListAsync();

    public async Task<Ugyszam?> GetByIdAsync(int id) =>
        await _context.Ugyszamok.Include(u => u.Ugyfel).Include(u => u.Hatosag).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Ugyszam> CreateAsync(Ugyszam ugyszam)
    {
        ugyszam.Letrehozva = DateTime.UtcNow;
        _context.Ugyszamok.Add(ugyszam);
        await _context.SaveChangesAsync();
        return ugyszam;
    }

    public async Task<Ugyszam> UpdateAsync(Ugyszam ugyszam)
    {
        ugyszam.Modositva = DateTime.UtcNow;
        _context.Entry(ugyszam).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return ugyszam;
    }

    public async Task DeleteAsync(int id)
    {
        var ugyszam = await _context.Ugyszamok.FindAsync(id);
        if (ugyszam is not null)
        {
            _context.Ugyszamok.Remove(ugyszam);
            await _context.SaveChangesAsync();
        }
    }
}