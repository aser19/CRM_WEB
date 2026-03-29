using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozService : IEszkozService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public EszkozService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Eszkoz>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Eszkozok
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
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Eszkozok
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
        var cegId = _tenantService.GetCurrentCegId();
        var ugyfel = await _context.Ugyfelek.FindAsync(eszkoz.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága eszköz létrehozásához ennél az ügyfélnél.");
        }

        eszkoz.Letrehozva = DateTime.UtcNow;
        _context.Eszkozok.Add(eszkoz);
        await _context.SaveChangesAsync();
        return eszkoz;
    }

    public async Task<Eszkoz> UpdateAsync(Eszkoz eszkoz)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Eszkozok
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

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await _context.Eszkozok
            .Include(e => e.Ugyfel)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eszkoz is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága az eszköz törléséhez.");
            }

            _context.Eszkozok.Remove(eszkoz);
            await _context.SaveChangesAsync();
        }
    }
}