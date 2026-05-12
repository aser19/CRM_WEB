using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TanusitvanyService : ITanusitvanyService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public TanusitvanyService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Tanusitvany>> GetAllAsync()
    {
        var query = _context.Tanusitvanyok.Include(t => t.Ugyfel).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(t => t.Ugyfel != null && cegIds.Contains(t.Ugyfel.CegId));
        }

        return await query.OrderBy(t => t.Nev).ToListAsync();
    }

    public async Task<Tanusitvany?> GetByIdAsync(int id)
    {
        var query = _context.Tanusitvanyok.Include(t => t.Ugyfel).AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(t => t.Ugyfel != null && cegIds.Contains(t.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tanusitvany> CreateAsync(Tanusitvany tanusitvany)
    {
        var ugyfel = await _context.Ugyfelek.FindAsync(tanusitvany.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (ugyfel == null || !cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága tanúsítvány létrehozásához ennél az ügyfélnél.");
        }

        tanusitvany.Letrehozva = DateTime.UtcNow;
        _context.Tanusitvanyok.Add(tanusitvany);
        await _context.SaveChangesAsync();
        return tanusitvany;
    }

    public async Task<Tanusitvany> UpdateAsync(Tanusitvany tanusitvany)
    {
        var existing = await _context.Tanusitvanyok.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == tanusitvany.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a tanúsítvány módosításához.");
        }

        existing.Nev = tanusitvany.Nev;
        existing.Szam = tanusitvany.Szam;
        existing.KiadoDatum = tanusitvany.KiadoDatum;
        existing.LejaratDatum = tanusitvany.LejaratDatum;
        existing.UgyfelId = tanusitvany.UgyfelId;
        existing.Megjegyzes = tanusitvany.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var tanusitvany = await _context.Tanusitvanyok.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == id);
        if (tanusitvany is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(tanusitvany.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a tanúsítvány törléséhez.");
        }

        _context.Tanusitvanyok.Remove(tanusitvany);
        await _context.SaveChangesAsync();
    }
}