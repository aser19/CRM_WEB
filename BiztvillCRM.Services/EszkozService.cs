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
        var query = context.Eszkozok.Include(e => e.Gyarto).Include(e => e.Ugyfel).Include(e => e.Telephely).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(e => e.Ugyfel != null && cegIds.Contains(e.Ugyfel.CegId));
        }

        return await query.OrderBy(e => e.Nev).ToListAsync();
    }

    public async Task<Eszkoz?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Eszkozok.Include(e => e.Gyarto).Include(e => e.Ugyfel).Include(e => e.Telephely).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(e => e.Ugyfel != null && cegIds.Contains(e.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Eszkoz> CreateAsync(Eszkoz eszkoz)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ugyfel = await context.Ugyfelek.FindAsync(eszkoz.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (ugyfel == null || !cegIds.Contains(ugyfel.CegId))
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
        var existing = await context.Eszkozok.Include(e => e.Ugyfel).FirstOrDefaultAsync(e => e.Id == eszkoz.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.Ugyfel!.CegId))
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
        var eszkoz = await context.Eszkozok.Include(e => e.Ugyfel).FirstOrDefaultAsync(e => e.Id == id);
        if (eszkoz is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(eszkoz.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága az eszköz törléséhez.");
        }

        context.Eszkozok.Remove(eszkoz);
        await context.SaveChangesAsync();
    }
}