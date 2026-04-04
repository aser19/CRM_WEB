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
        
        return await _context.Kepzesek
            .Include(k => k.KepzesTipus)
            .Where(k => k.CegId == cegId && k.Aktiv)
            .OrderBy(k => k.Nev)
            .ToListAsync();
    }

    public async Task<List<Kepzes>> GetAllAdminAsync()
    {
        return await _context.Kepzesek
            .Include(k => k.Ceg)
            .Include(k => k.KepzesTipus)
            .OrderBy(k => k.Nev)
            .ToListAsync();
    }

    public async Task<Kepzes?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Kepzesek
            .Include(k => k.KepzesTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Kepzes> CreateAsync(Kepzes kepzes)
    {
        kepzes.Letrehozva = DateTime.Now;
        
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

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.CegId != cegId)
            throw new UnauthorizedAccessException("Nincs jogosultsága.");

        existing.Nev = kepzes.Nev;
        existing.KepzesTipusId = kepzes.KepzesTipusId;
        existing.BizonyitvanySzam = kepzes.BizonyitvanySzam;
        existing.BizonyitvanyKelte = kepzes.BizonyitvanyKelte;
        existing.TovabbkepzesSzam = kepzes.TovabbkepzesSzam;
        existing.UtolsoTovabbkepzes = kepzes.UtolsoTovabbkepzes;
        existing.Jogosultsag = kepzes.Jogosultsag;
        existing.Megjegyzes = kepzes.Megjegyzes;
        existing.Aktiv = kepzes.Aktiv;
        existing.Modositva = DateTime.Now;
        
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            existing.CegId = kepzes.CegId;

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