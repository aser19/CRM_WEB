using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TerminalService : ITerminalService
{
    private readonly CrmDbContext _context;
    public TerminalService(CrmDbContext context) => _context = context;

    public async Task<List<Terminal>> GetAllAsync() =>
        await _context.Terminalok.Include(t => t.Telephely).OrderBy(t => t.Nev).ToListAsync();

    public async Task<Terminal?> GetByIdAsync(int id) =>
        await _context.Terminalok.Include(t => t.Telephely).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Terminal> CreateAsync(Terminal terminal)
    {
        terminal.Letrehozva = DateTime.UtcNow;
        _context.Terminalok.Add(terminal);
        await _context.SaveChangesAsync();
        return terminal;
    }

    public async Task<Terminal> UpdateAsync(Terminal terminal)
    {
        terminal.Modositva = DateTime.UtcNow;
        _context.Entry(terminal).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return terminal;
    }

    public async Task DeleteAsync(int id)
    {
        var terminal = await _context.Terminalok.FindAsync(id);
        if (terminal is not null)
        {
            _context.Terminalok.Remove(terminal);
            await _context.SaveChangesAsync();
        }
    }
}