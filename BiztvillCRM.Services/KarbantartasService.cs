using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KarbantartasService : IKarbantartasService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public KarbantartasService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Karbantartas>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Karbantartasok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e!.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.Eszkoz != null && k.Eszkoz.Ugyfel != null && k.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.OrderByDescending(k => k.Datum).ToListAsync();
    }

    public async Task<Karbantartas?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Karbantartasok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e!.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.Eszkoz != null && k.Eszkoz.Ugyfel != null && k.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Karbantartas> CreateAsync(Karbantartas karbantartas)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await _context.Eszkozok
            .Include(e => e.Ugyfel)
            .FirstOrDefaultAsync(e => e.Id == karbantartas.EszkozId);

        if (eszkoz is null)
        {
            throw new InvalidOperationException("Az eszköz nem található.");
        }

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz.Ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága karbantartás létrehozásához ennél az eszköznél.");
        }

        karbantartas.Letrehozva = DateTime.UtcNow;
        _context.Karbantartasok.Add(karbantartas);
        await _context.SaveChangesAsync();
        return karbantartas;
    }

    public async Task<Karbantartas> UpdateAsync(Karbantartas karbantartas)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Karbantartasok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e!.Ugyfel)
            .FirstOrDefaultAsync(k => k.Id == karbantartas.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Eszkoz?.Ugyfel?.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a karbantartás módosításához.");
        }

        existing.EszkozId = karbantartas.EszkozId;
        existing.Datum = karbantartas.Datum;
        existing.KovetkezoDatum = karbantartas.KovetkezoDatum;
        existing.Leiras = karbantartas.Leiras;
        existing.Elvegzo = karbantartas.Elvegzo;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var karbantartas = await _context.Karbantartasok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e!.Ugyfel)
            .FirstOrDefaultAsync(k => k.Id == id);

        if (karbantartas is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && karbantartas.Eszkoz?.Ugyfel?.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a karbantartás törléséhez.");
            }

            _context.Karbantartasok.Remove(karbantartas);
            await _context.SaveChangesAsync();
        }
    }
}