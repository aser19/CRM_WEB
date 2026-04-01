using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class CegService : ICegService
{
    private readonly CrmDbContext _context;

    public CegService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ceg>> GetAllAsync(bool csakAktiv = true)
    {
        var query = _context.Cegek.AsQueryable();
        
        if (csakAktiv)
            query = query.Where(c => c.Aktiv);
            
        return await query.OrderBy(c => c.Nev).ToListAsync();
    }

    public async Task<Ceg?> GetByIdAsync(int id)
    {
        return await _context.Cegek.FindAsync(id);
    }

    public async Task<Ceg> CreateAsync(Ceg ceg)
    {
        ceg.Letrehozva = DateTime.Now;
        ceg.Aktiv = true;
        
        _context.Cegek.Add(ceg);
        await _context.SaveChangesAsync();
        
        return ceg;
    }

    public async Task<Ceg> UpdateAsync(Ceg ceg)
    {
        var existing = await _context.Cegek.FindAsync(ceg.Id);
        if (existing == null)
            throw new InvalidOperationException("Cég nem található.");

        existing.Nev = ceg.Nev;
        existing.Adoszam = ceg.Adoszam;
        existing.Cim = ceg.Cim;
        existing.Email = ceg.Email;
        existing.Telefon = ceg.Telefon;
        existing.Weboldal = ceg.Weboldal;
        existing.Modositva = DateTime.Now;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> SetAktivAsync(int id, bool aktiv)
    {
        var ceg = await _context.Cegek.FindAsync(id);
        if (ceg == null)
            return false;

        ceg.Aktiv = aktiv;
        ceg.Modositva = DateTime.Now;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<int> GetFelhasznalokSzamaAsync(int cegId)
    {
        return await _context.Users.CountAsync(f => f.CegId == cegId);
    }

    public async Task<int> GetUgyfelekSzamaAsync(int cegId)
    {
        return await _context.Ugyfelek.CountAsync(u => u.CegId == cegId);
    }
}