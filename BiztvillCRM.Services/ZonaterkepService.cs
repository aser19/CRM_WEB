using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class ZonaterkepService : IZonaterkepService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public ZonaterkepService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Zonaterkep>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.Zonaterkepek
            .Include(z => z.Ugyfel)
            .Include(z => z.Telephely)
            .Where(z => z.CegId == cegId && z.Aktiv)
            .OrderBy(z => z.Megnevezes)
            .ToListAsync();
    }

    public async Task<Zonaterkep?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.Zonaterkepek
            .Include(z => z.Ugyfel)
            .Include(z => z.Telephely)
            .FirstOrDefaultAsync(z => z.Id == id && z.CegId == cegId);
    }

    public async Task<Zonaterkep> CreateAsync(Zonaterkep zonaterkep)
    {
        zonaterkep.CegId = _tenantService.GetCurrentCegId();
        zonaterkep.Letrehozva = DateTime.UtcNow;

        _context.Zonaterkepek.Add(zonaterkep);
        await _context.SaveChangesAsync();
        return zonaterkep;
    }

    public async Task<Zonaterkep> UpdateAsync(Zonaterkep zonaterkep)
    {
        var meglevo = await GetByIdAsync(zonaterkep.Id)
            ?? throw new InvalidOperationException("Nem található a zónatérkép.");

        meglevo.Megnevezes = zonaterkep.Megnevezes;
        meglevo.UgyfelId = zonaterkep.UgyfelId;
        meglevo.TelephelyId = zonaterkep.TelephelyId;
        meglevo.ZonaTipus = zonaterkep.ZonaTipus;
        meglevo.Leiras = zonaterkep.Leiras;
        meglevo.FajlNev = zonaterkep.FajlNev;
        meglevo.FajlUtvonal = zonaterkep.FajlUtvonal;
        meglevo.ErvenyessegKezdete = zonaterkep.ErvenyessegKezdete;
        meglevo.ErvenyessegVege = zonaterkep.ErvenyessegVege;
        meglevo.Aktiv = zonaterkep.Aktiv;
        meglevo.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return meglevo;
    }

    public async Task DeleteAsync(int id)
    {
        var zonaterkep = await GetByIdAsync(id)
            ?? throw new InvalidOperationException("Nem található a zónatérkép.");

        _context.Zonaterkepek.Remove(zonaterkep);
        await _context.SaveChangesAsync();
    }
}