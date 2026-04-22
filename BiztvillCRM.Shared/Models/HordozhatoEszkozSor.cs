// BiztvillCRM.Shared\Models\HordozhatoEszkozSor.cs
namespace BiztvillCRM.Shared.Models;

public class HordozhatoEszkozSor
{
    public int Sorsz { get; set; }
    public string Megnevezes { get; set; } = "";
    public string Tipus { get; set; } = "";
    public string Azonosito { get; set; } = "";
    
    /// <summary>Védelmi osztály: I, II, III</summary>
    public string VedelmiOsztaly { get; set; } = "I";
    
    /// <summary>Teljesítmény/Feszültség (pl. 230V, 400V)</summary>
    public string Telj { get; set; } = "230V";
    
    public string Megtekint { get; set; } = "MF";
    
    /// <summary>Folytonosság (csak I. osztálynál)</summary>
    public string Folyt { get; set; } = "";
    
    public string Szigell { get; set; } = "";
    public string Szivargo { get; set; } = "";
    public string Megjegyzes { get; set; } = "";
    
    /// <summary>I. osztálynál kell a Folyt mező</summary>
    public bool KellFolyt => VedelmiOsztaly == "I";

    // ÚJ: room selection (UI-only for the portable list)
    public int? HelyisegId { get; set; }
    public string HelyisegNev { get; set; } = "";

    // ÚJ MEZŐK:
    
    /// <summary>Főeszköz ID (ha van szülő)</summary>
    public int? ParentEszkozId { get; set; }
    
    /// <summary>Csoportosítás neve (pl. "Hosszabbító HOS-2025-001")</summary>
    public string? CsoportNev { get; set; }
    
    /// <summary>Csoporton belüli sorrend (0 = főeszköz, 1,2,3... = alkatrészek)</summary>
    public int CsoportSorrend { get; set; }
    
    /// <summary>Ez egy alkatrész?</summary>
    public bool IsAlkatresz => CsoportSorrend > 0;
    
    /// <summary>Ez egy főeszköz (van alkatrésze)?</summary>
    public bool IsFoEszkoz => !string.IsNullOrEmpty(CsoportNev) && CsoportSorrend == 0;
    
    /// <summary>Alkatrészek darabszáma (csak UI-hoz, generálásnál használt)</summary>
    public int AlkatreszDarabszam { get; set; } = 1;
}