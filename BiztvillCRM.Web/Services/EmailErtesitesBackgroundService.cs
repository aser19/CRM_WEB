using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
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
    private readonly TimeSpan _ellenorzesiIdoKoz = TimeSpan.FromMinutes(10); // 10 percenként ellenőriz

    public EmailErtesitesBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmailErtesitesBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email értesítés háttérszolgáltatás elindult.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var futasiIdo = await GetFutasiIdoAsync();
                _logger.LogDebug("Beállított futási idő: {FutasiIdo}, Ellenőrzési időköz: {Idokoz}", 
                    futasiIdo, _ellenorzesiIdoKoz);

                var most = DateTime.Now;
                
                // Ellenőrizzük, hogy futási időben vagyunk-e (± 10 perc ablak)
                if (FutasiIdobenVagyunk(most, futasiIdo))
                {
                    var utolsoFutas = await GetUtolsoSikeresFutasAsync();
                    
                    // Csak akkor futtatjuk, ha ma még nem futott le
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
                
                // Rövid várakozás a következő ellenőrzésig (10 perc)
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
                // Hiba esetén is folytatjuk, ne álljon le a szolgáltatás
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Email értesítés háttérszolgáltatás leállt.");
    }

    /// <summary>Lekéri az adatbázisból a futási időt.</summary>
    private async Task<TimeSpan> GetFutasiIdoAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BiztvillCRM.Data.CrmDbContext>();
            
            var smtp = await context.SmtpBeallitasok.FirstOrDefaultAsync();
            if (smtp != null)
            {
                return new TimeSpan(smtp.EmailFutasiOra, smtp.EmailFutasiPerc, 0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nem sikerült lekérni a futási időt az adatbázisból, alapértelmezett: 06:00");
        }
        
        // Alapértelmezett: 06:00
        return new TimeSpan(6, 0, 0);
    }

    /// <summary>Ellenőrzi, hogy a jelenlegi idő a futási időn belül van-e (± 10 perc).</summary>
    private bool FutasiIdobenVagyunk(DateTime most, TimeSpan futasiIdo)
    {
        var mostIdo = most.TimeOfDay;
        var tolerancia = TimeSpan.FromMinutes(10);
        
        var kezdet = futasiIdo - tolerancia;
        var veg = futasiIdo + tolerancia;
        
        return mostIdo >= kezdet && mostIdo <= veg;
    }

    /// <summary>Lekéri az utolsó sikeres AUTOMATIKUS értesítés időpontját.</summary>
    private async Task<DateTime?> GetUtolsoSikeresFutasAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BiztvillCRM.Data.CrmDbContext>();
            
            // ✅ JAVÍTÁS: AutomatikaFutasNaplo-ból nézzük, nem az EmailKuldesNaplo-ból
            return await context.AutomatikaFutasNaplok
                .Where(a => a.Sikeres)
                .OrderByDescending(a => a.FutasiIdo)
                .Select(a => a.FutasiIdo)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nem sikerült lekérni az utolsó futás időpontját.");
            return null;
        }
    }

    /// <summary>Végrehajtja a feldolgozást egy scoped service-en keresztül.</summary>
    private async Task VegrehajtFeldolgozastAsync(CancellationToken cancellationToken)
    {
        var naplo = new AutomatikaFutasNaplo
        {
            FutasiIdo = DateTime.UtcNow,
            Sikeres = false
        };

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var ertesitoService = scope.ServiceProvider.GetRequiredService<ILejaratErtesitoService>();
            var context = scope.ServiceProvider.GetRequiredService<BiztvillCRM.Data.CrmDbContext>();
            
            var eredmeny = await ertesitoService.FeldolgozasAsync();
            
            naplo.Sikeres = true;
            naplo.FeldolgozottHitelesitesek = eredmeny.FeldolgozottHitelesitesek;
            naplo.FeldolgozottMeresek = eredmeny.FeldolgozottMeresek;
            naplo.KuldottEmailek = eredmeny.KuldottEmailek;
            naplo.SikertelenEmailek = eredmeny.SikertelenEmailek;
            
            context.AutomatikaFutasNaplok.Add(naplo);
            await context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Email értesítés feldolgozás befejezve. Küldött: {Kuldott}, Sikertelen: {Sikertelen}",
                eredmeny.KuldottEmailek, eredmeny.SikertelenEmailek);
        }
        catch (Exception ex)
        {
            naplo.Sikeres = false;
            naplo.Hiba = ex.Message;
            
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BiztvillCRM.Data.CrmDbContext>();
            context.AutomatikaFutasNaplok.Add(naplo);
            await context.SaveChangesAsync();
            
            _logger.LogError(ex, "Hiba az email értesítés feldolgozása közben.");
        }
    }
}