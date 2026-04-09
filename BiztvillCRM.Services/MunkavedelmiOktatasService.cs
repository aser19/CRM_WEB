using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MunkavedelmiOktatasService : IMunkavedelmiOktatasService
{
    private readonly CrmDbContext _context;  // <-- CrmDbContext, NEM ApplicationDbContext
    private readonly ITenantService _tenantService;

    public MunkavedelmiOktatasService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<MunkavedelmiOktatas>> GetAllAsync(bool mindenCegre = false)
    {
        var query = _context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok)
            .Include(o => o.Ceg)
            .Where(o => o.Aktiv);

        // Ha NEM mindenCegre, akkor csak a saját cég oktatásait
        if (!mindenCegre)
        {
            var cegId = _tenantService.GetCurrentCegId();
            query = query.Where(o => o.CegId == cegId);
        }

        return await query
            .OrderByDescending(o => o.OktatasDatuma)
            .ToListAsync();
    }

    public async Task<MunkavedelmiOktatas?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok)
            .FirstOrDefaultAsync(o => o.Id == id && o.CegId == cegId);
    }

    public async Task<MunkavedelmiOktatas?> GetByIdWithResztvevokAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        return await _context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok)
            .FirstOrDefaultAsync(o => o.Id == id && o.CegId == cegId);
    }

    public async Task<MunkavedelmiOktatas> CreateAsync(MunkavedelmiOktatas oktatas)
    {
        oktatas.CegId = _tenantService.GetCurrentCegId();
        oktatas.Letrehozva = DateTime.UtcNow;
        oktatas.KovetkezoOktatas = oktatas.OktatasDatuma.AddMonths(oktatas.IdoszakHonap);

        _context.MunkavedelmiOktatasok.Add(oktatas);
        await _context.SaveChangesAsync();
        return oktatas;
    }

    public async Task<MunkavedelmiOktatas> UpdateAsync(MunkavedelmiOktatas oktatas)
    {
        var meglevo = await GetByIdAsync(oktatas.Id)
            ?? throw new InvalidOperationException("Nem található az oktatás.");

        meglevo.Megnevezes = oktatas.Megnevezes;
        meglevo.Leiras = oktatas.Leiras;
        meglevo.OktatasDatuma = oktatas.OktatasDatuma;
        meglevo.IdoszakHonap = oktatas.IdoszakHonap;
        meglevo.KovetkezoOktatas = oktatas.OktatasDatuma.AddMonths(oktatas.IdoszakHonap);
        meglevo.OktatoNeve = oktatas.OktatoNeve;
        meglevo.Megjegyzes = oktatas.Megjegyzes;
        meglevo.UgyfelId = oktatas.UgyfelId;
        meglevo.TelephelyId = oktatas.TelephelyId;
        meglevo.Aktiv = oktatas.Aktiv;
        meglevo.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return meglevo;
    }

    public async Task DeleteAsync(int id)
    {
        var oktatas = await GetByIdAsync(id)
            ?? throw new InvalidOperationException("Nem található az oktatás.");

        _context.MunkavedelmiOktatasok.Remove(oktatas);
        await _context.SaveChangesAsync();
    }

    // Résztvevők kezelése
    public async Task<List<MunkavedelmiOktatasResztvevo>> GetResztvevokAsync(int oktatasId)
    {
        return await _context.MunkavedelmiOktatasResztvevok
            .Where(r => r.MunkavedelmiOktatasId == oktatasId)
            .OrderBy(r => r.Nev)
            .ToListAsync();
    }

    public async Task<MunkavedelmiOktatasResztvevo> AddResztvevoAsync(int oktatasId, MunkavedelmiOktatasResztvevo resztvevo)
    {
        resztvevo.MunkavedelmiOktatasId = oktatasId;
        _context.MunkavedelmiOktatasResztvevok.Add(resztvevo);
        await _context.SaveChangesAsync();
        return resztvevo;
    }

    public async Task RemoveResztvevoAsync(int resztvevoId)
    {
        var resztvevo = await _context.MunkavedelmiOktatasResztvevok.FindAsync(resztvevoId);
        if (resztvevo != null)
        {
            _context.MunkavedelmiOktatasResztvevok.Remove(resztvevo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateResztvevokAsync(int oktatasId, List<MunkavedelmiOktatasResztvevo> resztvevok)
    {
        var regiResztvevok = await _context.MunkavedelmiOktatasResztvevok
            .Where(r => r.MunkavedelmiOktatasId == oktatasId)
            .ToListAsync();
        
        _context.RemoveRange(regiResztvevok);
        
        foreach (var resztvevo in resztvevok)
        {
            resztvevo.Id = 0;
            resztvevo.MunkavedelmiOktatasId = oktatasId;
            _context.MunkavedelmiOktatasResztvevok.Add(resztvevo);
        }
        
        await _context.SaveChangesAsync();
    }
}