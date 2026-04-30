using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class ErintesvedelmiModOsztalyService : IErintesvedelmiModOsztalyService
{
    private readonly CrmDbContext _context;

    public ErintesvedelmiModOsztalyService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<ErintesvedelmiModOsztaly>> GetAllAsync()
    {
        return await _context.ErintesvedelmiModOsztalyok
            .OrderBy(m => m.Sorrend)
            .ThenBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<List<ErintesvedelmiModOsztaly>> GetAllActiveAsync()
    {
        return await _context.ErintesvedelmiModOsztalyok
            .Where(m => m.Aktiv)
            .OrderBy(m => m.Sorrend)
            .ThenBy(m => m.Nev)
            .ToListAsync();
    }

    public async Task<ErintesvedelmiModOsztaly?> GetByIdAsync(int id)
    {
        return await _context.ErintesvedelmiModOsztalyok.FindAsync(id);
    }

    public async Task<int> CreateAsync(ErintesvedelmiModOsztaly modOsztaly)
    {
        modOsztaly.Letrehozva = DateTime.Now;
        _context.ErintesvedelmiModOsztalyok.Add(modOsztaly);
        await _context.SaveChangesAsync();
        return modOsztaly.Id;
    }

    public async Task UpdateAsync(ErintesvedelmiModOsztaly modOsztaly)
    {
        _context.ErintesvedelmiModOsztalyok.Update(modOsztaly);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var modOsztaly = await _context.ErintesvedelmiModOsztalyok.FindAsync(id);
        if (modOsztaly != null)
        {
            _context.ErintesvedelmiModOsztalyok.Remove(modOsztaly);
            await _context.SaveChangesAsync();
        }
    }
}