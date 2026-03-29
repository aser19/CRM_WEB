using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozService : IEszkozService
{
    private readonly CrmDbContext _context;

    public EszkozService(CrmDbContext context) => _context = context;

    public async Task<List<Eszkoz>> GetAllAsync() =>
        await _context.Eszkozok
            .Include(e => e.Gyarto)
            .Include(e => e.Ugyfel)
            .Include(e => e.Telephely)
            .OrderBy(e => e.Nev).ToListAsync();

    public async Task<Eszkoz?> GetByIdAsync(int id) =>
        await _context.Eszkozok
            .Include(e => e.Gyarto)
            .Include(e => e.Ugyfel)
            .Include(e => e.Telephely)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Eszkoz> CreateAsync(Eszkoz eszkoz)
    {
        eszkoz.Letrehozva = DateTime.UtcNow;
        _context.Eszkozok.Add(eszkoz);
        await _context.SaveChangesAsync();
        return eszkoz;
    }

    public async Task<Eszkoz> UpdateAsync(Eszkoz eszkoz)
    {
        eszkoz.Modositva = DateTime.UtcNow;
        _context.Entry(eszkoz).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return eszkoz;
    }

    public async Task DeleteAsync(int id)
    {
        var eszkoz = await _context.Eszkozok.FindAsync(id);
        if (eszkoz is not null)
        {
            _context.Eszkozok.Remove(eszkoz);
            await _context.SaveChangesAsync();
        }
    }
}