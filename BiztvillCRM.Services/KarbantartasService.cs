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
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.CegId == cegId);
        }

        return await query.OrderByDescending(k => k.Datum).ToListAsync();
    }

    public async Task<Karbantartas?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Karbantartasok
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Include(k => k.KarbantartasTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(k => k.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Karbantartas> CreateAsync(Karbantartas karbantartas)
    {
        var cegId = _tenantService.GetCurrentCegId();
        karbantartas.CegId = cegId;
        karbantartas.Letrehozva = DateTime.UtcNow;
        
        // Következő dátum automatikus számítása a típus alapján
        await SzamolKovetkezoDatumAsync(karbantartas);
        
        _context.Karbantartasok.Add(karbantartas);
        await _context.SaveChangesAsync();
        return karbantartas;
    }

    public async Task<Karbantartas> UpdateAsync(Karbantartas karbantartas)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Karbantartasok.FirstOrDefaultAsync(k => k.Id == karbantartas.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága.");
        }

        existing.UgyfelId = karbantartas.UgyfelId;
        existing.TelephelyId = karbantartas.TelephelyId;
        existing.KarbantartasTipusId = karbantartas.KarbantartasTipusId;
        existing.Datum = karbantartas.Datum;
        existing.KovetkezoDatum = karbantartas.KovetkezoDatum;
        existing.Leiras = karbantartas.Leiras;
        existing.Elvegzo = karbantartas.Elvegzo;
        existing.Elvegezve = karbantartas.Elvegezve;
        existing.Modositva = DateTime.UtcNow;

        // Következő dátum automatikus számítása a típus alapján
        await SzamolKovetkezoDatumAsync(existing);

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var karbantartas = await _context.Karbantartasok.FirstOrDefaultAsync(k => k.Id == id);

        if (karbantartas is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && karbantartas.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága.");
            }

            _context.Karbantartasok.Remove(karbantartas);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SzamolKovetkezoDatumAsync(Karbantartas karbantartas)
    {
        var tipus = await _context.KarbantartasTipusok.FindAsync(karbantartas.KarbantartasTipusId);
        
        if (tipus is null || tipus.Eseti)
        {
            // Eseti karbantartás - nincs következő dátum
            karbantartas.KovetkezoDatum = null;
        }
        else
        {
            // Ismétlődő karbantartás - számoljuk a következő dátumot
            karbantartas.KovetkezoDatum = karbantartas.Datum.AddMonths(tipus.IsmetlodesHonap);
        }
    }
}