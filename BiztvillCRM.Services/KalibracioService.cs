using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KalibracioService : IKalibracioService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public KalibracioService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Kalibracio>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Kalibraciok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.OrderByDescending(k => k.Datum).ToListAsync();
    }

    public async Task<Kalibracio?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Kalibraciok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.Eszkoz.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Kalibracio> CreateAsync(Kalibracio kalibracio)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var eszkoz = await _context.Eszkozok.Include(e => e.Ugyfel).FirstOrDefaultAsync(e => e.Id == kalibracio.EszkozId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && eszkoz?.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága kalibráció létrehozásához ennél az eszköznél.");
        }

        kalibracio.Letrehozva = DateTime.UtcNow;
        _context.Kalibraciok.Add(kalibracio);
        await _context.SaveChangesAsync();
        return kalibracio;
    }

    public async Task<Kalibracio> UpdateAsync(Kalibracio kalibracio)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Kalibraciok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(k => k.Id == kalibracio.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Eszkoz.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a kalibráció módosításához.");
        }

        existing.EszkozId = kalibracio.EszkozId;
        existing.Datum = kalibracio.Datum;
        existing.KovetkezoDatum = kalibracio.KovetkezoDatum;
        existing.Bizonyitvany = kalibracio.Bizonyitvany;
        existing.Elvegzo = kalibracio.Elvegzo;
        existing.Sikeres = kalibracio.Sikeres;
        existing.Megjegyzes = kalibracio.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var kalibracio = await _context.Kalibraciok
            .Include(k => k.Eszkoz)
                .ThenInclude(e => e.Ugyfel)
            .FirstOrDefaultAsync(k => k.Id == id);

        if (kalibracio is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && kalibracio.Eszkoz.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a kalibráció törléséhez.");
            }

            _context.Kalibraciok.Remove(kalibracio);
            await _context.SaveChangesAsync();
        }
    }
}