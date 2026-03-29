using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

/// <summary>
/// Ügyfél CRUD műveletek implementációja EF Core segítségével.
/// Multi-tenant: csak a bejelentkezett felhasználó cégéhez tartozó ügyfeleket kezeli.
/// </summary>
public class UgyfelService : IUgyfelService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public UgyfelService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    /// <inheritdoc/>
    public async Task<List<Ugyfel>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        // Admin látja az összeset
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await _context.Ugyfelek
                .Include(u => u.Telephelyek)
                .Include(u => u.Ceg)
                .OrderBy(u => u.Nev)
                .ToListAsync();
        }

        return await _context.Ugyfelek
            .Where(u => u.CegId == cegId)
            .Include(u => u.Telephelyek)
            .OrderBy(u => u.Nev)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Ugyfel?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Ugyfelek
            .Include(u => u.Telephelyek)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(u => u.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Ugyfel> CreateAsync(Ugyfel ugyfel)
    {
        ugyfel.Letrehozva = DateTime.UtcNow;
        ugyfel.CegId = _tenantService.GetCurrentCegId();
        _context.Ugyfelek.Add(ugyfel);
        await _context.SaveChangesAsync();
        return ugyfel;
    }

    /// <inheritdoc/>
    public async Task<Ugyfel> UpdateAsync(Ugyfel ugyfel)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Ugyfelek.FindAsync(ugyfel.Id)
            ?? throw new InvalidOperationException("Nem található.");

        // Ellenőrzés: csak saját cég ügyfele módosítható
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél módosításához.");
        }

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
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(id);
        
        if (ugyfel is not null)
        {
            // Ellenőrzés: csak saját cég ügyfele törölhető
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyfél törléséhez.");
            }

            _context.Ugyfelek.Remove(ugyfel);
            await _context.SaveChangesAsync();
        }
    }
}