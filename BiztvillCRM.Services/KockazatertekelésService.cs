using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KockazatertekelésService : IKockazatertekelésService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public KockazatertekelésService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Kockazatertekeles>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.Kockazatertekelesek
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .Where(k => k.CegId == cegId && k.Aktiv)
            .OrderByDescending(k => k.ErtekelesDatuma)
            .ToListAsync();
    }

    public async Task<Kockazatertekeles?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.Kockazatertekelesek
            .Include(k => k.Ugyfel)
            .Include(k => k.Telephely)
            .FirstOrDefaultAsync(k => k.Id == id && k.CegId == cegId);
    }

    public async Task<Kockazatertekeles> CreateAsync(Kockazatertekeles kockazatertekeles)
    {
        kockazatertekeles.CegId = _tenantService.GetCurrentCegId();
        kockazatertekeles.Letrehozva = DateTime.UtcNow;

        _context.Kockazatertekelesek.Add(kockazatertekeles);
        await _context.SaveChangesAsync();
        return kockazatertekeles;
    }

    public async Task<Kockazatertekeles> UpdateAsync(Kockazatertekeles kockazatertekeles)
    {
        var meglevo = await GetByIdAsync(kockazatertekeles.Id)
            ?? throw new InvalidOperationException("Nem található a kockázatértékelés.");

        meglevo.Megnevezes = kockazatertekeles.Megnevezes;
        meglevo.UgyfelId = kockazatertekeles.UgyfelId;
        meglevo.TelephelyId = kockazatertekeles.TelephelyId;
        meglevo.ErtekelesDatuma = kockazatertekeles.ErtekelesDatuma;
        meglevo.KovetkezoFelulvizsgalat = kockazatertekeles.KovetkezoFelulvizsgalat;
        meglevo.KockazatiSzint = kockazatertekeles.KockazatiSzint;
        meglevo.Leiras = kockazatertekeles.Leiras;
        meglevo.Intezkedesek = kockazatertekeles.Intezkedesek;
        meglevo.FelelosNeve = kockazatertekeles.FelelosNeve;
        meglevo.Statusz = kockazatertekeles.Statusz;
        meglevo.Aktiv = kockazatertekeles.Aktiv;
        meglevo.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return meglevo;
    }

    public async Task DeleteAsync(int id)
    {
        var kockazatertekeles = await GetByIdAsync(id)
            ?? throw new InvalidOperationException("Nem található a kockázatértékelés.");

        _context.Kockazatertekelesek.Remove(kockazatertekeles);
        await _context.SaveChangesAsync();
    }
}