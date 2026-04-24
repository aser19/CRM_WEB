using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class MunkaszamService : IMunkaszamService
{
    private readonly CrmDbContext _context;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public MunkaszamService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<string> GeneralKovetkezoMunkaszamAsync(int cegId, int meresTipusId = 0)
    {
        await _lock.WaitAsync();
        try
        {
            var aktualisEv = DateTime.Now.Year;

            var szamlalo = await _context.MunkaszamSzamlalok
                .FirstOrDefaultAsync(s => s.CegId == cegId && s.Ev == aktualisEv);

            if (szamlalo == null)
            {
                szamlalo = new MunkaszamSzamlalo
                {
                    CegId = cegId,
                    Ev = aktualisEv,
                    UtolsoSorszam = 0
                };
                _context.MunkaszamSzamlalok.Add(szamlalo);
            }

            szamlalo.UtolsoSorszam++;
            await _context.SaveChangesAsync();

            // ✅ JAVÍTÁS: Prefix lekérése MeresTipus alapján
            string prefix = "HK"; // Alapértelmezett fallback
            
            if (meresTipusId > 0)
            {
                var meresTipus = await _context.MeresTipusok
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == meresTipusId);
                
                if (meresTipus != null && !string.IsNullOrWhiteSpace(meresTipus.JegyzokonyvPrefix))
                {
                    prefix = meresTipus.JegyzokonyvPrefix;
                }
            }

            return $"{prefix}-{szamlalo.UtolsoSorszam:D6}/{aktualisEv}";
        }
        finally
        {
            _lock.Release();
        }
    }
}