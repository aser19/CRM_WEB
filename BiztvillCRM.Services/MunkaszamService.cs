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

    public async Task<string> GeneralKovetkezoMunkaszamAsync(int cegId)
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

            return $"HK-{szamlalo.UtolsoSorszam:D6}/{aktualisEv}";
        }
        finally
        {
            _lock.Release();
        }
    }
}