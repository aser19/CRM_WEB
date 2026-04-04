using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KepzesTipusService : IKepzesTipusService
{
    private readonly CrmDbContext _context;

    public KepzesTipusService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<KepzesTipus>> GetAllAsync()
    {
        return await _context.KepzesTipusok
            .OrderBy(k => k.Nev)
            .ToListAsync();
    }

    public async Task<List<KepzesTipus>> GetAllActiveAsync()
    {
        return await _context.KepzesTipusok
            .Where(k => k.Aktiv)
            .OrderBy(k => k.Nev)
            .ToListAsync();
    }

    public async Task<KepzesTipus?> GetByIdAsync(int id)
    {
        return await _context.KepzesTipusok.FindAsync(id);
    }

    public async Task<KepzesTipus> CreateAsync(KepzesTipus tipus)
    {
        _context.KepzesTipusok.Add(tipus);
        await _context.SaveChangesAsync();
        return tipus;
    }

    public async Task<KepzesTipus> UpdateAsync(KepzesTipus tipus)
    {
        var existing = await _context.KepzesTipusok.FindAsync(tipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = tipus.Nev;
        existing.Lejar = tipus.Lejar;
        existing.LejaratEvek = tipus.Lejar ? tipus.LejaratEvek : null;
        existing.TovabbkepzesKotelezo = tipus.TovabbkepzesKotelezo;
        existing.TovabbkepzesEvek = tipus.TovabbkepzesKotelezo ? tipus.TovabbkepzesEvek : null;
        existing.TovabbkepzesCsakFelulvizsgalonak = tipus.TovabbkepzesKotelezo && tipus.TovabbkepzesCsakFelulvizsgalonak;
        existing.Leiras = tipus.Leiras;
        existing.Aktiv = tipus.Aktiv;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var tipus = await _context.KepzesTipusok.FindAsync(id);
        if (tipus is not null)
        {
            _context.KepzesTipusok.Remove(tipus);
            await _context.SaveChangesAsync();
        }
    }
}