using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TanusitvanyService : ITanusitvanyService
{
    private readonly CrmDbContext _context;

    public TanusitvanyService(CrmDbContext context) => _context = context;

    public async Task<List<Tanusitvany>> GetAllAsync() =>
        await _context.Tanusitvanyok.Include(t => t.Ugyfel).OrderBy(t => t.Nev).ToListAsync();

    public async Task<Tanusitvany?> GetByIdAsync(int id) =>
        await _context.Tanusitvanyok.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tanusitvany> CreateAsync(Tanusitvany tanusitvany)
    {
        tanusitvany.Letrehozva = DateTime.UtcNow;
        _context.Tanusitvanyok.Add(tanusitvany);
        await _context.SaveChangesAsync();
        return tanusitvany;
    }

    public async Task<Tanusitvany> UpdateAsync(Tanusitvany tanusitvany)
    {
        tanusitvany.Modositva = DateTime.UtcNow;
        _context.Entry(tanusitvany).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return tanusitvany;
    }

    public async Task DeleteAsync(int id)
    {
        var tanusitvany = await _context.Tanusitvanyok.FindAsync(id);
        if (tanusitvany is not null)
        {
            _context.Tanusitvanyok.Remove(tanusitvany);
            await _context.SaveChangesAsync();
        }
    }
}