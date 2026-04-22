// BiztvillCRM.Shared\Models\JegyzokonyvImportAdatok.cs
namespace BiztvillCRM.Shared.Models;

public class JegyzokonyvImportAdatok
{
    public string? JegyzokonyvSzam { get; set; }
    public string? DolgozoNeve { get; set; }
    public string? ForgalmiRendszam { get; set; }
    public string? Megrendelo { get; set; }
    public string? UzemiKisero { get; set; }
    public string? KapcsolatTarto { get; set; }
    public string? VizsgalatHelye { get; set; }
    public List<HordozhatoEszkozImport> Eszkozok { get; set; } = new();
}

public class HordozhatoEszkozImport
{
    public string? Sorszam { get; set; }
    public string? Eszkoznev { get; set; }
    public string? Tipus { get; set; }
    public string? GyariSzam { get; set; }
    public string? Leltariszam { get; set; }
    
    // ✅ ÚJ MEZŐK
    public string? JellemzoTeljesitmeny { get; set; } // 230V, 400V stb.
    public string? Megtekintes { get; set; } // MF, NMF, KSZ
    public string? Folytonossag { get; set; } // Ω
    public string? Szigeteles { get; set; } // MΩ
    public string? SzivargoAram { get; set; } // mA
    public string? Megjegyzes { get; set; }
    
    // Hierarchikus eszközökhöz
    public string? CsoportNev { get; set; }
    public int CsoportSorrend { get; set; }
}