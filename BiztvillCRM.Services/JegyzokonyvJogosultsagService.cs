using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class JegyzokonyvJogosultsagService : IJegyzokonyvJogosultsagService
{
    private readonly CrmDbContext _db;
    private readonly ITenantService _tenantService;

    public JegyzokonyvJogosultsagService(CrmDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<List<Felulvizsgalo>> GetJogosultFelulvizsgalokAsync(int meresTipusId)
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        return await _db.Felulvizsgalok
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.KepzesTipus)
            .Include(f => f.Kepzesek)
                .ThenInclude(k => k.Tovabbkepzesek)
            .Where(f => f.CegId == cegId && f.Aktiv)
            .OrderBy(f => f.Nev)
            .ToListAsync();
    }

    public async Task<(bool VanJogosult, List<string> HianyzoKepzesek)> EllenorizJogosultsagotAsync(int meresTipusId)
    {
        var cegId = _tenantService.GetCurrentCegId();
        
        // 1. Lekérjük a mérés típushoz szükséges képzés követelményeket
        var kovetelemények = await _db.MeresTipusKepzesKovetelemenyei
            .Include(k => k.KepzesTipus)
            .Where(k => k.MeresTipusId == meresTipusId && k.Kotelezo)
            .ToListAsync();
        
        if (kovetelemények.Count == 0)
        {
            // Ha nincs követelmény, bárki lehet
            var vanBarkiFelulvizsgalo = await _db.Felulvizsgalok
                .AnyAsync(f => f.CegId == cegId && f.Aktiv && f.LehetFelelosFelulvizsgalo);
            
            return (vanBarkiFelulvizsgalo, []);
        }
        
        // 2. Lekérjük a cég felülvizsgálóit képzéseikkel
        var felulvizsgalok = await _db.Felulvizsgalok
            .Where(f => f.CegId == cegId && f.Aktiv)
            .ToListAsync();

        // Memóriában szűrjük a számított property-re
        var jogosultFelulvizsgalok = felulvizsgalok
            .Where(f => f.LehetFelelosFelulvizsgalo)
            .ToList();
        
        // 3. Ellenőrizzük, hogy van-e olyan felülvizsgáló, aki teljesíti a követelményeket
        var hianyzoKepzesek = new List<string>();
        
        foreach (var felulvizsgalo in jogosultFelulvizsgalok)
        {
            var meglevőKepzesIds = felulvizsgalo.Kepzesek
                .Where(k => k.Aktiv)
                .Select(k => k.KepzesTipusId)
                .ToHashSet();
            
            // Csoportosítjuk alternatíva csoportok szerint
            var csoportok = kovetelemények.GroupBy(k => k.AlternativaCsoport);
            var teljesiti = true;
            
            foreach (var csoport in csoportok)
            {
                if (csoport.Key == 0)
                {
                    // Kötelező képzések (mindegyik kell)
                    foreach (var kov in csoport)
                    {
                        if (!meglevőKepzesIds.Contains(kov.KepzesTipusId))
                        {
                            teljesiti = false;
                            var kepzesNev = kov.KepzesTipus?.Label ?? kov.KepzesTipus?.Nev ?? "Ismeretlen";
                            if (!hianyzoKepzesek.Contains(kepzesNev))
                                hianyzoKepzesek.Add(kepzesNev);
                        }
                    }
                }
                else
                {
                    // Alternatív képzések (legalább egy kell a csoportból)
                    var vanValamelyik = csoport.Any(k => meglevőKepzesIds.Contains(k.KepzesTipusId));
                    if (!vanValamelyik)
                    {
                        teljesiti = false;
                        var alternativak = string.Join(" VAGY ", 
                            csoport.Select(k => k.KepzesTipus?.Label ?? k.KepzesTipus?.Nev ?? "?"));
                        if (!hianyzoKepzesek.Contains(alternativak))
                            hianyzoKepzesek.Add(alternativak);
                    }
                }
            }
            
            if (teljesiti)
                return (true, []); // Van legalább egy jogosult
        }
        
        return (false, hianyzoKepzesek);
    }

    public async Task<string> GeneralBizonyitvanySzovegetAsync(int meresTipusId, int felulvizsgaloId)
    {
        var kepzesek = await _db.FelulvizsgaloKepzesek
            .Include(k => k.KepzesTipus)
            .Where(k => k.FelulvizsgaloId == felulvizsgaloId && k.Aktiv)
            .OrderBy(k => k.KepzesTipus!.Nev)
            .ToListAsync();

        var parts = new List<string>();
        foreach (var kepzes in kepzesek)
        {
            if (!string.IsNullOrEmpty(kepzes.BizonyitvanySzam))
            {
                var label = kepzes.KepzesTipus?.Label ?? kepzes.KepzesTipus?.Nev ?? "";
                parts.Add($"{label}: {kepzes.BizonyitvanySzam}");
            }
        }
        
        return string.Join(", ", parts);
    }

    public async Task<string> GeneralTovabbkepzesSzovegetAsync(int meresTipusId, int felulvizsgaloId)
    {
        var kepzesek = await _db.FelulvizsgaloKepzesek
            .Include(k => k.KepzesTipus)
            .Include(k => k.Tovabbkepzesek)
            .Where(k => k.FelulvizsgaloId == felulvizsgaloId && k.Aktiv)
            .OrderBy(k => k.KepzesTipus!.Nev)
            .ToListAsync();

        var parts = new List<string>();
        foreach (var kepzes in kepzesek)
        {
            var utolsoTkp = kepzes.Tovabbkepzesek.OrderByDescending(t => t.Datum).FirstOrDefault();
            if (utolsoTkp != null && !string.IsNullOrEmpty(utolsoTkp.BizonyitvanySzam))
            {
                var label = kepzes.KepzesTipus?.Label ?? kepzes.KepzesTipus?.Nev ?? "";
                parts.Add($"{label}: {utolsoTkp.BizonyitvanySzam}");
            }
        }
        
        return string.Join(", ", parts);
    }
}