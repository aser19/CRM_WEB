using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresService : IMeresService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public MeresService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Meres>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Meresek
            .Include(m => m.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .Include(m => m.MeresTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(m => m.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<Meres?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Meresek
            .Include(m => m.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .Include(m => m.MeresTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(m => m.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Meres> CreateAsync(Meres meres)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await _context.Eszkozok.Include(e => e.Ugyfel).FirstOrDefaultAsync(e => e.Id == meres.EszkozId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz?.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága mérés létrehozásához ennél az eszköznél.");
        }

        meres.Letrehozva = DateTime.UtcNow;
        _context.Meresek.Add(meres);
        await _context.SaveChangesAsync();
        return meres;
    }

    public async Task<Meres> UpdateAsync(Meres meres)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Meresek
            .Include(m => m.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == meres.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Eszkoz.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a mérés módosításához.");
        }

        existing.EszkozId = meres.EszkozId;
        existing.MeresTipusId = meres.MeresTipusId;
        existing.Datum = meres.Datum;
        existing.KovetkezoDatum = meres.KovetkezoDatum;
        existing.Eredmeny = meres.Eredmeny;
        existing.MeresStatusz = meres.MeresStatusz;
        existing.Megjegyzes = meres.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var meres = await _context.Meresek
            .Include(m => m.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meres is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && meres.Eszkoz.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés törléséhez.");
            }

            _context.Meresek.Remove(meres);
            await _context.SaveChangesAsync();
        }
    }
}