using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class HitelesitesService : IHitelesitesService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public HitelesitesService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Hitelesites>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Hitelesitesek
            .AsNoTracking()
            .Include(h => h.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .Include(h => h.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(h => h.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.OrderByDescending(h => h.Datum).ToListAsync();
    }

    public async Task<Hitelesites?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Hitelesitesek
            .AsNoTracking()
            .Include(h => h.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .Include(h => h.Hatosag)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(h => h.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hitelesites> CreateAsync(Hitelesites hitelesites)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await _context.Eszkozok.Include(e => e.Ugyfel).FirstOrDefaultAsync(e => e.Id == hitelesites.EszkozId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz?.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága hitelesítés létrehozásához ennél az eszköznél.");
        }

        hitelesites.Letrehozva = DateTime.UtcNow;
        _context.Hitelesitesek.Add(hitelesites);
        await _context.SaveChangesAsync();
        return hitelesites;
    }

    public async Task<Hitelesites> UpdateAsync(Hitelesites hitelesites)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Hitelesitesek
            .Include(h => h.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(h => h.Id == hitelesites.Id)
            ?? throw new InvalidOperationException("Nem található a hitelesítés.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Eszkoz.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a hitelesítés módosításához.");
        }

        existing.EszkozId = hitelesites.EszkozId;
        existing.HatosagId = hitelesites.HatosagId;
        existing.Ugyszam = hitelesites.Ugyszam;
        existing.Datum = hitelesites.Datum;
        existing.LejaratDatum = hitelesites.LejaratDatum;
        existing.HitelesitesStatusz = hitelesites.HitelesitesStatusz;
        existing.Megjegyzes = hitelesites.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var hitelesites = await _context.Hitelesitesek
            .Include(h => h.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hitelesites is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && hitelesites.Eszkoz.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a hitelesítés törléséhez.");
            }

            _context.Hitelesitesek.Remove(hitelesites);
            await _context.SaveChangesAsync();
        }
    }
}