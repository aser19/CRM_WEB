using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TelephelyService : ITelephelyService
{
    private readonly CrmDbContext _context;

    public TelephelyService(CrmDbContext context) => _context = context;

    public async Task<List<Telephely>> GetAllAsync() =>
        await _context.Telephelyek.Include(t => t.Ugyfel).OrderBy(t => t.Nev).ToListAsync();

    public async Task<Telephely?> GetByIdAsync(int id) =>
        await _context.Telephelyek.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Telephely> CreateAsync(Telephely telephely)
    {
        telephely.Letrehozva = DateTime.UtcNow;
        _context.Telephelyek.Add(telephely);
        await _context.SaveChangesAsync();
        return telephely;
    }

    public async Task<Telephely> UpdateAsync(Telephely telephely)
    {
        var existing = await _context.Telephelyek.FindAsync(telephely.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = telephely.Nev;
        existing.Cim = telephely.Cim;
        existing.UgyfelId = telephely.UgyfelId;
        existing.Kapcsolattarto = telephely.Kapcsolattarto;
        existing.Telefon = telephely.Telefon;
        existing.Email = telephely.Email;
        existing.Aktiv = telephely.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var telephely = await _context.Telephelyek.FindAsync(id);
        if (telephely is not null)
        {
            _context.Telephelyek.Remove(telephely);
            await _context.SaveChangesAsync();
        }
    }
}