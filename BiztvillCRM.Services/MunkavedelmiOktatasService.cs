using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MunkavedelmiOktatasService : IMunkavedelmiOktatasService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public MunkavedelmiOktatasService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<MunkavedelmiOktatas>> GetAllAsync(bool mindenCegre = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok)
            .Include(o => o.Ceg)
            .Where(o => o.Aktiv);

        if (!mindenCegre)
        {
            var cegId = _tenantService.GetCurrentCegId();
            query = query.Where(o => o.CegId == cegId);
        }

        return await query
            .OrderByDescending(o => o.OktatasDatuma)
            .ToListAsync();
    }

    public async Task<MunkavedelmiOktatas?> GetByIdAsync(int id, bool mindenCegre = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var baseQuery = context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok);

        if (mindenCegre)
        {
            return await baseQuery.FirstOrDefaultAsync(o => o.Id == id);
        }
        
        var cegId = _tenantService.GetCurrentCegId();
        return await baseQuery.FirstOrDefaultAsync(o => o.Id == id && o.CegId == cegId);
    }

    public async Task<MunkavedelmiOktatas?> GetByIdWithResztvevokAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        return await context.MunkavedelmiOktatasok
            .Include(o => o.Ugyfel)
            .Include(o => o.Telephely)
            .Include(o => o.Resztvevok)
            .FirstOrDefaultAsync(o => o.Id == id && o.CegId == cegId);
    }

    public async Task<MunkavedelmiOktatas> CreateAsync(MunkavedelmiOktatas oktatas)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        oktatas.CegId = _tenantService.GetCurrentCegId();
        oktatas.Letrehozva = DateTime.UtcNow;
        oktatas.KovetkezoOktatas = oktatas.OktatasDatuma.AddMonths(oktatas.IdoszakHonap);

        oktatas.Ugyfel = null!;
        oktatas.Telephely = null;
        oktatas.Ceg = null!;

        context.MunkavedelmiOktatasok.Add(oktatas);
        await context.SaveChangesAsync();
        return oktatas;
    }

    public async Task<MunkavedelmiOktatas> UpdateAsync(MunkavedelmiOktatas oktatas)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        var meglevo = await context.MunkavedelmiOktatasok
            .FirstOrDefaultAsync(o => o.Id == oktatas.Id && o.CegId == cegId)
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
        meglevo.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return meglevo;
    }

    public async Task DeleteAsync(int id, bool mindenCegre = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        MunkavedelmiOktatas? oktatas;
        if (mindenCegre)
        {
            oktatas = await context.MunkavedelmiOktatasok.FindAsync(id);
        }
        else
        {
            var cegId = _tenantService.GetCurrentCegId();
            oktatas = await context.MunkavedelmiOktatasok
                .FirstOrDefaultAsync(o => o.Id == id && o.CegId == cegId);
        }

        if (oktatas is not null)
        {
            oktatas.Aktiv = false;
            oktatas.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<MunkavedelmiOktatasResztvevo>> GetResztvevokAsync(int oktatasId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MunkavedelmiOktatasResztvevok
            .Where(r => r.MunkavedelmiOktatasId == oktatasId)
            .ToListAsync();
    }

    public async Task<MunkavedelmiOktatasResztvevo> AddResztvevoAsync(int oktatasId, MunkavedelmiOktatasResztvevo resztvevo)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        resztvevo.MunkavedelmiOktatasId = oktatasId;
        context.MunkavedelmiOktatasResztvevok.Add(resztvevo);
        await context.SaveChangesAsync();
        return resztvevo;
    }

    public async Task RemoveResztvevoAsync(int resztvevoId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var resztvevo = await context.MunkavedelmiOktatasResztvevok.FindAsync(resztvevoId);
        if (resztvevo is not null)
        {
            context.MunkavedelmiOktatasResztvevok.Remove(resztvevo);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateResztvevokAsync(int oktatasId, List<MunkavedelmiOktatasResztvevo> resztvevok)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meglevoResztvevok = await context.MunkavedelmiOktatasResztvevok
            .Where(r => r.MunkavedelmiOktatasId == oktatasId)
            .ToListAsync();
        
        context.MunkavedelmiOktatasResztvevok.RemoveRange(meglevoResztvevok);

        foreach (var r in resztvevok)
        {
            r.MunkavedelmiOktatasId = oktatasId;
            r.Id = 0;
            context.MunkavedelmiOktatasResztvevok.Add(r);
        }

        await context.SaveChangesAsync();
    }
}