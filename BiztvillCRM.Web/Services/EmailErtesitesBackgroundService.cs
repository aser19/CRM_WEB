using BiztvillCRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Web.Services;

/// <summary>
/// Háttérszolgáltatás, amely naponta egyszer ellenőrzi a lejáró hitelesítéseket/méréseket
/// és automatikusan küldi az értesítő emaileket.
/// </summary>
public class EmailErtesitesBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailErtesitesBackgroundService> _logger;
    private readonly TimeSpan _futasiIdo;
    private readonly TimeSpan _ellenorzesiIdoKoz = TimeSpan.FromMinutes(10); // ✅ 10 percenként ellenőriz

    public EmailErtesitesBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmailErtesitesBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var ora = configuration.GetValue<int>("EmailErtesites:FutasiIdoOra", 6);
        var perc = configuration.GetValue<int>("EmailErtesites:FutasiIdoPerc", 0);
        _futasiIdo = new TimeSpan(ora, perc, 0);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email értesítés háttérszolgáltatás elindult.");
        _logger.LogInformation("Beállított futási idő: {FutasiIdo}, Ellenőrzési időköz: {Idokoz}", 
            _futasiIdo, _ellenorzesiIdoKoz);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var most = DateTime.Now;
                
                // ✅ Ellenőrizzük, hogy futási időben vagyunk-e (± 10 perc ablak)
                if (FutasiIdobenVagyunk(most))
                {
                    var utolsoFutas = await GetUtolsoSikeresFutasAsync();
                    
                    // ✅ Csak akkor futtatjuk, ha ma még nem futott le
                    if (utolsoFutas == null || utolsoFutas.Value.Date < DateTime.Today)
                    {
                        _logger.LogInformation("Email értesítés feldolgozás indítása: {Ido}", most);
                        await VegrehajtFeldolgozastAsync(stoppingToken);
                    }
                    else
                    {
                        _logger.LogDebug("Ma már lefutott az email értesítés: {UtolsoFutas}", utolsoFutas);
                    }
                }
                
                // ✅ Rövid várakozás a következő ellenőrzésig (10 perc)
                await Task.Delay(_ellenorzesiIdoKoz, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Email értesítés háttérszolgáltatás leállítása folyamatban...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba az email értesítés háttérszolgáltatásban.");
                // ✅ Hiba esetén is folytatjuk, ne álljon le a szolgáltatás
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Email értesítés háttérszolgáltatás leállt.");
    }

    /// <summary>Ellenőrzi, hogy a jelenlegi idő a futási időn belül van-e (± 10 perc).</summary>
    private bool FutasiIdobenVagyunk(DateTime most)
    {
        var mostIdo = most.TimeOfDay;
        var tolerancia = TimeSpan.FromMinutes(10);
        
        var kezdet = _futasiIdo - tolerancia;
        var veg = _futasiIdo + tolerancia;
        
        return mostIdo >= kezdet && mostIdo <= veg;
    }

    /// <summary>Lekéri az utolsó sikeres futás időpontját.</summary>
    private async Task<DateTime?> GetUtolsoSikeresFutasAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BiztvillCRM.Data.CrmDbContext>();
            
            // ✅ Lekérjük a legutóbbi sikeres email küldés időpontját
            var utolsoEmail = await context.EmailKuldesNaplok
                .Where(e => e.Sikeres)
                .OrderByDescending(e => e.Kuldve)
                .Select(e => e.Kuldve)
                .FirstOrDefaultAsync();
            
            return utolsoEmail;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nem sikerült lekérni az utolsó futás időpontját.");
            return null;
        }
    }

    private async Task VegrehajtFeldolgozastAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email értesítés feldolgozás indítása...");

        try
        {
            // Scope létrehozása a scoped szolgáltatások eléréséhez
            using var scope = _serviceProvider.CreateScope();
            var ertesitoService = scope.ServiceProvider.GetRequiredService<ILejaratErtesitoService>();

            var eredmeny = await ertesitoService.FeldolgozasAsync();

            _logger.LogInformation(
                "Email értesítés feldolgozás kész. " +
                "Hitelesítések: {Hitelesitesek}, Mérések: {Meresek}, " +
                "Küldött: {Kuldott}, Sikertelen: {Sikertelen}",
                eredmeny.FeldolgozottHitelesitesek,
                eredmeny.FeldolgozottMeresek,
                eredmeny.KuldottEmailek,
                eredmeny.SikertelenEmailek);

            if (eredmeny.Hibak.Count > 0)
            {
                foreach (var hiba in eredmeny.Hibak)
                {
                    _logger.LogWarning("Feldolgozási hiba: {Hiba}", hiba);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kritikus hiba az email értesítés feldolgozás közben.");
        }
    }
}