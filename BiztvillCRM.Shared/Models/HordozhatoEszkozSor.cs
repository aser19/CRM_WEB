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
}