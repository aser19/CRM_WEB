using MiniSoftware;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services;

public class JegyzokonyvWordService : IJegyzokonyvWordService
{
    private readonly IMeresService _meresService;
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    private readonly ISablonService _sablonService;

    public JegyzokonyvWordService(
        IMeresService meresService, 
        ITenantService tenantService,
        ICegService cegService,
        ISablonService sablonService)
    {
        _meresService = meresService;
        _tenantService = tenantService;
        _cegService = cegService;
        _sablonService = sablonService;
    }

    /// <summary>Generálás alapértelmezett sablonnal.</summary>
    public Task<byte[]> GeneralasAsync(int meresId)
        => GeneralasAsync(meresId, null!, "VBF_KIF_MINTA");

    /// <summary>Generálás választott sablonnal.</summary>
    public Task<byte[]> GeneralasAsync(int meresId, string sablonId)
        => GeneralasAsync(meresId, null!, sablonId);

    public async Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok formAdatok, string sablonId = "VBF_KIF_MINTA")
    {
        var meres = await _meresService.GetByIdAsync(meresId)
            ?? throw new ArgumentException($"Mérés nem található: {meresId}");

        var sablon = await _sablonService.GetByIdAsync(sablonId)
            ?? throw new ArgumentException($"Sablon nem található: {sablonId}");

        var sablonPath = _sablonService.GetSablonPath(sablon.FajlNev);
        
        if (!File.Exists(sablonPath))
            throw new FileNotFoundException($"Sablon fájl nem található: {sablonPath}");

        // Bejelentkezett felhasználó cége
        var cegId = _tenantService.GetCurrentCegId();
        var ceg = await _cegService.GetByIdAsync(cegId);

        var adatok = new Dictionary<string, object>
        {
            // === VIZSGÁLAT ALAPADATOK ===
            ["UGYFEL_CIM"] = formAdatok?.VizsgalatHelye ?? meres.Telephely?.Cim ?? "",
            ["MERES_IDEJE"] = meres.Datum.ToString("yyyy.MM.dd"),
            ["GENERALT_SZAM_CEG_JGYK"] = formAdatok?.JegyzokonyvSzam ?? $"VBF-{meres.Id:D6}/{DateTime.Now:yyyy}",
            ["VIZSG_TARGYA"] = formAdatok?.VizsgalatTargya ?? "",
            ["VIZSG_BERENDEZES"] = formAdatok?.VizsgaltBerendezes ?? "",
            
            // === CÉG ADATOK ===
            ["LICENSZ_TULAJDONOSA_(CEG_NEVE)"] = ceg?.Nev ?? "",
            ["LICENSZ_TULAJDONOSA_(CEG_CIME)"] = ceg?.Cim ?? "",
            
            // === MEGRENDELŐ ADATOK ===
            ["VIZSG_MEGRENDELO"] = formAdatok?.Megrendelo ?? meres.Ugyfel?.Nev ?? "",
            ["VIZSG_UZEMI_KISERO"] = formAdatok?.UzemiKisero ?? "",
            ["VIZSG_KAPCSOLAT_TARTO"] = formAdatok?.KapcsolatTarto ?? meres.Telephely?.Kapcsolattarto ?? "",
            ["VIZSG_IDOTARTAM"] = formAdatok?.VizsgalatIdotartama ?? "",
            
            // === FELELŐS FELÜLVIZSGÁLÓ ===
            ["FELULVIZSGALO"] = formAdatok?.FelulvizsgaloNev ?? "",
            ["FELULVIZSGALO_BIZONYITVANY"] = formAdatok?.FelulvizsgaloBizonyitvany ?? "",
            ["FELULVIZSGALO_MEGUJITO_KEPZES"] = formAdatok?.FelulvizsgaloKepzes ?? "",
            
            // === SEGÍTŐ FELÜLVIZSGÁLÓ ===
            ["SEGITO_FELULVIZSGALO"] = formAdatok?.SegitoFelulvizsgalo ?? "",
            ["SEGITO_BIZONYITVANY"] = formAdatok?.SegitoBizonyitvany ?? "",
            ["SEGITO_MEGUJIT_KEPZES"] = formAdatok?.SegitoKepzes ?? "",
            
            // === ELLENŐR ===
            ["ELLENOR"] = formAdatok?.Ellenor ?? "",
            ["ELLENOR_BIZONYITVANY_SZAMA"] = formAdatok?.EllenorBizonyitvany ?? "",
            ["ELLENOR_FELUJITO_KEPZES"] = formAdatok?.EllenorKepzes ?? "",
            
            // === KELTEZÉS ===
            ["AKT_DATUM"] = DateTime.Today.ToString("yyyy.MM.dd"),
        };

        using var ms = new MemoryStream();
        await MiniWord.SaveAsByTemplateAsync(ms, sablonPath, adatok);
        return ms.ToArray();
    }
}