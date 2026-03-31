using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class CegService : ICegService
{
    private readonly CrmDbContext _context;

    public CegService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ceg>> GetAllAsync()
    {
        return await _context.Cegek.Where(c => c.Aktiv).OrderBy(c => c.Nev).ToListAsync();
    }
}