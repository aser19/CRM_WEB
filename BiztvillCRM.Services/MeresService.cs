using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MeresService : IMeresService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public MeresService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Meres>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(m => m.Ugyfel!.CegId == cegId);
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<Meres?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(m => m.Ugyfel!.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Meres> CreateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await context.Ugyfelek.FirstOrDefaultAsync(u => u.Id == meres.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága mérés létrehozásához ennél az ügyfélnél.");
        }

        var telephely = await context.Telephelyek.FirstOrDefaultAsync(t => t.Id == meres.TelephelyId);
        if (telephely?.UgyfelId != meres.UgyfelId)
        {
            throw new InvalidOperationException("A telephely nem tartozik a kiválasztott ügyfélhez.");
        }

        meres.Ugyfel = null!;
        meres.Telephely = null!;
        meres.MeresTipus = null!;
        meres.Letrehozva = DateTime.UtcNow;
        
        context.Meresek.Add(meres);
        await context.SaveChangesAsync();
        return meres;
    }

    public async Task<Meres> UpdateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == meres.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Ugyfel!.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a mérés módosításához.");
        }

        existing.UgyfelId = meres.UgyfelId;
        existing.TelephelyId = meres.TelephelyId;
        existing.MeresTipusId = meres.MeresTipusId;
        existing.Datum = meres.Datum;
        existing.KovetkezoDatum = meres.KovetkezoDatum;
        existing.Eredmeny = meres.Eredmeny;           // <-- JegyzokonyvSzam helyett
        existing.MeresStatusz = meres.MeresStatusz;   // <-- Ez is hiányzott
        existing.Megjegyzes = meres.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var meres = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meres is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && meres.Ugyfel!.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés törléséhez.");
            }

            context.Meresek.Remove(meres);
            await context.SaveChangesAsync();
        }
    }
}