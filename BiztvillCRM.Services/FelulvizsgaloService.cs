using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class FelulvizsgaloService(CrmDbContext context, ITenantService tenantService) : IFelulvizsgaloService
{
    public async Task<List<Felulvizsgalo>> GetAllAsync()
    {
        var cegId = tenantService.GetCurrentCegId();
        return await context.Felulvizsgalok
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.KepzesTipus)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.Tovabbkepzesek)
            .Where(f => f.CegId == cegId && f.Aktiv)
            .OrderBy(f => f.Nev)
            .ToListAsync();
    }

    public async Task<List<Felulvizsgalo>> GetAllAdminAsync()
    {
        return await context.Felulvizsgalok
            .Include(f => f.Ceg)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.KepzesTipus)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.Tovabbkepzesek)
            .OrderBy(f => f.Nev)
            .ToListAsync();
    }

    public async Task<Felulvizsgalo?> GetByIdAsync(int id)
    {
        var cegId = tenantService.GetCurrentCegId();
        var query = context.Felulvizsgalok.AsQueryable();

        if (!tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            query = query.Where(f => f.CegId == cegId);

        return await query.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Felulvizsgalo?> GetByIdWithDetailsAsync(int id)
    {
        var cegId = tenantService.GetCurrentCegId();
        var query = context.Felulvizsgalok
            .Include(f => f.Ceg)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.KepzesTipus)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.Tovabbkepzesek)
            .AsQueryable();

        if (!tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            query = query.Where(f => f.CegId == cegId);

        return await query.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Felulvizsgalo> CreateAsync(Felulvizsgalo felulvizsgalo)
    {
        felulvizsgalo.Letrehozva = DateTime.Now;
        
        if (!tenantService.IsInRole(FelhasznaloSzerepkor.Admin) || felulvizsgalo.CegId == 0)
            felulvizsgalo.CegId = tenantService.GetCurrentCegId();
        
        context.Felulvizsgalok.Add(felulvizsgalo);
        await context.SaveChangesAsync();
        return felulvizsgalo;
    }

    public async Task<Felulvizsgalo> UpdateAsync(Felulvizsgalo felulvizsgalo)
    {
        var cegId = tenantService.GetCurrentCegId();
        var existing = await context.Felulvizsgalok.FindAsync(felulvizsgalo.Id)
            ?? throw new InvalidOperationException("Felülvizsgáló nem található.");

        if (!tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.CegId != cegId)
            throw new UnauthorizedAccessException("Nincs jogosultsága.");

        existing.Nev = felulvizsgalo.Nev;
        existing.Jogosultsag = felulvizsgalo.Jogosultsag;
        existing.Email = felulvizsgalo.Email;
        existing.Telefon = felulvizsgalo.Telefon;
        existing.Megjegyzes = felulvizsgalo.Megjegyzes;
        existing.Aktiv = felulvizsgalo.Aktiv;
        existing.Modositva = DateTime.Now;
        
        if (tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            existing.CegId = felulvizsgalo.CegId;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = tenantService.GetCurrentCegId();
        var felulvizsgalo = await context.Felulvizsgalok.FindAsync(id);
        
        if (felulvizsgalo is not null)
        {
            if (!tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && felulvizsgalo.CegId != cegId)
                throw new UnauthorizedAccessException("Nincs jogosultsága.");
            
            context.Felulvizsgalok.Remove(felulvizsgalo);
            await context.SaveChangesAsync();
        }
    }

    // === KÉPZÉSEK ===

    public async Task<FelulvizsgaloKepzes> AddKepzesAsync(int felulvizsgaloId, FelulvizsgaloKepzes kepzes)
    {
        kepzes.FelulvizsgaloId = felulvizsgaloId;
        kepzes.Letrehozva = DateTime.Now;
        
        context.FelulvizsgaloKepzesek.Add(kepzes);
        await context.SaveChangesAsync();
        
        return await context.FelulvizsgaloKepzesek
            .Include(k => k.KepzesTipus)
            .FirstAsync(k => k.Id == kepzes.Id);
    }

    public async Task<FelulvizsgaloKepzes> UpdateKepzesAsync(FelulvizsgaloKepzes kepzes)
    {
        var existing = await context.FelulvizsgaloKepzesek.FindAsync(kepzes.Id)
            ?? throw new InvalidOperationException("Képzés nem található.");

        existing.KepzesTipusId = kepzes.KepzesTipusId;
        existing.BizonyitvanySzam = kepzes.BizonyitvanySzam;
        existing.BizonyitvanyKelte = kepzes.BizonyitvanyKelte;
        existing.BizonyitvanyLejarat = kepzes.BizonyitvanyLejarat;
        existing.Megjegyzes = kepzes.Megjegyzes;
        existing.Aktiv = kepzes.Aktiv;
        existing.Modositva = DateTime.Now;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteKepzesAsync(int kepzesId)
    {
        var kepzes = await context.FelulvizsgaloKepzesek.FindAsync(kepzesId);
        if (kepzes is not null)
        {
            context.FelulvizsgaloKepzesek.Remove(kepzes);
            await context.SaveChangesAsync();
        }
    }

    // === TOVÁBBKÉPZÉSEK ===

    public async Task<KepzesTovabbkepzes> AddTovabbkepzesAsync(int kepzesId, KepzesTovabbkepzes tovabbkepzes)
    {
        tovabbkepzes.FelulvizsgaloKepzesId = kepzesId;
        tovabbkepzes.Letrehozva = DateTime.Now;
        
        context.KepzesTovabbkepzesek.Add(tovabbkepzes);
        await context.SaveChangesAsync();
        return tovabbkepzes;
    }

    public async Task DeleteTovabbkepzesAsync(int tovabbkepzesId)
    {
        var tovabbkepzes = await context.KepzesTovabbkepzesek.FindAsync(tovabbkepzesId);
        if (tovabbkepzes is not null)
        {
            context.KepzesTovabbkepzesek.Remove(tovabbkepzes);
            await context.SaveChangesAsync();
        }
    }

    // === LEKÉRDEZÉSEK ===

    public async Task<List<Felulvizsgalo>> GetLejaroKepzesekkelAsync(int naponBelul = 90)
    {
        var cegId = tenantService.GetCurrentCegId();
        
        return await context.Felulvizsgalok
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.KepzesTipus)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.Tovabbkepzesek)
            .Where(f => f.CegId == cegId && f.Aktiv)
            .Where(f => f.Kepzesek.Any(k => 
                k.KepzesTipus != null && 
                k.KepzesTipus.TovabbkepzesKotelezo))
            .ToListAsync();
    }
}