using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KalibracioService : IKalibracioService
{
    private readonly CrmDbContext _context;
    public KalibracioService(CrmDbContext context) => _context = context;

    public async Task<List<Kalibracio>> GetAllAsync() =>
        await _context.Kalibraciok.Include(k => k.Eszkoz).OrderByDescending(k => k.Datum).ToListAsync();

    public async Task<Kalibracio?> GetByIdAsync(int id) =>
        await _context.Kalibraciok.Include(k => k.Eszkoz).FirstOrDefaultAsync(k => k.Id == id);

    public async Task<Kalibracio> CreateAsync(Kalibracio kalibracio)
    {
        kalibracio.Letrehozva = DateTime.UtcNow;
        _context.Kalibraciok.Add(kalibracio);
        await _context.SaveChangesAsync();
        return kalibracio;
    }

    public async Task<Kalibracio> UpdateAsync(Kalibracio kalibracio)
    {
        kalibracio.Modositva = DateTime.UtcNow;
        _context.Entry(kalibracio).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return kalibracio;
    }

    public async Task DeleteAsync(int id)
    {
        var kalibracio = await _context.Kalibraciok.FindAsync(id);
        if (kalibracio is not null)
        {
            _context.Kalibraciok.Remove(kalibracio);
            await _context.SaveChangesAsync();
        }
    }
}