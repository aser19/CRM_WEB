using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KepzesService : IKepzesService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public KepzesService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Kepzes>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        // Admin látja az összeset
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await _context.Kepzesek
                .Include(k => k.Ceg)
                .OrderByDescending(k => k.Datum)
                .ToListAsync();
        }

        return await _context.Kepzesek
            .Where(k => k.CegId == cegId)
            .OrderByDescending(k => k.Datum)
            .ToListAsync();
    }

    public async Task<Kepzes?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Kepzesek.AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Kepzes> CreateAsync(Kepzes kepzes)
    {
        kepzes.Letrehozva = DateTime.UtcNow;
        
        // CegId beállítása
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) || kepzes.CegId == 0)
        {
            kepzes.CegId = _tenantService.GetCurrentCegId();
        }
        
        _context.Kepzesek.Add(kepzes);
        await _context.SaveChangesAsync();
        return kepzes;
    }

    public async Task<Kepzes> UpdateAsync(Kepzes kepzes)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Kepzesek.FindAsync(kepzes.Id)
            ?? throw new InvalidOperationException("Nem található.");

        // Jogosultság ellenőrzés
        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a módosításhoz.");
        }

        existing.Nev = kepzes.Nev;
        existing.Datum = kepzes.Datum;
        existing.LejaratDatum = kepzes.LejaratDatum;
        existing.Resztvevo = kepzes.Resztvevo;
        existing.Megjegyzes = kepzes.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;
        
        // Admin módosíthatja a CegId-t
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            existing.CegId = kepzes.CegId;
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var kepzes = await _context.Kepzesek.FindAsync(id);
        
        if (kepzes is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && kepzes.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a törléshez.");
            }
            
            _context.Kepzesek.Remove(kepzes);
            await _context.SaveChangesAsync();
        }
    }
}