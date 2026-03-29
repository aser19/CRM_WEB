using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

/// <summary>
/// Ügyfél CRUD műveletek implementációja EF Core segítségével.
/// </summary>
public class UgyfelService : IUgyfelService
{
    private readonly CrmDbContext _context;

    public UgyfelService(CrmDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<Ugyfel>> GetAllAsync()
    {
        return await _context.Ugyfelek
            .Include(u => u.Telephelyek)
            .OrderBy(u => u.Nev)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Ugyfel?> GetByIdAsync(int id)
    {
        return await _context.Ugyfelek
            .Include(u => u.Telephelyek)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Ugyfel> CreateAsync(Ugyfel ugyfel)
    {
        ugyfel.Letrehozva = DateTime.UtcNow;
        _context.Ugyfelek.Add(ugyfel);
        await _context.SaveChangesAsync();
        return ugyfel;
    }

    /// <inheritdoc/>
    // UgyfelService.UpdateAsync
    public async Task<Ugyfel> UpdateAsync(Ugyfel ugyfel)
    {
        var existing = await _context.Ugyfelek.FindAsync(ugyfel.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = ugyfel.Nev;
        existing.Adoszam = ugyfel.Adoszam;
        existing.Cim = ugyfel.Cim;
        existing.Email = ugyfel.Email;
        existing.Telefon = ugyfel.Telefon;
        existing.UgyfelTipus = ugyfel.UgyfelTipus;
        existing.Aktiv = ugyfel.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }


    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        if (ugyfel is not null)
        {
            _context.Ugyfelek.Remove(ugyfel);
            await _context.SaveChangesAsync();
        }
    }
}
