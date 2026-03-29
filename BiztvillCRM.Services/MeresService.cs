using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresService : IMeresService
{
    private readonly CrmDbContext _context;

    public MeresService(CrmDbContext context) => _context = context;

    public async Task<List<Meres>> GetAllAsync() =>
        await _context.Meresek
            .Include(m => m.Eszkoz)
            .Include(m => m.MeresTipus)
            .OrderByDescending(m => m.Datum).ToListAsync();

    public async Task<Meres?> GetByIdAsync(int id) =>
        await _context.Meresek
            .Include(m => m.Eszkoz)
            .Include(m => m.MeresTipus)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Meres> CreateAsync(Meres meres)
    {
        meres.Letrehozva = DateTime.UtcNow;
        _context.Meresek.Add(meres);
        await _context.SaveChangesAsync();
        return meres;
    }

    public async Task<Meres> UpdateAsync(Meres meres)
    {
        var existing = await _context.Meresek.FindAsync(meres.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.EszkozId = meres.EszkozId;
        existing.MeresTipusId = meres.MeresTipusId;
        existing.Datum = meres.Datum;
        existing.KovetkezoDatum = meres.KovetkezoDatum;
        existing.Eredmeny = meres.Eredmeny;
        existing.MeresStatusz = meres.MeresStatusz;
        existing.Megjegyzes = meres.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var meres = await _context.Meresek.FindAsync(id);
        if (meres is not null)
        {
            _context.Meresek.Remove(meres);
            await _context.SaveChangesAsync();
        }
    }
}