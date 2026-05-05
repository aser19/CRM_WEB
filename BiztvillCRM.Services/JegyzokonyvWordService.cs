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
            ["MF_X"] = (formAdatok?.Eredmeny == "MEGFELELT") ? "☑" : "🗷",
            ["NMF_X"] = (formAdatok?.Eredmeny == "NEM FELELT MEG") ? "☑" : "🗷",

            // Végső minősítés checkbox-ok
            ["VMF_X"] = (formAdatok?.VegsoMinosites == "MEGFELELT") ? "☑" : "🗷",
            ["VNMF_X"] = (formAdatok?.VegsoMinosites == "NEM FELELT MEG") ? "☑" : "🗷",

            // === 3. OLDAL ===
            ["ERV_MEGRENDELES_X"] = formAdatok?.ERV_MEGRENDELES_X ?? "🗷",
            ["ERV_SZABALYZAT_X"] = formAdatok?.ERV_SZABALYZAT_X ?? "🗷",
            ["ERV_DATUM"] = formAdatok?.ERV_DATUM ?? "",

            // 3 éves csoport (301-305, 310, 311)
            ["301"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "50kW" ? "☑" : "🗷",
            ["302"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "32A" ? "☑" : "🗷",
            ["303"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "VMBSZ" ? "☑" : "🗷",
            ["304"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "RV300" ? "☑" : "🗷",
            ["305"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305" ? "☑" : "🗷",
            ["3051"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305"  ? (formAdatok.KovetkezoFelulvizsgalatEgyeb ?? "") : "",
            ["310"] = (formAdatok?.KovetkezoFelulvizsgalatTipus is "50kW" or "32A" or "VMBSZ" or "RV300" or "egyeb305")
                ? $"a kiadási dátumtól számított 3 éven belül, legkésőbb: {GetSzamitottDatumStatic(meres?.Datum, 3)}-ig kell elvégezni." : "",
            ["311"] = GetSzamitottDatumStatic(meres?.Datum, 3),

            // 6 éves csoport (307, 308, 3081, 309)
            ["307"] = formAdatok?.HataridoTipus == "307" ? "☑" : "🗷",
            ["308"] = formAdatok?.HataridoTipus == "308" ? "☑" : "🗷",
            ["309"] = formAdatok?.HataridoTipus == "308" ? (formAdatok.HataridoEgyeb ?? "") : "",
            ["3091"] = formAdatok?.HataridoTipus == "309" ? (formAdatok.HataridoEgyeb ?? "") : "",
            ["312"] = (formAdatok?.HataridoTipus is "307" or "308" or "309")            
                ? $"a kiadási dátumtól számított 6 éven belül, legkésőbb: {GetSzamitottDatumStatic(meres?.Datum, 6)}-ig kell elvégezni." : "",

            ["313_MEGJEGYZES"] = formAdatok?.MINOSITO_MEGJEGYZES ?? "",

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
            ["301"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "50kW" ? "☑" : "🗷",
            ["302"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "32A" ? "☑" : "🗷",
            ["303"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "VMBSZ" ? "☑" : "🗷",
            ["304"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "RV300" ? "☑" : "🗷",
            ["305"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305" ? "☑" : "🗷",
            ["3051"] = formAdatok?.KovetkezoFelulvizsgalatTipus == "egyeb305"
    ? (formAdatok.KovetkezoFelulvizsgalatEgyeb ?? "") : "",
            ["310"] = (formAdatok?.KovetkezoFelulvizsgalatTipus is "50kW" or "32A" or "VMBSZ" or "RV300" or "egyeb305")
    ? $"a kiadási dátumtól számított 3 éven belül, legkésőbb: {GetSzamitottDatumStatic(meres?.Datum, 3)}-ig kell elvégezni." : "",
            ["311"] = GetSzamitottDatumStatic(meres?.Datum, 3),

            // 6 éves csoport (307, 308, 3081, 309)
            ["307"] = formAdatok?.HataridoTipus == "307" ? "☑" : "🗷",
            ["308"] = formAdatok?.HataridoTipus == "308" ? "☑" : "🗷",
            ["309"] = formAdatok?.HataridoTipus == "308" ? "☑" : "🗷",
            ["3091"] = formAdatok?.HataridoTipus == "309"
    ? (formAdatok.HataridoEgyeb ?? "") : "",
            ["312"] = (formAdatok?.HataridoTipus is "307" or "308" or "309")
    ? $"a kiadási dátumtól számított 6 éven belül, legkésőbb: {GetSzamitottDatumStatic(meres?.Datum, 6)}-ig kell elvégezni." : "",
            ["313"] = GetSzamitottDatumStatic(meres?.Datum, 6),
            ["313_MEGJEGYZES"] = formAdatok?.MinositoIratMegjegyzes ?? "",

            // === 4. OLDAL – VILLAMOS BERENDEZÉS ADATAI ===
            ["401"] = formAdatok?.NevlegesFeszultsegTipus == "1fazis" ? "230 V" : "3×230 V / 400 V",
            ["NEVLEGES_FESZULTSEG_1F_X"] = formAdatok?.NevlegesFeszultsegTipus == "1fazis" ? "☑" : "🗷",
            ["NEVLEGES_FESZULTSEG_3F_X"] = formAdatok?.NevlegesFeszultsegTipus == "3fazis" ? "☑" : "🗷",

            ["402"] = formAdatok?.FoldelesiTipusKod switch
            {
                "szonda" => "A",
                "vizszintes" => "B",
                "mindketto" => "A + B",
                _ => ""
            },

            ["403"] = formAdatok?.ErintesvedelmiMod ?? "",

            ["404"] = formAdatok != null && formAdatok.Vedelem404 ? "☑" : "🗷",
            ["405"] = formAdatok != null && formAdatok.Vedelem405 ? "☑" : "🗷",
            ["406"] = formAdatok != null && formAdatok.Vedelem406 ? "☑" : "🗷",
            ["407"] = formAdatok != null && formAdatok.Vedelem407 ? "☑" : "🗷",
            ["408"] = formAdatok != null && formAdatok.Vedelem408 ? "☑" : "🗷",
            ["409"] = formAdatok != null && formAdatok.Vedelem409 ? "☑" : "🗷",

            ["410"] = formAdatok?.Betaplalas ?? "",
            ["411"] = formAdatok?.TartalekEnergia ?? "",
            ["412"] = formAdatok?.LegutolsoFelujitas ?? "",
            ["413"] = formAdatok?.Dokumentaciok ?? "",

            // === JOGSZABÁLYOK ÉS SZABVÁNYOK LISTA ===
            ["jogszabalyok"] = (formAdatok?.KijeloltJogszabalyok?
                .Where(j => !j.IsSzabvany && j.Kivalasztva)
                .OrderBy(j => j.Szam)
                .Select(j => new Dictionary<string, object> { ["jsz_szam"] = j.Szam })
                .ToList() as object) ?? new List<Dictionary<string, object>>(),

            ["szabvanyok"] = (formAdatok?.KijeloltJogszabalyok?
                .Where(j => j.IsSzabvany && j.Kivalasztva)
                .OrderBy(j => j.Szam)
                .Select(j => new Dictionary<string, object> { ["sz_szam"] = j.Szam })
                .ToList() as object) ?? new List<Dictionary<string, object>>(),

            // === 5. OLDAL – MSZ HD 60364-6 ELLENŐRZÉSEK ===
            // Szerkezetek
            ["501"] = formAdatok?.Ellen5_Sz_A ?? "MF",
            ["502"] = formAdatok?.Ellen5_Sz_B ?? "MF",
            ["503"] = formAdatok?.Ellen5_Sz_C ?? "MF",

            // Megtekintéses ellenőrzések
            ["504"] = formAdatok?.Ellen5_Me_A ?? "MF",
            ["505"] = formAdatok?.Ellen5_Me_B ?? "MF",
            ["506"] = formAdatok?.Ellen5_Me_C ?? "MF",
            ["507"] = formAdatok?.Ellen5_Me_D ?? "MF",
            ["508"] = formAdatok?.Ellen5_Me_E ?? "MF",
            ["509"] = formAdatok?.Ellen5_Me_F ?? "MF",
            ["510"] = formAdatok?.Ellen5_Me_G ?? "MF",
            ["511"] = formAdatok?.Ellen5_Me_H ?? "MF",
            ["512"] = formAdatok?.Ellen5_Me_I ?? "MF",
            ["513"] = formAdatok?.Ellen5_Me_J ?? "MF",
            ["514"] = formAdatok?.Ellen5_Me_K ?? "MF",
            ["515"] = formAdatok?.Ellen5_Me_L ?? "MF",
            ["516"] = formAdatok?.Ellen5_Me_M ?? "MF",
            ["517"] = formAdatok?.Ellen5_Me_N ?? "MF",
            ["518"] = formAdatok?.Ellen5_Me_O ?? "MF",
            ["519"] = formAdatok?.Ellen5_Me_P ?? "MF",

            // Megjegyzések
            // === 5. OLDAL – MEGJEGYZÉSEK ===
            ["501M"] = formAdatok?.Ellen5_Sz_A_M ?? "",
            ["502M"] = formAdatok?.Ellen5_Sz_B_M ?? "",
            ["503M"] = formAdatok?.Ellen5_Sz_C_M ?? "",
            ["504M"] = formAdatok?.Ellen5_Me_A_M ?? "",
            ["505M"] = formAdatok?.Ellen5_Me_B_M ?? "",
            ["506M"] = formAdatok?.Ellen5_Me_C_M ?? "",
            ["507M"] = formAdatok?.Ellen5_Me_D_M ?? "",
            ["508M"] = formAdatok?.Ellen5_Me_E_M ?? "",
            ["509M"] = formAdatok?.Ellen5_Me_F_M ?? "",
            ["510M"] = formAdatok?.Ellen5_Me_G_M ?? "",
            ["511M"] = formAdatok?.Ellen5_Me_H_M ?? "",
            ["512M"] = formAdatok?.Ellen5_Me_I_M ?? "",
            ["513M"] = formAdatok?.Ellen5_Me_J_M ?? "",
            ["514M"] = formAdatok?.Ellen5_Me_K_M ?? "",
            ["515M"] = formAdatok?.Ellen5_Me_L_M ?? "",
            ["516M"] = formAdatok?.Ellen5_Me_M_M ?? "",
            ["517M"] = formAdatok?.Ellen5_Me_N_M ?? "",
            ["518M"] = formAdatok?.Ellen5_Me_O_M ?? "",
            ["519M"] = formAdatok?.Ellen5_Me_P_M ?? "",
            ["Ellen5_Megjegyzes"] = formAdatok?.Ellen5_Megjegyzes ?? "",
            // === 5. OLDAL – MÉRÉSEK ===
            ["520"] = formAdatok?.Ellen5_Mr_A ?? "MF",
            ["521"] = formAdatok?.Ellen5_Mr_B ?? "MF",
            ["522"] = formAdatok?.Ellen5_Mr_C ?? "MF",
            ["523"] = formAdatok?.Ellen5_Mr_D ?? "MF",
            ["524"] = formAdatok?.Ellen5_Mr_E ?? "MF",
            ["525"] = formAdatok?.Ellen5_Mr_F ?? "MF",
            ["526"] = formAdatok?.Ellen5_Mr_G ?? "MF",
            ["527"] = formAdatok?.Ellen5_Mr_H ?? "MF",
            ["528"] = formAdatok?.Ellen5_Mr_I ?? "MF",
            ["529"] = formAdatok?.Ellen5_Mr_J ?? "MF",
            ["520M"] = formAdatok?.Ellen5_Mr_A_M ?? "",
            ["521M"] = formAdatok?.Ellen5_Mr_B_M ?? "",
            ["522M"] = formAdatok?.Ellen5_Mr_C_M ?? "",
            ["523M"] = formAdatok?.Ellen5_Mr_D_M ?? "",
            ["524M"] = formAdatok?.Ellen5_Mr_E_M ?? "",
            ["525M"] = formAdatok?.Ellen5_Mr_F_M ?? "",
            ["526M"] = formAdatok?.Ellen5_Mr_G_M ?? "",
            ["527M"] = formAdatok?.Ellen5_Mr_H_M ?? "",
            ["528M"] = formAdatok?.Ellen5_Mr_I_M ?? "",
            ["529M"] = formAdatok?.Ellen5_Mr_J_M ?? "",
            // === 6. OLDAL – OTSZ ELLENŐRZÉSEK ===
            ["601"] = formAdatok?.Ellen6_A ?? "MF",
            ["602"] = formAdatok?.Ellen6_B ?? "MF",
            ["603"] = formAdatok?.Ellen6_C ?? "MF",
            ["604"] = formAdatok?.Ellen6_D ?? "MF",
            ["605"] = formAdatok?.Ellen6_E ?? "MF",
            ["606"] = formAdatok?.Ellen6_F ?? "MF",
            ["607"] = formAdatok?.Ellen6_G ?? "MF",
            ["608"] = formAdatok?.Ellen6_H ?? "MF",
            ["609"] = formAdatok?.Ellen6_I ?? "MF",
            ["610"] = formAdatok?.Ellen6_J ?? "MF",
            ["611"] = formAdatok?.Ellen6_K ?? "MF",
            ["612"] = formAdatok?.Ellen6_L ?? "MF",
            ["613"] = formAdatok?.Ellen6_M ?? "MF",
            ["614"] = formAdatok?.Ellen6_N ?? "MF",
            ["615"] = formAdatok?.Ellen6_O ?? "MF",
            ["616"] = formAdatok?.Ellen6_P ?? "MF",
            ["601M"] = formAdatok?.Ellen6_A_M ?? "",
            ["602M"] = formAdatok?.Ellen6_B_M ?? "",
            ["603M"] = formAdatok?.Ellen6_C_M ?? "",
            ["604M"] = formAdatok?.Ellen6_D_M ?? "",
            ["605M"] = formAdatok?.Ellen6_E_M ?? "",
            ["606M"] = formAdatok?.Ellen6_F_M ?? "",
            ["607M"] = formAdatok?.Ellen6_G_M ?? "",
            ["608M"] = formAdatok?.Ellen6_H_M ?? "",
            ["609M"] = formAdatok?.Ellen6_I_M ?? "",
            ["610M"] = formAdatok?.Ellen6_J_M ?? "",
            ["611M"] = formAdatok?.Ellen6_K_M ?? "",
            ["612M"] = formAdatok?.Ellen6_L_M ?? "",
            ["613M"] = formAdatok?.Ellen6_M_M ?? "",
            ["614M"] = formAdatok?.Ellen6_N_M ?? "",
            ["615M"] = formAdatok?.Ellen6_O_M ?? "",
            ["616M"] = formAdatok?.Ellen6_P_M ?? "",
            ["Ellen6_Megjegyzes"] = formAdatok?.Ellen6_Megjegyzes ?? "",

            // === 6. OLDAL – VMBSZ ELLENŐRZÉSEK ===
            ["701"] = formAdatok?.Ellen6V_01 ?? "MF",
            ["702"] = formAdatok?.Ellen6V_02 ?? "MF",
            ["703"] = formAdatok?.Ellen6V_03 ?? "MF",
            ["704"] = formAdatok?.Ellen6V_04 ?? "MF",
            ["705"] = formAdatok?.Ellen6V_05 ?? "MF",
            ["706"] = formAdatok?.Ellen6V_06 ?? "MF",
            ["707"] = formAdatok?.Ellen6V_07 ?? "MF",
            ["708"] = formAdatok?.Ellen6V_08 ?? "MF",
            ["709"] = formAdatok?.Ellen6V_09 ?? "MF",
            ["710"] = formAdatok?.Ellen6V_10 ?? "MF",
            ["711"] = formAdatok?.Ellen6V_11 ?? "MF",
            ["712"] = formAdatok?.Ellen6V_12 ?? "MF",
            ["713"] = formAdatok?.Ellen6V_13 ?? "MF",
            ["714"] = formAdatok?.Ellen6V_14 ?? "MF",
            ["715"] = formAdatok?.Ellen6V_15 ?? "MF",
            ["716"] = formAdatok?.Ellen6V_16 ?? "MF",
            ["717"] = formAdatok?.Ellen6V_17 ?? "MF",
            ["701M"] = formAdatok?.Ellen6V_01_M ?? "",
            ["702M"] = formAdatok?.Ellen6V_02_M ?? "",
            ["703M"] = formAdatok?.Ellen6V_03_M ?? "",
            ["704M"] = formAdatok?.Ellen6V_04_M ?? "",
            ["705M"] = formAdatok?.Ellen6V_05_M ?? "",
            ["706M"] = formAdatok?.Ellen6V_06_M ?? "",
            ["707M"] = formAdatok?.Ellen6V_07_M ?? "",
            ["708M"] = formAdatok?.Ellen6V_08_M ?? "",
            ["709M"] = formAdatok?.Ellen6V_09_M ?? "",
            ["710M"] = formAdatok?.Ellen6V_10_M ?? "",
            ["711M"] = formAdatok?.Ellen6V_11_M ?? "",
            ["712M"] = formAdatok?.Ellen6V_12_M ?? "",
            ["713M"] = formAdatok?.Ellen6V_13_M ?? "",
            ["714M"] = formAdatok?.Ellen6V_14_M ?? "",
            ["715M"] = formAdatok?.Ellen6V_15_M ?? "",
            ["716M"] = formAdatok?.Ellen6V_16_M ?? "",
            ["717M"] = formAdatok?.Ellen6V_17_M ?? "",
            ["718"] = formAdatok?.Ellen6V_18 ?? "MF",
            ["719"] = formAdatok?.Ellen6V_19 ?? "MF",
            ["720"] = formAdatok?.Ellen6V_20 ?? "MF",
            ["721"] = formAdatok?.Ellen6V_21 ?? "MF",
            ["718M"] = formAdatok?.Ellen6V_18_M ?? "",
            ["719M"] = formAdatok?.Ellen6V_19_M ?? "",
            ["720M"] = formAdatok?.Ellen6V_20_M ?? "",
            ["721M"] = formAdatok?.Ellen6V_21_M ?? "",
            ["700_Megjegyzes"] = formAdatok?.Ellen6V_Megjegyzes ?? "",
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