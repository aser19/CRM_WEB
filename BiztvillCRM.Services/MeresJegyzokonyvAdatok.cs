namespace BiztvillCRM.Shared.Models;

/// <summary>Mérési jegyzőkönyv adatai PDF generáláshoz.</summary>
public class MeresJegyzokonyvAdatok
{
    public string JegyzokonyvSzam { get; set; } = string.Empty;
    public DateTime KiallitasDatum { get; set; } = DateTime.Today;
    
    // Ügyfél adatok
    public string UgyfelNev { get; set; } = string.Empty;
    public string UgyfelCim { get; set; } = string.Empty;
    public string? UgyfelAdoszam { get; set; }
    
    // Telephely adatok
    public string TelephelyNev { get; set; } = string.Empty;
    public string TelephelyCim { get; set; } = string.Empty;
    
    // Mérés adatok
    public string MeresTipusNev { get; set; } = string.Empty;
    public DateTime MeresDatum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Eredmeny { get; set; }
    public string? Megjegyzes { get; set; }
    
    // Mért értékek (12+ oldalhoz)
    public List<MertErtek> MertErtekek { get; set; } = [];
    
    // Aláírások
    public string? MeroNeve { get; set; }
    public string? UgyfelKepviseloNeve { get; set; }
    public byte[]? MeroAlairasKep { get; set; }
    public byte[]? UgyfelAlairasKep { get; set; }
}

public class MertErtek
{
    public int Sorszam { get; set; }
    public string MerespontNev { get; set; } = string.Empty;
    public string? MertErtek { get; set; }
    public string? Egyseg { get; set; }
    public string? HatarErtek { get; set; }
    public bool Megfelelt { get; set; }
    public string? Megjegyzes { get; set; }
}