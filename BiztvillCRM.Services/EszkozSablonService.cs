using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozSablonService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService) : IEszkozSablonService
{
    public async Task<List<EszkozSablon>> GetAllAsync()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        
        // ✅ MÓDOSÍTOTT: NULL = admin sablon, egyébként cég-specifikus
        return await context.EszkozSablonok
            .Include(s => s.Alkatreszek.OrderBy(a => a.Sorrend))
            .Where(s => (s.CegId == null || s.CegId == cegId) && s.Aktiv)
            .OrderBy(s => s.CegId == null ? 0 : 1) // Admin sablonok elől
            .ThenBy(s => s.Megnevezes)
            .ToListAsync();
    }

    public async Task<EszkozSablon?> GetByIdAsync(int id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        
        return await context.EszkozSablonok
            .Include(s => s.Alkatreszek.OrderBy(a => a.Sorrend))
            .FirstOrDefaultAsync(s => s.Id == id && (s.CegId == null || s.CegId == cegId));
    }

    public async Task<EszkozSablon?> GetByMegnevezesAsync(string megnevezes)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        
        return await context.EszkozSablonok
            .Include(s => s.Alkatreszek.OrderBy(a => a.Sorrend))
            .Where(s => (s.CegId == null || s.CegId == cegId) && 
                        s.Aktiv && 
                        s.Megnevezes.ToLower() == megnevezes.ToLower())
            .OrderBy(s => s.CegId == null ? 0 : 1) // Admin sablon előnyben
            .FirstOrDefaultAsync();
    }

    public async Task<EszkozSablon> CreateAsync(EszkozSablon sablon)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        var isAdmin = tenantService.IsInRole(FelhasznaloSzerepkor.Admin);

        // Ellenőrizzük, hogy nincs-e már ilyen nevű admin sablon
        var adminSablonLetezik = await context.EszkozSablonok
            .AnyAsync(s => s.CegId == null && 
                          s.Aktiv && 
                          s.Megnevezes.ToLower() == sablon.Megnevezes.ToLower());

        if (adminSablonLetezik && !isAdmin)
        {
            throw new InvalidOperationException(
                $"Már létezik '{sablon.Megnevezes}' nevű admin sablon! Kérlek válassz másik nevet.");
        }

        // ✅ MÓDOSÍTOTT: Admin → CegId = NULL, Felhasználó → CegId = saját cég
        sablon.CegId = isAdmin ? null : cegId;
        sablon.Letrehozva = DateTime.UtcNow;
        sablon.Ceg = null;
        
        for (int i = 0; i < sablon.Alkatreszek.Count; i++)
        {
            sablon.Alkatreszek[i].Sorrend = i + 1;
            sablon.Alkatreszek[i].EszkozSablon = null;
        }
        
        context.EszkozSablonok.Add(sablon);
        await context.SaveChangesAsync();
        
        return sablon;
    }

    public async Task<EszkozSablon> UpdateAsync(EszkozSablon sablon)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        var isAdmin = tenantService.IsInRole(FelhasznaloSzerepkor.Admin);

        var existing = await context.EszkozSablonok
            .Include(s => s.Alkatreszek)
            .FirstOrDefaultAsync(s => s.Id == sablon.Id)
            ?? throw new InvalidOperationException("Sablon nem található.");

        // ✅ MÓDOSÍTOTT: NULL = admin sablon
        if (existing.CegId == null && !isAdmin)
        {
            throw new UnauthorizedAccessException("Admin sablonokat csak adminisztrátor módosíthatja!");
        }

        if (existing.CegId != null && existing.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Csak a saját céged sablonját módosíthatod!");
        }

        existing.Megnevezes = sablon.Megnevezes;
        existing.Tipus = sablon.Tipus;
        existing.Azonosito = sablon.Azonosito;
        existing.VedelmiOsztaly = sablon.VedelmiOsztaly;
        existing.Telj = sablon.Telj;
        existing.Megtekint = sablon.Megtekint;
        existing.Aktiv = sablon.Aktiv;
        existing.Megjegyzes = sablon.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        context.EszkozSablonAlkatreszek.RemoveRange(existing.Alkatreszek);
        
        existing.Alkatreszek = sablon.Alkatreszek.Select((a, i) => new EszkozSablonAlkatresz
        {
            EszkozSablonId = existing.Id,
            Sorrend = i + 1,
            Megnevezes = a.Megnevezes,
            Tipus = a.Tipus,
            Azonosito = a.Azonosito,
            VedelmiOsztaly = a.VedelmiOsztaly ?? "I",
            Telj = a.Telj ?? "230V",
            Megtekint = a.Megtekint ?? "MF"
        }).ToList();

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var cegId = tenantService.GetCurrentCegId();
        var isAdmin = tenantService.IsInRole(FelhasznaloSzerepkor.Admin);

        var sablon = await context.EszkozSablonok
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sablon == null) return;

        // ✅ MÓDOSÍTOTT: NULL = admin sablon
        if (sablon.CegId == null && !isAdmin)
        {
            throw new UnauthorizedAccessException("Admin sablonokat csak adminisztrátor törölheti!");
        }

        if (sablon.CegId != null && sablon.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Csak a saját céged sablonját törölheted!");
        }

        context.EszkozSablonok.Remove(sablon);
        await context.SaveChangesAsync();
    }
}