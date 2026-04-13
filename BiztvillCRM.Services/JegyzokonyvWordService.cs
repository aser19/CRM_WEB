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
        // Új jegyzőkönyvnél (meresId == 0) a meres objektum opcionális
        Meres? meres = null;
        if (meresId > 0)
        {
            meres = await _meresService.GetByIdAsync(meresId);
            if (meres == null)
                throw new ArgumentException($"Mérés nem található: {meresId}");
        }

        var sablon = await _sablonService.GetByIdAsync(sablonId)
            ?? throw new ArgumentException($"Sablon nem található: {sablonId}");

        var sablonPath = _sablonService.GetSablonPath(sablon.FajlNev);
        
        if (!File.Exists(sablonPath))
            throw new FileNotFoundException($"Sablon fájl nem található: {sablonPath}");

        var cegId = _tenantService.GetCurrentCegId();
        var ceg = await _cegService.GetByIdAsync(cegId);

        // Eszközök statisztikái
        var eszkozok = formAdatok?.Eszkozok ?? new List<HordozhatoEszkozSor>();
        var osszesDb = eszkozok.Count;
        var mfDb = eszkozok.Count(e => e.Megtekint == "MF");
        var nmfDb = eszkozok.Count(e => e.Megtekint == "NMF");

        // *** MŰSZEREK - DICTIONARY ELŐTT KELL DEKLARÁLNI! ***
        var kitoltottMuszerek = formAdatok?.Muszerek?
            .Where(m => !string.IsNullOrEmpty(m.Tipus))
            .ToList() ?? new List<MuszerSor>();

        var adatok = new Dictionary<string, object>
        {
            // === VIZSGÁLAT ALAPADATOK ===
            ["UGYFEL_CIM"] = formAdatok?.VizsgalatHelye ?? meres?.Telephely?.Cim ?? "",
            ["MERES_IDEJE"] = meres?.Datum.ToString("yyyy.MM.dd") ?? DateTime.Today.ToString("yyyy.MM.dd"),
            ["GENERALT_SZAM_CEG_JGYK"] = formAdatok?.JegyzokonyvSzam ?? $"VBF-{meresId:D6}/{DateTime.Now:yyyy}",
            ["VIZSG_TARGYA"] = formAdatok?.VizsgalatTargya ?? "",
            ["VIZSG_BERENDEZES"] = formAdatok?.VizsgaltBerendezes ?? "",
            
            // === CÉG ADATOK ===
            ["CEG_NEVE"] = ceg?.Nev ?? "",
            ["CEG_CIME"] = ceg?.Cim ?? "",
            
            // === MEGRENDELŐ ADATOK ===
            ["VIZSG_MEGRENDELO"] = formAdatok?.Megrendelo ?? meres?.Ugyfel?.Nev ?? "",
            ["VIZSG_UZEMI_KISERO"] = formAdatok?.UzemiKisero ?? "",
            ["VIZSG_KAPCSOLAT_TARTO"] = formAdatok?.KapcsolatTarto ?? meres?.Telephely?.Kapcsolattarto ?? "",
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
            ["aktualis_datum"] = DateTime.Today.ToString("yyyy.MM.dd"),

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

            // Eredmény checkbox-ok
            ["MF_X"] = (formAdatok?.Eredmeny == "MEGFELELT") ? "☒" : "☐",
            ["NMF_X"] = (formAdatok?.Eredmeny == "NEM FELELT MEG") ? "☒" : "☐",

            // Végső minősítés checkbox-ok
            ["VMF_X"] = (formAdatok?.VegsoMinosites == "MEGFELELT") ? "☒" : "☐",
            ["VNMF_X"] = (formAdatok?.VegsoMinosites == "NEM FELELT MEG") ? "☒" : "☐",

            // === 3. OLDAL ===
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

            // === HORDOZHATÓ KÉSZÜLÉK SPECIFIKUS ===
            ["ugyfel_nev"] = formAdatok?.Megrendelo ?? meres?.Ugyfel?.Nev ?? "",
            
            ["dolgozo_neve"] = formAdatok?.DolgozoNeve ?? "",
            ["van_dolgozo"] = !string.IsNullOrWhiteSpace(formAdatok?.DolgozoNeve) ? "true" : "",
            ["dolgozo_szoveg"] = !string.IsNullOrWhiteSpace(formAdatok?.DolgozoNeve) 
                ? $"Dolgozó neve: {formAdatok.DolgozoNeve}" 
                : "",

            // === KOMBINÁLT DOLGOZÓ + FORGALMI RENDSZÁM - HIÁNYZOTT! ===
            ["dolgozo_frsz_kombinalt"] = GetDolgozoFrszKombinalt(formAdatok?.DolgozoNeve, formAdatok?.ForgalmiRendszam),

            ["forgalmi_rendszam"] = formAdatok?.ForgalmiRendszam ?? "",
            
            ["kovetkezo_felulvizsgalat"] = formAdatok?.KovetkezoFelulvizsgalatDatum?.ToString("yyyy.MM.dd") 
                ?? meres?.KovetkezoDatum?.ToString("yyyy.MM.dd") 
                ?? DateTime.Today.AddYears(1).ToString("yyyy.MM.dd"),
            ["kov_felulviz_datum"] = formAdatok?.KovetkezoFelulvizsgalatDatum?.ToString("yyyy.MM.dd") 
                ?? DateTime.Today.AddYears(1).ToString("yyyy.MM.dd"),
                
            ["megrendelo"] = formAdatok?.Megrendelo ?? meres?.Ugyfel?.Nev ?? "",
            ["telephely"] = formAdatok?.VizsgalatHelye ?? meres?.Telephely?.Cim ?? "",
            ["felulvizsgalat_ideje"] = meres?.Datum.ToString("yyyy.MM.dd") ?? DateTime.Today.ToString("yyyy.MM.dd"),

            // === MŰSZEREK (DINAMIKUS) ===
            ["muszer1_tipus"] = kitoltottMuszerek.ElementAtOrDefault(0)?.Tipus ?? "",
            ["muszer_gysz1"] = kitoltottMuszerek.ElementAtOrDefault(0)?.GyariSzam ?? "",
            ["kalib1"] = kitoltottMuszerek.ElementAtOrDefault(0)?.Kalibralas ?? "",
            ["muszer2_tipus"] = kitoltottMuszerek.ElementAtOrDefault(1)?.Tipus ?? "",
            ["muszer_gysz2"] = kitoltottMuszerek.ElementAtOrDefault(1)?.GyariSzam ?? "",
            ["kalib2"] = kitoltottMuszerek.ElementAtOrDefault(1)?.Kalibralas ?? "",
            ["muszer3_tipus"] = kitoltottMuszerek.ElementAtOrDefault(2)?.Tipus ?? "",
            ["muszer_gysz3"] = kitoltottMuszerek.ElementAtOrDefault(2)?.GyariSzam ?? "",
            ["kalib3"] = kitoltottMuszerek.ElementAtOrDefault(2)?.Kalibralas ?? "",

            // Dinamikus műszerlista
            ["muszerek"] = kitoltottMuszerek.Select((m, i) => new 
            {
                muszer_sorszam = (i + 1).ToString(),
                muszer_tipus = m.Tipus ?? "",
                muszer_gysz = m.GyariSzam ?? "",
                muszer_kalib = m.Kalibralas ?? "",
            }).ToList(),

            // Van-e műszer feltételek
            ["van_muszer1"] = kitoltottMuszerek.Count >= 1 ? "true" : "",
            ["van_muszer2"] = kitoltottMuszerek.Count >= 2 ? "true" : "",
            ["van_muszer3"] = kitoltottMuszerek.Count >= 3 ? "true" : "",

            // Munkaszám
            ["munkaszam"] = formAdatok?.Munkaszam ?? $"HK-{meresId:D6}/{DateTime.Now:yyyy}",
            
            // === ESZKÖZÖK STATISZTIKA ===
            ["osszes_db"] = osszesDb.ToString(),
            ["mf_db"] = mfDb.ToString(),
            ["nmf_db"] = nmfDb.ToString(),
            
            // === DINAMIKUS ESZKÖZLISTA - HELYISÉG CSOPORTOSÍTÁSSAL ===
            // A formAdatok.Eszkozok-nak már rendezettnek kell lennie!
["eszkozok"] = GenerateEszkozListaHelyiseggel(eszkozok),

            // Cég lábléc
            ["ceg_telephely"] = ceg?.Cim ?? "",
            ["ceg_weboldal"] = ceg?.Weboldal ?? "",
            ["ceg_telefonszam"] = ceg?.Telefon ?? "",

            // === MATRICA SOROZATSZÁMOK ===
            ["matrica_tol"] = formAdatok?.MatricaSorszamTol ?? "",
            ["matrica_ig"] = formAdatok?.MatricaSorszamIg ?? "",
            ["matrica_szoveg"] = (formAdatok?.VanMatricaSorszam == true) 
                ? $"A felülvizsgálat során elhelyezett matricák sorozatszáma: {formAdatok.MatricaSorszamTol} -tól {formAdatok.MatricaSorszamIg} -ig tart."
                : "",
            ["van_matrica"] = formAdatok?.VanMatricaSorszam == true ? "true" : "",
        };

        using var ms = new MemoryStream();
        await MiniWord.SaveAsByTemplateAsync(ms, sablonPath, adatok);
        return ms.ToArray();
    }

    private static string GetDolgozoFrszKombinalt(string? dolgozoNeve, string? forgalmiRendszam)
    {
        var vanDolgozo = !string.IsNullOrWhiteSpace(dolgozoNeve);
        var vanFrsz = !string.IsNullOrWhiteSpace(forgalmiRendszam);

        if (vanDolgozo && vanFrsz)
        {
            return $" | Munkavállaló neve: {dolgozoNeve} | Forgalmi rendszám: {forgalmiRendszam}";
        }
        else if (vanDolgozo)
        {
            return $" | Munkavállaló neve: {dolgozoNeve}";
        }
        else if (vanFrsz)
        {
            return $" | Forgalmi rendszám: {forgalmiRendszam}";
        }
        
        return "";
    }

    /// <summary>
    /// Eszközlista generálása helyiség csoportosítással.
    /// Minden helyiség előtt egy fejléc sor jelenik meg.
    /// </summary>
    private static List<object> GenerateEszkozListaHelyiseggel(List<HordozhatoEszkozSor> eszkozok)
    {
        var eredmeny = new List<object>();
        var sorszam = 1;
        
        // Csoportosítás helyiség szerint - BEVITELI SORREND megtartása
        var csoportok = eszkozok
            .GroupBy(e => string.IsNullOrWhiteSpace(e.HelyisegNev) ? "" : e.HelyisegNev)
            .Select((g, index) => new { 
                Key = g.Key, 
                Items = g.ToList(), 
                FirstIndex = eszkozok.FindIndex(e => (e.HelyisegNev ?? "") == g.Key) // Első előfordulás indexe
            })
            .OrderBy(g => string.IsNullOrEmpty(g.Key) ? int.MaxValue : g.FirstIndex); // Üres helyiség a végére

        foreach (var csoport in csoportok)
        {
            // Helyiség fejléc sor (ha van helyiség név)
            if (!string.IsNullOrEmpty(csoport.Key))
            {
                eredmeny.Add(new
                {
                    sorsz = "",
                    megnevezes = csoport.Key,  // Helyiség neve a megnevezés oszlopban
                    tipus = "",
                    azonosito = "",
                    osztaly = "",
                    telj = "",
                    megtekint = "",
                    folyt = "",
                    szigell = "",
                    szivargo = "",
                    megjegyzes = "",
                    is_header = "true"  // Jelző a sablonban való formázáshoz
                });
            }

            // Eszközök a csoportban
            foreach (var eszkoz in csoport.Items)
            {
                eredmeny.Add(new
                {
                    sorsz = sorszam.ToString() + ".",
                    megnevezes = eszkoz.Megnevezes ?? "",
                    tipus = eszkoz.Tipus ?? "",
                    azonosito = eszkoz.Azonosito ?? "",
                    osztaly = eszkoz.VedelmiOsztaly ?? "",
                    telj = eszkoz.Telj ?? "",
                    megtekint = eszkoz.Megtekint ?? "",
                    folyt = eszkoz.KellFolyt ? (eszkoz.Folyt ?? "-") : "-",
                    szigell = eszkoz.Szigell ?? "",
                    szivargo = eszkoz.Szivargo ?? "",
                    megjegyzes = eszkoz.Megjegyzes ?? "",
                    is_header = ""
                });
                sorszam++;
            }
        }

        return eredmeny;
    }
}