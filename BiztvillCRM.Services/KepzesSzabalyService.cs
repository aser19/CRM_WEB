// BiztvillCRM.Services/KepzesSzabalyService.cs
using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class KepzesSzabalyService(CrmDbContext db) : IKepzesSzabalyService
{
    public async Task<List<KepzesSzabaly>> GetAllAsync() =>
        await db.KepzesSzabalyok
            .Include(s => s.ForrasKepzesTipus)
            .Include(s => s.CelKepzesTipus)
            .OrderBy(s => s.Tipus)
            .ThenBy(s => s.ForrasKepzesTipus!.Nev)
            .ToListAsync();

    public async Task<List<KepzesSzabaly>> GetByTipusAsync(KepzesSzabalyTipus tipus) =>
        await db.KepzesSzabalyok
            .Include(s => s.ForrasKepzesTipus)
            .Include(s => s.CelKepzesTipus)
            .Where(s => s.Tipus == tipus && s.Aktiv)
            .ToListAsync();

    public async Task<List<KepzesSzabaly>> GetByKepzesTipusIdAsync(int kepzesTipusId) =>
        await db.KepzesSzabalyok
            .Include(s => s.ForrasKepzesTipus)
            .Include(s => s.CelKepzesTipus)
            .Where(s => s.ForrasKepzesTipusId == kepzesTipusId || s.CelKepzesTipusId == kepzesTipusId)
            .ToListAsync();

    public async Task<KepzesSzabaly?> GetByIdAsync(int id) =>
        await db.KepzesSzabalyok
            .Include(s => s.ForrasKepzesTipus)
            .Include(s => s.CelKepzesTipus)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<KepzesSzabaly> CreateAsync(KepzesSzabaly szabaly)
    {
        szabaly.Letrehozva = DateTime.UtcNow;
        db.KepzesSzabalyok.Add(szabaly);
        await db.SaveChangesAsync();
        return szabaly;
    }

    public async Task UpdateAsync(KepzesSzabaly szabaly)
    {
        db.KepzesSzabalyok.Update(szabaly);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var szabaly = await db.KepzesSzabalyok.FindAsync(id);
        if (szabaly is not null)
        {
            db.KepzesSzabalyok.Remove(szabaly);
            await db.SaveChangesAsync();
        }
    }

    // === ÜZLETI LOGIKA ===

    public async Task<List<int>> GetMegujitandoKepzesTipusIdkAsync(int tovabbkepzesTipusId)
    {
        return await db.KepzesSzabalyok
            .Where(s => s.Tipus == KepzesSzabalyTipus.Megujit 
                     && s.ForrasKepzesTipusId == tovabbkepzesTipusId 
                     && s.Aktiv)
            .Select(s => s.CelKepzesTipusId)
            .ToListAsync();
    }

    public async Task<bool> FelmentettETovabbkepzesAlolAsync(
        int kepzesTipusId, 
        IEnumerable<int> felulvizsgaloOsszesKepzesTipusai)
    {
        var felmentoKepzesIds = await db.KepzesSzabalyok
            .Where(s => s.Tipus == KepzesSzabalyTipus.Felment 
                     && s.CelKepzesTipusId == kepzesTipusId 
                     && s.Aktiv)
            .Select(s => s.ForrasKepzesTipusId)
            .ToListAsync();
        
        // Ha van olyan képzése, ami felment
        return felmentoKepzesIds.Any(fkId => felulvizsgaloOsszesKepzesTipusai.Contains(fkId));
    }

    public async Task<(bool Engedelyezett, List<string> HianyzoElofeltetelek)> EllenorizElofelteteelketAsync(
        int kepzesTipusId, 
        IEnumerable<int> felulvizsgaloMegleVoKepzesTipusai)
    {
        var elofeltetelek = await db.KepzesSzabalyok
            .Include(s => s.ForrasKepzesTipus)
            .Where(s => s.Tipus == KepzesSzabalyTipus.Elofeltetel 
                     && s.CelKepzesTipusId == kepzesTipusId 
                     && s.Aktiv)
            .ToListAsync();
        
        if (!elofeltetelek.Any())
            return (true, []);
        
        var hianyzo = elofeltetelek
            .Where(e => !felulvizsgaloMegleVoKepzesTipusai.Contains(e.ForrasKepzesTipusId))
            .Select(e => e.ForrasKepzesTipus!.Nev)
            .ToList();
        
        return (hianyzo.Count == 0, hianyzo);
    }

    public async Task<List<LejaroKepzesInfo>> GetLejaroKepzesekAsync(int cegId, int naponBelul = 90)
    {
        var hatar = DateTime.Today.AddDays(naponBelul);
        var ma = DateTime.Today;
        
        var kepzesek = await db.Kepzesek
            .Include(k => k.KepzesTipus)
            .Where(k => k.CegId == cegId 
                     && k.Aktiv 
                     && k.KepzesTipus != null
                     && k.KepzesTipus.TovabbkepzesKotelezo
                     && k.UtolsoTovabbkepzes.HasValue)
            .ToListAsync();
        
        var eredmeny = new List<LejaroKepzesInfo>();
        
        foreach (var k in kepzesek)
        {
            if (k.KepzesTipus?.TovabbkepzesEvek is null) continue;
            
            var kovetkezoTkp = k.UtolsoTovabbkepzes!.Value.AddYears(k.KepzesTipus.TovabbkepzesEvek.Value);
            if (kovetkezoTkp > hatar) continue;
            
            // Ellenőrizzük, felmentett-e
            var felulvizsgaloKepzesTipusai = await db.Kepzesek
                .Where(fk => fk.Nev == k.Nev && fk.CegId == cegId && fk.Aktiv && fk.KepzesTipusId.HasValue)
                .Select(fk => fk.KepzesTipusId!.Value)
                .ToListAsync();
            
            var felmentett = await FelmentettETovabbkepzesAlolAsync(k.KepzesTipusId!.Value, felulvizsgaloKepzesTipusai);
            
            eredmeny.Add(new LejaroKepzesInfo(
                k.Id,
                k.Nev,
                k.KepzesTipus.Nev,
                kovetkezoTkp,
                (kovetkezoTkp - ma).Days,
                felmentett));
        }
        
        return eredmeny.OrderBy(e => e.LejaratDatum).ToList();
    }
}