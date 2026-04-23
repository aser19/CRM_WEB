// BiztvillCRM.Shared\Models\EszkozSablon.cs
namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Eszköz sablon alkatrészekkel - admin felhasználók által létrehozott sablonok
/// </summary>
public class EszkozSablon
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    /// <summary>
    /// Cég azonosító (NULL = globális admin sablon, egyébként cég-specifikus)
    /// </summary>
    public int? CegId { get; set; }  // ✅ NULLABLE
    
    /// <summary>Főeszköz megnevezése</summary>
    public string Megnevezes { get; set; } = string.Empty;
    
    /// <summary>Főeszköz típusa</summary>
    public string? Tipus { get; set; }
    
    /// <summary>Főeszköz gyári száma (sablon)</summary>
    public string? Azonosito { get; set; }
    
    /// <summary>Alapértelmezett védelmi osztály</summary>
    public string VedelmiOsztaly { get; set; } = "I";
    
    /// <summary>Alapértelmezett feszültség</summary>
    public string Telj { get; set; } = "230V";
    
    /// <summary>Alapértelmezett szemrevétel</summary>
    public string Megtekint { get; set; } = "MF";
    
    /// <summary>Aktív-e a sablon</summary>
    public bool Aktiv { get; set; } = true;
    
    /// <summary>Megjegyzés a sablonhoz</summary>
    public string? Megjegyzes { get; set; }

    // Navigációs property
    public Ceg? Ceg { get; set; }
    public List<EszkozSablonAlkatresz> Alkatreszek { get; set; } = new();
}

/// <summary>
/// Sablonhoz tartozó alkatrész
/// </summary>
public class EszkozSablonAlkatresz
{
    public int Id { get; set; }
    public int EszkozSablonId { get; set; }
    
    /// <summary>Sorrend a sablonon belül</summary>
    public int Sorrend { get; set; }
    
    /// <summary>Alkatrész megnevezése</summary>
    public string Megnevezes { get; set; } = string.Empty;
    
    /// <summary>Alkatrész típusa</summary>
    public string? Tipus { get; set; }
    
    /// <summary>Alkatrész gyári száma (sablon)</summary>
    public string? Azonosito { get; set; }
    
    /// <summary>Alkatrész védelmi osztálya (alapértelmezett: I)</summary>
    public string VedelmiOsztaly { get; set; } = "I";
    
    /// <summary>Alkatrész feszültsége (alapértelmezett: 230V)</summary>
    public string Telj { get; set; } = "230V";
    
    /// <summary>Alkatrész szemrevétele (alapértelmezett: MF)</summary>
    public string Megtekint { get; set; } = "MF";

    // Navigációs property
    public EszkozSablon? EszkozSablon { get; set; }
}