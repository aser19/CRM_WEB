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

        var kitoltottMuszerek = formAdatok?.Muszerek?
            .Where(m => !string.IsNullOrEmpty(m.Tipus))
            .ToList() ?? new List<MuszerSor>();

        // *** ÚJ: MÉRÉSI PONTOK ***
        var meresiPontok = formAdatok?.MeresiPontok ?? new List<MeresiPontSor>();

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
            ["MF_X"] = (formAdatok?.Eredmeny == "MEGFELELT") ? "☑" : "☐",
            ["NMF_X"] = (formAdatok?.Eredmeny == "NEM FELELT MEG") ? "☑" : "☐",

            // Végső minősítés checkbox-ok
            ["VMF_X"] = (formAdatok?.VegsoMinosites == "MEGFELELT") ? "☑" : "☐",
            ["VNMF_X"] = (formAdatok?.VegsoMinosites == "NEM FELELT MEG") ? "☑" : "☐",

            // === 3. OLDAL ===
            ["ERV_MEGRENDELES_X"] = formAdatok?.ERV_MEGRENDELES_X ?? "☐",
            ["ERV_SZABALYZAT_X"] = formAdatok?.ERV_SZABALYZAT_X ?? "☐",
            ["ERV_DATUM"] = formAdatok?.ERV_DATUM ?? "",

            ["KOV_50KW_X"] = formAdatok != null && formAdatok.KovFelulv50kW ? "☑" : "☐",
            ["KOV_32A_X"] = formAdatok != null && formAdatok.KovFelulv32A ? "☑" : "☐",
            ["KOV_VMBSZ_X"] = formAdatok != null && formAdatok.KovFelulvVMBSZ ? "☑" : "☐",
            ["KOV_RV300_X"] = formAdatok != null && formAdatok.KovFelulvRV300 ? "☑" : "☐",
            ["KOV_EGYEB1_X"] = formAdatok?.KOV_EGYEB1_X ?? "☐",
            ["KOV_EGYEB1_SZOVEG"] = formAdatok?.KOV_EGYEB1_SZOVEG ?? "",

            ["HAT_3EV_X"] = formAdatok != null && formAdatok.HataridoHarom ? "☑" : "☐",
            ["HAT_3EV_DATUM"] = formAdatok?.HAT_3EV_DATUM ?? "",
            ["HAT_LAKAS_X"] = formAdatok != null && formAdatok.HataridoHat ? "☑" : "☐",
            ["HAT_RV_X"] = formAdatok != null && formAdatok.HataridoRV ? "☑" : "☐",
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

            // ÚJ: MÉRÉSI PONTOK DINAMIKUS TÁBLÁZAT
            ["meresi_rendszer_tipus"] = formAdatok?.MeresiRendszerTipus ?? "TN",
            ["meresi_pontok"] = meresiPontok.Select(mp => (object)new
            {
                sorszam               = $"{mp.Sorszam}.",
                meresi_pont_helye     = mp.MeresiPontHelye ?? "",
                rendszer_tipus        = formAdatok?.MeresiRendszerTipus ?? "TN",
                modszer               = mp.Modszer ?? "",
                tularamvedelem_helye  = mp.TularamvedelemHelye ?? "",
                tularamvedelem_tipusa = mp.TularamvedelemTipusa ?? "",
                avk                   = mp.AVKCsatolva ? "✓" : "✗",
                avk_szin              = mp.AVKCsatolva ? "zöld" : "piros",
                pe_folyt              = mp.PEFolytOhm?.ToString() ?? "",
                ertek_ohm             = mp.MertHurokimpedancia?.ToString("F2") ?? mp.ErtekOhm?.ToString() ?? "",
                Minosites             = mp.Minosites ?? "",
                mp_megjegyzes         = mp.Megjegyzes ?? ""
            }).ToList(),
            ["meresi_pontok_db"] = meresiPontok.Count.ToString(),

            // 3 éves csoport (301-305, 310, 311)
            ["301"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "50kW" ? "☑" : "☐",
            ["302"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "32A" ? "☑" : "☐",
            ["303"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "VMBSZ" ? "☑" : "☐",
            ["304"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "RV300" ? "☑" : "☐",
            ["305"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305" ? "☑" : "☐",
            ["3051"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305"
    ? (formAdatok.KovetkezoFelulvizsgalatEgyeb ?? "") : "",
            ["310"] = (formAdatok?.KovetkezoFelulvizsgalatTipus is "50kW" or "32A" or "VMBSZ" or "RV300" or "egyeb305")
    ? $"a kiadási dátumtól számított 3 éven belül, legkésőbb: {GetSzamitottDatumStatic(meres?.Datum, 3)}-ig kell elvégezni." : "",
            ["311"] = GetSzamitottDatumStatic(meres?.Datum, 3),

            // 6 éves csoport (307, 308, 3081, 309)
            ["307"] = formAdatok?.HataridoTipus == "307" ? "☑" : "☐",
            ["308"] = formAdatok?.HataridoTipus == "308" ? "☑" : "☐",
            ["3081"] = formAdatok?.HataridoTipus == "308"
    ? (formAdatok.HataridoEgyeb ?? "") : "",
            ["309"] = (formAdatok?.HataridoTipus is "307" or "308")
    ? GetSzamitottDatumStatic(meres?.Datum, 6) : "",

            // === 4. OLDAL – VILLAMOS BERENDEZÉS ADATAI ===
            ["401"] = formAdatok?.NevlegesFeszultsegTipus == "1fazis" ? "230 V" : "3×230 V / 400 V",
            ["NEVLEGES_FESZULTSEG_1F_X"] = formAdatok?.NevlegesFeszultsegTipus == "1fazis" ? "☑" : "☐",
            ["NEVLEGES_FESZULTSEG_3F_X"] = formAdatok?.NevlegesFeszultsegTipus == "3fazis" ? "☑" : "☐",

            ["402"] = formAdatok?.FoldelesiTipusKod switch
            {
                "szonda" => "A",
                "vizszintes" => "B",
                "mindketto" => "A + B",
                _ => ""
            },

            ["403"] = formAdatok?.ErintesvedelmiMod ?? "",

            ["404"] = formAdatok != null && formAdatok.Vedelem404 ? "☑" : "☐",
            ["405"] = formAdatok != null && formAdatok.Vedelem405 ? "☑" : "☐",
            ["406"] = formAdatok != null && formAdatok.Vedelem406 ? "☑" : "☐",
            ["407"] = formAdatok != null && formAdatok.Vedelem407 ? "☑" : "☐",
            ["408"] = formAdatok != null && formAdatok.Vedelem408 ? "☑" : "☐",
            ["409"] = formAdatok != null && formAdatok.Vedelem409 ? "☑" : "☐",

            ["410"] = formAdatok?.Betaplalas ?? "",
            ["411"] = formAdatok?.TartalekEnergia ?? "",
            ["412"] = formAdatok?.LegutolsoFelujitas ?? "",
            ["413"] = formAdatok?.Dokumentaciok ?? "",
        };

        
        // DEBUG: Ellenőrizd az adatokat
        var meresiPontokList = adatok["meresi_pontok"] as List<object>;
        System.Diagnostics.Debug.WriteLine($"[DEBUG] Mérési pontok száma: {meresiPontokList?.Count ?? 0}");
        foreach (var pont in meresiPontokList ?? new List<object>())
        {
            var dict = pont as Dictionary<string, object>;
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Pont: {dict?["sorszam"]}, Helye: {dict?["meresi_pont_helye"]}");
        }

        // NULL ellenőrzés minden értékre
foreach (var kv in adatok)
{
    if (kv.Value == null)
        System.Diagnostics.Debug.WriteLine($"[NULL] Kulcs: {kv.Key}");
}

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
    private static string GetSzamitottDatumStatic(DateTime? datum, int evek)
    => datum?.AddYears(evek).ToString("yyyy.MM.dd") ?? "-";
}