using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KarbantartasService : IKarbantartasService
{
    private readonly CrmDbContext _context;

    public KarbantartasService(CrmDbContext context) => _context = context;

    public async Task<List<Karbantartas>> GetAllAsync() =>
        await _context.Karbantartasok.Include(k => k.Eszkoz).OrderByDescending(k => k.Datum).ToListAsync();

    public async Task<Karbantartas?> GetByIdAsync(int id) =>
        await _context.Karbantartasok.Include(k => k.Eszkoz).FirstOrDefaultAsync(k => k.Id == id);

    public async Task<Karbantartas> CreateAsync(Karbantartas karbantartas)
    {
        karbantartas.Letrehozva = DateTime.UtcNow;
        _context.Karbantartasok.Add(karbantartas);
        await _context.SaveChangesAsync();
        return karbantartas;
    }

    public async Task<Karbantartas> UpdateAsync(Karbantartas karbantartas)
    {
        var existing = await _context.Karbantartasok.FindAsync(karbantartas.Id)
            ?? throw new InvalidOperationException("Nem található.");

        existing.EszkozId = karbantartas.EszkozId;
        existing.Datum = karbantartas.Datum;
        existing.KovetkezoDatum = karbantartas.KovetkezoDatum;
        existing.Leiras = karbantartas.Leiras;
        existing.Elvegzo = karbantartas.Elvegzo;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var karbantartas = await _context.Karbantartasok.FindAsync(id);
        if (karbantartas is not null)
        {
            _context.Karbantartasok.Remove(karbantartas);
            await _context.SaveChangesAsync();
        }
    }
}