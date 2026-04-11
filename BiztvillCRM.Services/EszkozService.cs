using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozService : IEszkozService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public EszkozService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Eszkoz>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var query = context.Eszkozok
            .Include(e => e.Gyarto)
            .Include(e => e.Ugyfel)
            .Include(e => e.Telephely)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(e => e.Ugyfel.CegId == cegId);
        }

        return await query.OrderBy(e => e.Nev).ToListAsync();
    }

    public async Task<Eszkoz?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var query = context.Eszkozok
            .Include(e => e.Gyarto)
            .Include(e => e.Ugyfel)
            .Include(e => e.Telephely)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(e => e.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Eszkoz> CreateAsync(Eszkoz eszkoz)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await context.Ugyfelek.FindAsync(eszkoz.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága eszköz létrehozásához ennél az ügyfélnél.");
        }

        eszkoz.Ugyfel = null!;
        eszkoz.Telephely = null!;
        eszkoz.Gyarto = null!;
        eszkoz.Letrehozva = DateTime.UtcNow;
        
        context.Eszkozok.Add(eszkoz);
        await context.SaveChangesAsync();
        return eszkoz;
    }

    public async Task<Eszkoz> UpdateAsync(Eszkoz eszkoz)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await context.Eszkozok
            .Include(e => e.Ugyfel)
            .FirstOrDefaultAsync(e => e.Id == eszkoz.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága az eszköz módosításához.");
        }

        existing.Nev = eszkoz.Nev;
        existing.GyariSzam = eszkoz.GyariSzam;
        existing.Tipus = eszkoz.Tipus;
        existing.GyartoId = eszkoz.GyartoId;
        existing.UgyfelId = eszkoz.UgyfelId;
        existing.TelephelyId = eszkoz.TelephelyId;
        existing.Aktiv = eszkoz.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await context.Eszkozok
            .Include(e => e.Ugyfel)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eszkoz is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága az eszköz törléséhez.");
            }

            context.Eszkozok.Remove(eszkoz);
            await context.SaveChangesAsync();
        }
    }
}