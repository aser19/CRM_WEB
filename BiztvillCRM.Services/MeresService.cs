using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BiztvillCRM.Services;

public class MeresService : IMeresService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    public MeresService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<Meres>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var mellekletMeresIds = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Where(m => !mellekletMeresIds.Contains(m.Id) && m.Aktiv) // Csak az aktívak
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<List<Meres>> GetInaktivakAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var mellekletMeresIds = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId.HasValue)
            .Select(m => m.MellekletMeresId!.Value)
            .ToListAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Where(m => !mellekletMeresIds.Contains(m.Id) && !m.Aktiv) // Csak az inaktívak
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.OrderByDescending(m => m.Datum).ToListAsync();
    }

    public async Task<Meres?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .AsQueryable();

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            query = query.Where(m => m.Ugyfel != null && cegIds.Contains(m.Ugyfel.CegId));
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Meres> CreateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ugyfel = await context.Ugyfelek.FirstOrDefaultAsync(u => u.Id == meres.UgyfelId);

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (ugyfel == null || !cegIds.Contains(ugyfel.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága mérés létrehozásához ennél az ügyfélnél.");
        }

        var telephely = await context.Telephelyek.FirstOrDefaultAsync(t => t.Id == meres.TelephelyId);
        if (telephely?.UgyfelId != meres.UgyfelId)
            throw new InvalidOperationException("A telephely nem tartozik a kiválasztott ügyfélhez.");

        meres.Ugyfel = null!;
        meres.Telephely = null!;
        meres.MeresTipus = null!;
        meres.Letrehozva = DateTime.UtcNow;

        context.Meresek.Add(meres);
        await context.SaveChangesAsync();
        return meres;
    }

    public async Task<Meres> UpdateAsync(Meres meres)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == meres.Id)
            ?? throw new InvalidOperationException("Nem található.");

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(existing.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés módosításához.");
        }

        existing.UgyfelId = meres.UgyfelId;
        existing.TelephelyId = meres.TelephelyId;
        existing.MeresTipusId = meres.MeresTipusId;
        existing.Datum = meres.Datum;
        existing.KovetkezoDatum = meres.KovetkezoDatum;
        existing.Eredmeny = meres.Eredmeny;
        existing.MeresStatusz = meres.MeresStatusz;
        existing.Megjegyzes = meres.Megjegyzes;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek
            .Include(m => m.Ugyfel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meres is null) return;

        if (!_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            var cegIds = await _tenantService.GetElerhhetoCegIdsAsync();
            if (!cegIds.Contains(meres.Ugyfel!.CegId))
                throw new UnauthorizedAccessException("Nincs jogosultsága a mérés törléséhez.");
        }

        var mellekletHivatkozas = await context.MellekletJegyzokonyvek
            .Where(m => m.MellekletMeresId == id)
            .ToListAsync();
        foreach (var m in mellekletHivatkozas)
        {
            m.MellekletMeresId = null;
            m.Modositva = DateTime.UtcNow;
        }

        var mellekletek = await context.MellekletJegyzokonyvek
            .Where(m => m.MeresId == id)
            .ToListAsync();
        context.MellekletJegyzokonyvek.RemoveRange(mellekletek);

        context.Meresek.Remove(meres);
        await context.SaveChangesAsync();
    }

    public async Task<JegyzokonyvAdatok?> BetoltJegyzokonyvAdatokAsync(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meres = await context.Meresek.FindAsync(meresId);
        if (meres == null || string.IsNullOrEmpty(meres.JegyzokonyvAdatokJson))
            return null;

        return JsonSerializer.Deserialize<JegyzokonyvAdatok>(meres.JegyzokonyvAdatokJson);
    }

    public async Task MentesJegyzokonyvAdatokkalAsync(int meresId, JegyzokonyvAdatok adatok)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.JegyzokonyvAdatokJson = JsonSerializer.Serialize(adatok);
            await context.SaveChangesAsync();
        }
    }

    public async Task StatuszFrissitesAsync(int meresId, MeresStatusz statusz, string? eredmeny)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.MeresStatusz = statusz;
            meres.Eredmeny = eredmeny;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task MentesJegyzokonyvAdatokkalEsStatuszAsync(
        int meresId, JegyzokonyvAdatok adatok, MeresStatusz statusz, string? eredmeny)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.JegyzokonyvAdatokJson = JsonSerializer.Serialize(adatok);
            meres.MeresStatusz = statusz;
            meres.Eredmeny = eredmeny;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<Meres?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int meresTipusId, DateTime ujMeresDatum)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Meresek
            .Include(m => m.Ugyfel)
            .Include(m => m.Telephely)
            .Include(m => m.MeresTipus)
            .Where(m => m.UgyfelId == ugyfelId
                        && m.TelephelyId == telephelyId
                        && m.MeresTipusId == meresTipusId
                        && m.Aktiv);

        var regi = await query.FirstOrDefaultAsync();
        if (regi == null) return null;

        // Ellenőrizzük, hogy az új mérés 40 napon belül van-e a régihez képest
        if (regi.KovetkezoDatum.HasValue)
        {
            var kulonbseg = Math.Abs((ujMeresDatum - regi.KovetkezoDatum.Value).TotalDays);
            if (kulonbseg <= 40)
            {
                return regi;
            }
        }

        return null;
    }

    public async Task InaktivvaTesz(int meresId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var meres = await context.Meresek.FindAsync(meresId);
        if (meres != null)
        {
            meres.Aktiv = false;
            meres.Modositva = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}
