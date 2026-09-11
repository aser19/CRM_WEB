using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class CegService : ICegService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public CegService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Ceg>> GetAllAsync(bool csakAktiv = true)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.Cegek.AsQueryable();
        
        if (csakAktiv)
            query = query.Where(c => c.Aktiv);
            
        return await query.OrderBy(c => c.Nev).ToListAsync();
    }

    public async Task<Ceg?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Cegek.FindAsync(id);
    }

    public async Task<Ceg> CreateAsync(Ceg ceg)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        ceg.Letrehozva = DateTime.Now;
        ceg.Aktiv = true;
        
        context.Cegek.Add(ceg);
        await context.SaveChangesAsync();
        
        return ceg;
    }

    public async Task<Ceg> UpdateAsync(Ceg ceg)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existing = await context.Cegek.FindAsync(ceg.Id);
        if (existing == null)
            throw new InvalidOperationException("Cég nem található.");

        existing.Nev             = ceg.Nev;
        existing.Adoszam         = ceg.Adoszam;
        existing.Cim             = ceg.Cim;
        existing.Email           = ceg.Email;
        existing.Telefon         = ceg.Telefon;
        existing.Weboldal        = ceg.Weboldal;
        existing.AktivModulok    = ceg.AktivModulok;
        existing.Aktiv           = ceg.Aktiv;
        existing.MatricaElotag   = ceg.MatricaElotag;
        // NAV API mezők
        existing.NavLoginName      = ceg.NavLoginName;
        existing.NavPassword       = ceg.NavPassword;
        existing.NavXmlSignKey     = ceg.NavXmlSignKey;
        existing.NavTaxNumber      = ceg.NavTaxNumber;
        existing.NavTesztKornyezet = ceg.NavTesztKornyezet;
        existing.BelyegzoPath      = ceg.BelyegzoPath;
        existing.Modositva         = DateTime.Now;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> SetAktivAsync(int id, bool aktiv)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var ceg = await context.Cegek.FindAsync(id);
        if (ceg == null)
            return false;

        ceg.Aktiv = aktiv;
        ceg.Modositva = DateTime.Now;
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<int> GetFelhasznalokSzamaAsync(int cegId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync(f => f.CegId == cegId);
    }

    public async Task<int> GetUgyfelekSzamaAsync(int cegId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Ugyfelek.CountAsync(u => u.CegId == cegId);
    }
}