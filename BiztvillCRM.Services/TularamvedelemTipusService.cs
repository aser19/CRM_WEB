using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class TularamvedelemTipusService : ITularamvedelemTipusService
{
    private readonly CrmDbContext _context;

    public TularamvedelemTipusService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<TularamvedelemTipus>> GetAllAsync()
    {
        return await _context.TularamvedelemTipusok
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<List<TularamvedelemTipus>> GetAllActiveAsync()
    {
        return await _context.TularamvedelemTipusok
            .Where(t => t.Aktiv)
            .OrderBy(t => t.Nev)
            .ToListAsync();
    }

    public async Task<TularamvedelemTipus?> GetByIdAsync(int id)
    {
        return await _context.TularamvedelemTipusok.FindAsync(id);
    }

    public async Task<TularamvedelemTipus?> GetByNevAsync(string nev)
    {
        return await _context.TularamvedelemTipusok
            .FirstOrDefaultAsync(t => t.Nev == nev);
    }

    public async Task<TularamvedelemTipus> GetOrCreateAsync(string tipusNev)
    {
        if (string.IsNullOrWhiteSpace(tipusNev))
        {
            throw new ArgumentException("A típus neve nem lehet üres!", nameof(tipusNev));
        }

        // Megpróbáljuk megtalálni a meglévő típust
        var meglevo = await GetByNevAsync(tipusNev);
        if (meglevo != null)
        {
            return meglevo;
        }

        // Ha nem létezik, automatikusan létrehozzuk
        var ujTipus = new TularamvedelemTipus
        {
            Nev = tipusNev,
            NevlegesAram = KiolvasNevlegesAramot(tipusNev),
            Aktiv = true,
            FelulvizsgalasraVar = true,
            Leiras = "Automatikusan létrehozott típus - felülvizsgálatra vár",
            Letrehozva = DateTime.Now
        };

        _context.TularamvedelemTipusok.Add(ujTipus);
        await _context.SaveChangesAsync();

        return ujTipus;
    }

    /// <summary>
    /// Megpróbálja kiolvasni a névleges áramot a típus nevéből (pl. "C16" → 16, "B20" → 20)
    /// </summary>
    private static decimal KiolvasNevlegesAramot(string tipusNev)
    {
        if (string.IsNullOrWhiteSpace(tipusNev))
            return 16m; // Alapértelmezett 16A

        // Keres olyan mintát, ahol karakterisztika betű (A/B/C/D) után következik szám
        var match = System.Text.RegularExpressions.Regex.Match(
            tipusNev, 
            @"[AaBbCcDd](\d+(?:\.\d+)?)",
            System.Text.RegularExpressions.RegexOptions.None);

        if (match.Success && decimal.TryParse(
            match.Groups[1].Value, 
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, 
            out var aram))
        {
            return aram;
        }

        // Ha nem találtunk karakterisztika + áram kombinációt, keresünk csak számot
        var numMatch = System.Text.RegularExpressions.Regex.Match(
            tipusNev, 
            @"(\d+(?:\.\d+)?)",
            System.Text.RegularExpressions.RegexOptions.None);

        if (numMatch.Success && decimal.TryParse(
            numMatch.Groups[1].Value,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var aram2))
        {
            return aram2;
        }

        return 16m; // Alapértelmezett
    }

    public async Task<int> CreateAsync(TularamvedelemTipus tipus)
    {
        // Egyediség ellenőrzés
        if (await _context.TularamvedelemTipusok.AnyAsync(t => t.Nev == tipus.Nev))
        {
            throw new InvalidOperationException($"A '{tipus.Nev}' típus már létezik!");
        }

        tipus.Letrehozva = DateTime.Now;
        _context.TularamvedelemTipusok.Add(tipus);
        await _context.SaveChangesAsync();
        return tipus.Id;
    }

    public async Task UpdateAsync(TularamvedelemTipus tipus)
    {
        // Egyediség ellenőrzés (kivéve saját magát)
        if (await _context.TularamvedelemTipusok
            .AnyAsync(t => t.Nev == tipus.Nev && t.Id != tipus.Id))
        {
            throw new InvalidOperationException($"A '{tipus.Nev}' típus már létezik!");
        }

        _context.TularamvedelemTipusok.Update(tipus);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tipus = await _context.TularamvedelemTipusok.FindAsync(id);
        if (tipus != null)
        {
            _context.TularamvedelemTipusok.Remove(tipus);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> SzamolMinositestAsync(string tipusNev, decimal mertHurokimpedancia)
    {
        var tipus = await GetByNevAsync(tipusNev);
        if (tipus == null)
            return "MEGFELELT"; // Alapértelmezett, ha nincs típus megadva

        // Zs ≤ Zs_max → MEGFELELT
        var zsMax = tipus.MaxHurokimpedancia;
        if (zsMax <= 0) return "MEGFELELT";

        return mertHurokimpedancia <= zsMax ? "MEGFELELT" : "NEM FELELT MEG";
    }
}