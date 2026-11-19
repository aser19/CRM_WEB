using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class LyukadasjelzoSzamService : ILyukadasjelzoSzamService
{
    private readonly CrmDbContext _context;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public LyukadasjelzoSzamService(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<string> GeneralKovetkezoSorszamAsync(int cegId)
    {
        await _lock.WaitAsync();
        try
        {
            var aktualisEv = DateTime.Now.Year;

            var szamlalo = await _context.LyukadasjelzoJegyzokonyvSzamlalok
                .FirstOrDefaultAsync(s => s.CegId == cegId && s.Ev == aktualisEv);

            if (szamlalo == null)
            {
                szamlalo = new LyukadasjelzoJegyzokonyvSzamlalo
                {
                    CegId = cegId,
                    Ev = aktualisEv,
                    UtolsoSorszam = 0
                };
                _context.LyukadasjelzoJegyzokonyvSzamlalok.Add(szamlalo);
            }

            szamlalo.UtolsoSorszam++;
            await _context.SaveChangesAsync();

            return $"LYUK-{szamlalo.UtolsoSorszam:D6}/{aktualisEv}";
        }
        finally
        {
            _lock.Release();
        }
    }
}
