using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class UgyszamService : IUgyszamService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public UgyszamService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Ugyszam>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Ugyszamok
            .Include(u => u.Ugyfel)
            .Include(u => u.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(u => u.Ugyfel.CegId == cegId);
        }

        return await query.OrderByDescending(u => u.Beerkezett).ToListAsync();
    }

    public async Task<Ugyszam?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Ugyszamok
            .Include(u => u.Ugyfel)
            .Include(u => u.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(u => u.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Ugyszam> CreateAsync(Ugyszam ugyszam)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(ugyszam.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága ügyszám létrehozásához ennél az ügyfélnél.");
        }

        ugyszam.Letrehozva = DateTime.UtcNow;
        _context.Ugyszamok.Add(ugyszam);
        await _context.SaveChangesAsync();
        return ugyszam;
    }

    public async Task<Ugyszam> UpdateAsync(Ugyszam ugyszam)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Ugyszamok
            .Include(u => u.Ugyfel)
            .FirstOrDefaultAsync(u => u.Id == ugyszam.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága az ügyszám módosításához.");
        }

        existing.Szam = ugyszam.Szam;
        existing.Targy = ugyszam.Targy;
        existing.UgyfelId = ugyszam.UgyfelId;
        existing.HatosagId = ugyszam.HatosagId;
        existing.Beerkezett = ugyszam.Beerkezett;
        existing.Hatarido = ugyszam.Hatarido;
        existing.Lezart = ugyszam.Lezart;
        existing.Megjegyzes = ugyszam.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var ugyszam = await _context.Ugyszamok
            .Include(u => u.Ugyfel)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (ugyszam is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyszam.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága az ügyszám törléséhez.");
            }

            _context.Ugyszamok.Remove(ugyszam);
            await _context.SaveChangesAsync();
        }
    }
}