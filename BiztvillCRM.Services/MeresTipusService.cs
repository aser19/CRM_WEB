using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresTipusService : IMeresTipusService
{
    private readonly CrmDbContext _context;

    public MeresTipusService(CrmDbContext context) => _context = context;

    public async Task<List<MeresTipus>> GetAllAsync() =>
        await _context.MeresTipusok.OrderBy(m => m.Nev).ToListAsync();

    public async Task<MeresTipus?> GetByIdAsync(int id) =>
        await _context.MeresTipusok.FindAsync(id);

    public async Task<MeresTipus> CreateAsync(MeresTipus meresTipus)
    {
        meresTipus.Letrehozva = DateTime.UtcNow;
        _context.MeresTipusok.Add(meresTipus);
        await _context.SaveChangesAsync();
        return meresTipus;
    }

    public async Task<MeresTipus> UpdateAsync(MeresTipus meresTipus)
    {
        meresTipus.Modositva = DateTime.UtcNow;
        _context.Entry(meresTipus).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return meresTipus;
    }

    public async Task DeleteAsync(int id)
    {
        var meresTipus = await _context.MeresTipusok.FindAsync(id);
        if (meresTipus is not null)
        {
            _context.MeresTipusok.Remove(meresTipus);
            await _context.SaveChangesAsync();
        }
    }
}