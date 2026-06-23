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

    public async Task<AvkVedelemTipus?> GetByNevAsync(string nev)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AvkVedelemTipusok.FirstOrDefaultAsync(t => t.Nev == nev);
    }

    public async Task<AvkVedelemTipus> GetOrCreateAsync(string tipusNev)
    {
        if (string.IsNullOrWhiteSpace(tipusNev))
            throw new ArgumentException("A típus neve nem lehet üres!", nameof(tipusNev));

        var meglevo = await GetByNevAsync(tipusNev);
        if (meglevo != null) return meglevo;

        var ujTipus = new AvkVedelemTipus
        {
            Nev = tipusNev,
            TipusKod = "AC",
            In = KiolvasNevlegesAramot(tipusNev),
            IDn = KiolvasIDn(tipusNev),
            Un = 230,
            Polusszam = 2,
            Aktiv = true,
            FelulvizsgalasraVar = true,
            Leiras = "Automatikusan létrehozott típus - felülvizsgálatra vár",
            Letrehozva = DateTime.Now
        };

        await using var db = await _dbFactory.CreateDbContextAsync();
        db.AvkVedelemTipusok.Add(ujTipus);
        await db.SaveChangesAsync();
        return ujTipus;
    }

    private static decimal KiolvasNevlegesAramot(string tipusNev)
    {
        var match = System.Text.RegularExpressions.Regex.Match(tipusNev, @"[AaBbCcDd]?(\d+(?:\.\d+)?)");
        return match.Success && decimal.TryParse(match.Groups[1].Value, 
            System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, out var aram) ? aram : 25m;
    }

    private static decimal KiolvasIDn(string tipusNev)
    {
        var match = System.Text.RegularExpressions.Regex.Match(tipusNev, @"[/\-_](\d+(?:\.\d+)?)\s*(?:mA)?");
        if (match.Success && decimal.TryParse(match.Groups[1].Value,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var idn))
        {
            return idn < 1 ? idn * 1000 : idn; // 0.03 → 30mA
        }
        return 30m;
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