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
        await _context.Jogszabalyok.OrderBy(j => j.Szam).ToListAsync();

    public async Task<Jogszabaly?> GetByIdAsync(int id) =>
        await _context.Jogszabalyok.FindAsync(id);

    public async Task<Jogszabaly> CreateAsync(Jogszabaly jogszabaly)
    {
        jogszabaly.Letrehozva = DateTime.UtcNow;
        _context.Jogszabalyok.Add(jogszabaly);
        await _context.SaveChangesAsync();
        return jogszabaly;
    }

    public async Task<Jogszabaly> UpdateAsync(Jogszabaly jogszabaly)
    {
        jogszabaly.Modositva = DateTime.UtcNow;
        _context.Entry(jogszabaly).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return jogszabaly;
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