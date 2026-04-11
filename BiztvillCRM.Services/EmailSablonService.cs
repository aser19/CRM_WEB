using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class EmailSablonService : IEmailSablonService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;
    private readonly ITenantService _tenantService;

    // Mapping: EmailErtesitesTipus -> ModulJogosultsag
    private static readonly Dictionary<EmailErtesitesTipus, ModulJogosultsag> TipusModulMapping = new()
    {
        { EmailErtesitesTipus.HitelesitesLejarat90Nap, ModulJogosultsag.Hitelesitesek },
        { EmailErtesitesTipus.HitelesitesLejarat30Nap, ModulJogosultsag.Hitelesitesek },
        { EmailErtesitesTipus.MeresLejarat90Nap, ModulJogosultsag.Meresek },
        { EmailErtesitesTipus.MeresLejarat30Nap, ModulJogosultsag.Meresek },
        { EmailErtesitesTipus.KockazatFelulvizsgalat90Nap, ModulJogosultsag.Kockazatertekelesek },
        { EmailErtesitesTipus.KockazatFelulvizsgalat30Nap, ModulJogosultsag.Kockazatertekelesek },
        { EmailErtesitesTipus.MunkavedelmiOktatas90Nap, ModulJogosultsag.MunkavedelmiOktatasok },
        { EmailErtesitesTipus.MunkavedelmiOktatas30Nap, ModulJogosultsag.MunkavedelmiOktatasok },
        { EmailErtesitesTipus.ZonaterkepLejarat90Nap, ModulJogosultsag.Zonaterkepek },
        { EmailErtesitesTipus.ZonaterkepLejarat30Nap, ModulJogosultsag.Zonaterkepek },
    };

    public EmailSablonService(IDbContextFactory<CrmDbContext> contextFactory, ITenantService tenantService)
    {
        _contextFactory = contextFactory;
        _tenantService = tenantService;
    }

    public async Task<List<EmailSablon>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var cegId = _tenantService.GetCurrentCegId();
        
        // Admin látja az összeset, egyébként csak a globális + saját cég sablonjait
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
        {
            return await context.EmailSablonok
                .Include(s => s.Ceg)
                .OrderBy(s => s.Tipus)
                .ThenBy(s => s.CegId)
                .ToListAsync();
        }

        return await context.EmailSablonok
            .Where(s => s.CegId == null || s.CegId == cegId)
            .OrderBy(s => s.Tipus)
            .ToListAsync();
    }

    public async Task<List<EmailSablon>> GetAllFilteredByModulokAsync(ModulJogosultsag aktivModulok)
    {
        var sablonok = await GetAllAsync();
        
        // Szűrés: csak azok a sablonok, amelyek modulja aktív vagy nincs modulhoz kötve (pl. SmtpTeszt)
        return sablonok
            .Where(s => IsModulAktiv(s.Tipus, aktivModulok))
            .ToList();
    }

    private static bool IsModulAktiv(EmailErtesitesTipus tipus, ModulJogosultsag aktivModulok)
    {
        // Ha nincs mapping (pl. SmtpTeszt), akkor mindenki láthatja
        if (!TipusModulMapping.TryGetValue(tipus, out var szuksegesModul))
        {
            return true;
        }

        return aktivModulok.HasFlag(szuksegesModul);
    }

    public async Task<EmailSablon?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.EmailSablonok.FindAsync(id);
    }

    public async Task<EmailSablon?> GetByTipusAsync(EmailErtesitesTipus tipus, int? cegId = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var sablon = await context.EmailSablonok
            .FirstOrDefaultAsync(s => s.Tipus == tipus && s.CegId == cegId && s.Aktiv);

        if (sablon is null && cegId.HasValue)
        {
            sablon = await context.EmailSablonok
                .FirstOrDefaultAsync(s => s.Tipus == tipus && s.CegId == null && s.Aktiv);
        }

        return sablon;
    }

    public async Task<EmailSablon> CreateAsync(EmailSablon sablon)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        sablon.Ceg = null;
        sablon.Letrehozva = DateTime.UtcNow;
        
        context.EmailSablonok.Add(sablon);
        await context.SaveChangesAsync();
        return sablon;
    }

    public async Task<EmailSablon> UpdateAsync(EmailSablon sablon)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existing = await context.EmailSablonok.FindAsync(sablon.Id)
            ?? throw new InvalidOperationException("Sablon nem található.");

        existing.Nev = sablon.Nev;
        existing.Tipus = sablon.Tipus;
        existing.Targy = sablon.Targy;
        existing.Szoveg = sablon.Szoveg;
        existing.Aktiv = sablon.Aktiv;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var sablon = await context.EmailSablonok.FindAsync(id);
        if (sablon is not null)
        {
            context.EmailSablonok.Remove(sablon);
            await context.SaveChangesAsync();
        }
    }
}