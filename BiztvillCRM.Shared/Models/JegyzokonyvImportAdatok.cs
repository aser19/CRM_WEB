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
    public List<AvkSor> AvkSorok { get; set; } = new();  // ← visszaállítva a valódi AvkSor-ra
}

public class HordozhatoEszkozImport
{
    public string? Sorszam { get; set; }
    public string? Eszkoznev { get; set; }
    public string? Tipus { get; set; }
    public string? GyariSzam { get; set; }
    public string? Leltariszam { get; set; }
    public string? JellemzoTeljesitmeny { get; set; }
    public string? Megtekintes { get; set; }
    public string? Folytonossag { get; set; }
    public string? Szigeteles { get; set; }
    public string? SzivargoAram { get; set; }
    public string? Megjegyzes { get; set; }
    public string? CsoportNev { get; set; }
    public int CsoportSorrend { get; set; }
}

// LeltarEszkozSor – IT eszköz leltár, külön osztályban marad
public class LeltarEszkozSor
{
    public string? Terulet { get; set; }
    public string? Rendszam { get; set; }
    public string? Megnevezes { get; set; }
    public string? Gyarto { get; set; }
    public string? Tipus { get; set; }
    public string? SorozatSzam { get; set; }
    public string? OperaciosRendszer { get; set; }
    public string? FinstalledProgramok { get; set; }
    public string? EgyebAdatok { get; set; }
}