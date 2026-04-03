using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class JogszabalyService : IJogszabalyService
{
    private readonly CrmDbContext _context;

    public JogszabalyService(CrmDbContext context) => _context = context;

    public async Task<List<Jogszabaly>> GetAllAsync() =>
        await _context.Jogszabalyok
            .AsNoTracking()
            .OrderBy(j => j.Tipus)
            .ThenBy(j => j.Szam)
            .ToListAsync();

    public async Task<Jogszabaly?> GetByIdAsync(int id) =>
        await _context.Jogszabalyok
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

    public async Task<Jogszabaly> CreateAsync(Jogszabaly jogszabaly)
    {
        jogszabaly.Letrehozva = DateTime.UtcNow;
        _context.Jogszabalyok.Add(jogszabaly);
        await _context.SaveChangesAsync();
        return jogszabaly;
    }

    public async Task<Jogszabaly> UpdateAsync(Jogszabaly jogszabaly)
    {
        var existing = await _context.Jogszabalyok.FindAsync(jogszabaly.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.Szam = jogszabaly.Szam;
        existing.Cim = jogszabaly.Cim;
        existing.Leiras = jogszabaly.Leiras;
        existing.Tipus = jogszabaly.Tipus;
        existing.Terulet = jogszabaly.Terulet;  // <-- EZ HIÁNYZOTT!
        existing.HatalyosKezdet = jogszabaly.HatalyosKezdet;
        existing.HatalyosVege = jogszabaly.HatalyosVege;
        existing.Url = jogszabaly.Url;
        existing.Megjegyzes = jogszabaly.Megjegyzes;
        existing.Aktiv = jogszabaly.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var jogszabaly = await _context.Jogszabalyok.FindAsync(id);
        if (jogszabaly is not null)
        {
            _context.Jogszabalyok.Remove(jogszabaly);
            await _context.SaveChangesAsync();
        }
    }
}