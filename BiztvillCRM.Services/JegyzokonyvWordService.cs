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
            ["CEG_NEVE"] = ceg?.Nev ?? "",
            ["CEG_CIME"] = ceg?.Cim ?? "",
            
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

            // === 2. OLDAL - MINŐSÍTŐ IRAT ===
            ["EREDMENY"] = formAdatok?.Eredmeny ?? "",
            ["HIBAK_B"] = formAdatok?.HibakB ?? "",
            ["HIBAK_C"] = formAdatok?.HibakC ?? "",
            ["HIBAK_D"] = formAdatok?.HibakD ?? "",
            ["HIBAK_E"] = formAdatok?.HibakE ?? "",
            ["VEGSO_MINOSITES"] = formAdatok?.VegsoMinosites ?? "",
            ["Melleklet_db"] = formAdatok?.MellekletekSzama ?? "",
            ["HIBAVED_JKV"] = formAdatok?.HibavedelmiJkv ?? "",
            ["AVK_JEGYZOKONYV"] = formAdatok?.AvkJegyzokonyv ?? "",
            ["MEGJEGYZES"] = formAdatok?.Megjegyzes ?? "",

            // Eredmény checkbox-ok (szöveges helyettesítés)
            ["MF_X"] = (formAdatok?.Eredmeny == "MEGFELELT") ? "☒" : "☐",
            ["NMF_X"] = (formAdatok?.Eredmeny == "NEM FELELT MEG") ? "☒" : "☐",

            // Végső minősítés checkbox-ok
            ["VMF_X"] = (formAdatok?.VegsoMinosites == "MEGFELELT") ? "☒" : "☐",
            ["VNMF_X"] = (formAdatok?.VegsoMinosites == "NEM FELELT MEG") ? "☒" : "☐",

            // === 3. OLDAL - MINŐSÍTŐ IRAT 2/2 ===
            ["ERV_MEGRENDELES_X"] = formAdatok?.ERV_MEGRENDELES_X ?? "☐",
            ["ERV_SZABALYZAT_X"] = formAdatok?.ERV_SZABALYZAT_X ?? "☐",
            ["ERV_DATUM"] = formAdatok?.ERV_DATUM ?? "",
            
            ["KOV_50KW_X"] = formAdatok?.KOV_50KW_X ?? "☐",
            ["KOV_32A_X"] = formAdatok?.KOV_32A_X ?? "☐",
            ["KOV_VMBSZ_X"] = formAdatok?.KOV_VMBSZ_X ?? "☐",
            ["KOV_RV300_X"] = formAdatok?.KOV_RV300_X ?? "☐",
            ["KOV_EGYEB1_X"] = formAdatok?.KOV_EGYEB1_X ?? "☐",
            ["KOV_EGYEB1_SZOVEG"] = formAdatok?.KOV_EGYEB1_SZOVEG ?? "",
            
            ["HAT_3EV_X"] = formAdatok?.HAT_3EV_X ?? "☐",
            ["HAT_3EV_DATUM"] = formAdatok?.HAT_3EV_DATUM ?? "",
            ["HAT_LAKAS_X"] = formAdatok?.HAT_LAKAS_X ?? "☐",
            ["HAT_RV_X"] = formAdatok?.HAT_RV_X ?? "☐",
            ["HAT_EGYEB2_X"] = formAdatok?.HAT_EGYEB2_X ?? "☐",
            ["HAT_EGYEB2_SZOVEG"] = formAdatok?.HAT_EGYEB2_SZOVEG ?? "",
            ["HAT_6EV_DATUM"] = formAdatok?.HAT_6EV_DATUM ?? "",
            
            ["MINOSITO_MEGJEGYZES"] = formAdatok?.MINOSITO_MEGJEGYZES ?? "",
        };

        using var ms = new MemoryStream();
        await MiniWord.SaveAsByTemplateAsync(ms, sablonPath, adatok);
        return ms.ToArray();
    }
}