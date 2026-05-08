using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class AvkVedelemTipusService : IAvkVedelemTipusService
{
    private readonly IDbContextFactory<CrmDbContext> _dbFactory;

    public AvkVedelemTipusService(IDbContextFactory<CrmDbContext> dbFactory)
        => _dbFactory = dbFactory;

    public async Task<List<AvkVedelemTipus>> GetAktivTipusokAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AvkVedelemTipusok
            .Where(t => t.Aktiv)
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<AvkVedelemTipus?> GetByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AvkVedelemTipusok.FindAsync(id);
    }

    public async Task MentesAsync(AvkVedelemTipus tipus)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        if (tipus.Id == 0)
            db.AvkVedelemTipusok.Add(tipus);
        else
            db.AvkVedelemTipusok.Update(tipus);
        await db.SaveChangesAsync();
    }

    public async Task TorlesAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var t = await db.AvkVedelemTipusok.FindAsync(id);
        if (t is not null)
        {
            t.Aktiv = false;
            await db.SaveChangesAsync();
        }
    }
}