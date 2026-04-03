using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EszkozTipusService : IEszkozTipusService
{
    private readonly CrmDbContext _context;

    public EszkozTipusService(CrmDbContext context) => _context = context;

    public async Task<List<EszkozTipus>> GetAllAsync() =>
        await _context.EszkozTipusok
            .Where(e => e.Aktiv)
            .OrderBy(e => e.Nev)
            .ToListAsync();

    public async Task<EszkozTipus?> GetByIdAsync(int id) =>
        await _context.EszkozTipusok.FindAsync(id);

    public async Task<EszkozTipus> CreateAsync(EszkozTipus eszkozTipus)
    {
        eszkozTipus.Letrehozva = DateTime.UtcNow;
        eszkozTipus.Aktiv = true;
        _context.EszkozTipusok.Add(eszkozTipus);
        await _context.SaveChangesAsync();
        return eszkozTipus;
    }

    public async Task<EszkozTipus> UpdateAsync(EszkozTipus eszkozTipus)
    {
        var existing = await _context.EszkozTipusok.FindAsync(eszkozTipus.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Nev = eszkozTipus.Nev;
        existing.HitelesitesiIdotartamHonap = eszkozTipus.HitelesitesiIdotartamHonap;
        existing.Aktiv = eszkozTipus.Aktiv;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var eszkozTipus = await _context.EszkozTipusok.FindAsync(id);
        if (eszkozTipus is not null)
        {
            // Soft delete - csak inaktiválás
            eszkozTipus.Aktiv = false;
            await _context.SaveChangesAsync();
        }
    }
}