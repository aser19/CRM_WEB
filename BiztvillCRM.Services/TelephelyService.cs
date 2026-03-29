using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TelephelyService : ITelephelyService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public TelephelyService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Telephely>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Telephelyek
            .Include(t => t.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(t => t.Ugyfel.CegId == cegId);
        }

        return await query.OrderBy(t => t.Nev).ToListAsync();
    }

    public async Task<Telephely?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Telephelyek
            .Include(t => t.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(t => t.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Telephely> CreateAsync(Telephely telephely)
    {
        // Ellenőrzés: az ügyfél a saját céghez tartozik-e
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(telephely.UgyfelId);
        
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága telephely létrehozásához ennél az ügyfélnél.");
        }

        telephely.Letrehozva = DateTime.UtcNow;
        _context.Telephelyek.Add(telephely);
        await _context.SaveChangesAsync();
        return telephely;
    }

    public async Task<Telephely> UpdateAsync(Telephely telephely)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Telephelyek
            .Include(t => t.Ugyfel)
            .FirstOrDefaultAsync(t => t.Id == telephely.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a telephely módosításához.");
        }

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
        var cegId = _tenantService.GetCurrentCegId();
        var telephely = await _context.Telephelyek
            .Include(t => t.Ugyfel)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (telephely is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && telephely.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a telephely törléséhez.");
            }

            _context.Telephelyek.Remove(telephely);
            await _context.SaveChangesAsync();
        }
    }
}