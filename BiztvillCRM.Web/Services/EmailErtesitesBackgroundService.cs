using BiztvillCRM.Services.Interfaces;

namespace BiztvillCRM.Web.Services;

/// <summary>
/// Háttérszolgáltatás, amely naponta egyszer ellenőrzi a lejáró hitelesítéseket/méréseket
/// és automatikusan küldi az értesítő emaileket.
/// </summary>
public class EmailErtesitesBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailErtesitesBackgroundService> _logger;
    private TimeSpan _futasiIdo; // 06:00 reggel

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

        while (!stoppingToken.IsCancellationRequested)
        {
            var most = DateTime.Now;
            var kovetkezoFutas = SzamolKovetkezoFutas(most);
            var varakozas = kovetkezoFutas - most;

            _logger.LogInformation(
                "Következő email értesítés feldolgozás: {KovetkezoFutas} ({Varakozas} múlva)",
                kovetkezoFutas.ToString("yyyy-MM-dd HH:mm:ss"),
                varakozas);

            try
            {
                await Task.Delay(varakozas, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            // Feldolgozás végrehajtása
            await VegrehajtFeldolgozastAsync(stoppingToken);
        }

        _logger.LogInformation("Email értesítés háttérszolgáltatás leállt.");
    }

    private DateTime SzamolKovetkezoFutas(DateTime most)
    {
        var maiNaponFutas = most.Date.Add(_futasiIdo);

        // Ha ma már elmúlt a futási idő, akkor holnapra ütemezzük
        if (most >= maiNaponFutas)
        {
            return maiNaponFutas.AddDays(1);
        }

        return maiNaponFutas;
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