using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TerminalService : ITerminalService
{
    private readonly CrmDbContext _context;
    private readonly ITenantService _tenantService;

    public TerminalService(CrmDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<Terminal>> GetAllAsync()
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Terminalok
            .Include(t => t.Telephely)
                .ThenInclude(tp => tp.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(t => t.Telephely.Ugyfel.CegId == cegId);
        }

        return await query.OrderBy(t => t.Nev).ToListAsync();
    }

    public async Task<Terminal?> GetByIdAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var query = _context.Terminalok
            .Include(t => t.Telephely)
                .ThenInclude(tp => tp.Ugyfel)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            query = query.Where(t => t.Telephely.Ugyfel.CegId == cegId);
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Terminal> CreateAsync(Terminal terminal)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var telephely = await _context.Telephelyek.Include(t => t.Ugyfel).FirstOrDefaultAsync(t => t.Id == terminal.TelephelyId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && telephely?.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága terminál létrehozásához ennél a telephelynél.");
        }

        terminal.Letrehozva = DateTime.UtcNow;
        _context.Terminalok.Add(terminal);
        await _context.SaveChangesAsync();
        return terminal;
    }

    public async Task<Terminal> UpdateAsync(Terminal terminal)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var existing = await _context.Terminalok
            .Include(t => t.Telephely)
                .ThenInclude(tp => tp.Ugyfel)
            .FirstOrDefaultAsync(t => t.Id == terminal.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && existing.Telephely.Ugyfel.CegId != cegId)
        {
            throw new UnauthorizedAccessException("Nincs jogosultsága a terminál módosításához.");
        }

        existing.Nev = terminal.Nev;
        existing.Azonosito = terminal.Azonosito;
        existing.IpCim = terminal.IpCim;
        existing.TelephelyId = terminal.TelephelyId;
        existing.Megjegyzes = terminal.Megjegyzes;
        existing.Aktiv = terminal.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var cegId = _tenantService.GetCurrentCegId();
        var terminal = await _context.Terminalok
            .Include(t => t.Telephely)
                .ThenInclude(tp => tp.Ugyfel)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (terminal is not null)
        {
            if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin) && terminal.Telephely.Ugyfel.CegId != cegId)
            {
                throw new UnauthorizedAccessException("Nincs jogosultsága a terminál törléséhez.");
            }

            _context.Terminalok.Remove(terminal);
            await _context.SaveChangesAsync();
        }
    }
}