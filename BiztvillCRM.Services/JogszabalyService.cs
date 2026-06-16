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
            .Include(j => j.Tagek)
            .AsNoTracking()
            .OrderBy(j => j.Tipus)
            .ThenBy(j => j.Szam)
            .ToListAsync();

    public async Task<Jogszabaly?> GetByIdAsync(int id) =>
        await _context.Jogszabalyok
            .Include(j => j.Tagek)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

    public async Task<Jogszabaly> CreateAsync(Jogszabaly jogszabaly)
    {
        jogszabaly.Letrehozva = DateTime.UtcNow;
        jogszabaly.Tagek.Clear(); // Tagek kezelése SetTagekAsync-on keresztül történik
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
        existing.Terulet = jogszabaly.Terulet;
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

    // --- Tagek ---

    public async Task<List<JogszabalyTag>> GetAllTagekAsync() =>
        await _context.JogszabalyTagek
            .AsNoTracking()
            .OrderBy(t => t.Nev)
            .ToListAsync();

    public async Task<JogszabalyTag> CreateTagAsync(JogszabalyTag tag)
    {
        _context.JogszabalyTagek.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _context.JogszabalyTagek.FindAsync(id);
        if (tag is not null)
        {
            _context.JogszabalyTagek.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetTagekAsync(int jogszabalyId, List<int> tagIds)
    {
        var jogszabaly = await _context.Jogszabalyok
            .Include(j => j.Tagek)
            .FirstOrDefaultAsync(j => j.Id == jogszabalyId)
            ?? throw new InvalidOperationException("Jogszabály nem található.");

        jogszabaly.Tagek.Clear();

        if (tagIds.Count > 0)
        {
            var tagek = await _context.JogszabalyTagek
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();
            foreach (var tag in tagek)
                jogszabaly.Tagek.Add(tag);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(JogszabalyTag tag)
    {
        var existing = await _context.JogszabalyTagek.FindAsync(tag.Id)
            ?? throw new InvalidOperationException("Tag nem található.");
        existing.Nev = tag.Nev;
        existing.Szin = tag.Szin;
        await _context.SaveChangesAsync();
    }
}