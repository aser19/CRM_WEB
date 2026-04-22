// BiztvillCRM.Shared\Models\EszkozSablon.cs
namespace BiztvillCRM.Shared.Models;

/// <summary>Eszköz sablon (pl. "Hosszabbító sablon")</summary>
public class EszkozSablon
{
    public int Id { get; set; }
    
    /// <summary>Eszköz típus neve (pl. "Hosszabbító", "Felvonulási szekrény")</summary>
    public string EszkozTipusNev { get; set; } = "";
    
    /// <summary>Sablon neve (pl. "Hosszabbító 4 dugalj")</summary>
    public string Nev { get; set; } = "";
    
    /// <summary>Védelmi osztály (I, II, III)</summary>
    public string VedelmiOsztaly { get; set; } = "I";
    
    /// <summary>Alapértelmezett teljesítmény/feszültség</summary>
    public string AlapTeljesitmeny { get; set; } = "230V";
    
    /// <summary>Van-e alkatrészei?</summary>
    public bool VanAlkatresz { get; set; }
    
    /// <summary>Alkatrészek listája</summary>
    public List<AlkatreszSablon> Alkatreszek { get; set; } = new();
    
    /// <summary>Utolsó módosítás</summary>
    public DateTime UtolsoModositas { get; set; } = DateTime.Now;
}

/// <summary>Alkatrész sablon (pl. "Dugalj")</summary>
public class AlkatreszSablon
{
    public int Id { get; set; }
    
    /// <summary>Melyik eszközhöz tartozik</summary>
    public int EszkozSablonId { get; set; }
    public EszkozSablon? EszkozSablon { get; set; }
    
    /// <summary>Alkatrész neve (pl. "Dugalj", "Földelési pont")</summary>
    public string Nev { get; set; } = "";
    
    /// <summary>Védelmi osztály (I, II, III)</summary>
    public string VedelmiOsztaly { get; set; } = "I";
    
    /// <summary>Alapértelmezett darabszám</summary>
    public int DefaultDarabszam { get; set; } = 1;
    
    /// <summary>Minimális darabszám (validációhoz)</summary>
    public int MinDarabszam { get; set; } = 1;
    
    /// <summary>Maximális darabszám (validációhoz)</summary>
    public int MaxDarabszam { get; set; } = 10;
    
    /// <summary>Kötelező alkatrész?</summary>
    public bool Kotelezo { get; set; } = true;
    
    /// <summary>Sorrend (megjelenítéshez)</summary>
    public int Sorrend { get; set; }
}