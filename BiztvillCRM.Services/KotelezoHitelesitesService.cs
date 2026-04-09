using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KotelezoHitelesitesService : IKotelezoHitelesitesService
{
    private readonly CrmDbContext _context;

    public KotelezoHitelesitesService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<KotelezoHitelesites>> GetAllAsync()
    {
        return await _context.KotelezoHitelesitesek
            .Where(k => k.Aktiv)
            .OrderBy(k => k.Megnevezes)
            .ToListAsync();
    }

    public async Task<KotelezoHitelesites?> GetByIdAsync(int id)
    {
        return await _context.KotelezoHitelesitesek.FindAsync(id);
    }

    public async Task<KotelezoHitelesites> CreateAsync(KotelezoHitelesites kotelezoHitelesites)
    {
        kotelezoHitelesites.Letrehozva = DateTime.UtcNow;
        _context.KotelezoHitelesitesek.Add(kotelezoHitelesites);
        await _context.SaveChangesAsync();
        return kotelezoHitelesites;
    }

    public async Task<KotelezoHitelesites> UpdateAsync(KotelezoHitelesites kotelezoHitelesites)
    {
        var meglevo = await _context.KotelezoHitelesitesek.FindAsync(kotelezoHitelesites.Id)
            ?? throw new InvalidOperationException("Nem található a kötelező hitelesítés.");

        meglevo.Megnevezes = kotelezoHitelesites.Megnevezes;
        meglevo.JogszabalyiHivatkozas = kotelezoHitelesites.JogszabalyiHivatkozas;
        meglevo.HitelesitesiIdoszakHonap = kotelezoHitelesites.HitelesitesiIdoszakHonap;
        meglevo.Megjegyzes = kotelezoHitelesites.Megjegyzes;
        meglevo.Aktiv = kotelezoHitelesites.Aktiv;
        meglevo.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return meglevo;
    }

    public async Task DeleteAsync(int id)
    {
        var kotelezoHitelesites = await _context.KotelezoHitelesitesek.FindAsync(id)
            ?? throw new InvalidOperationException("Nem található a kötelező hitelesítés.");

        _context.KotelezoHitelesitesek.Remove(kotelezoHitelesites);
        await _context.SaveChangesAsync();
    }
}