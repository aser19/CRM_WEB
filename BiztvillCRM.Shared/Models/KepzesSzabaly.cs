using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Képzések közötti kapcsolatok és szabályok definiálása.
/// </summary>
public class KepzesSzabaly
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    
    /// <summary>A szabály típusa (Megújít/Felment/Előfeltétel)</summary>
    public KepzesSzabalyTipus Tipus { get; set; }
    
    /// <summary>Forrás képzéstípus (amelyik hat)</summary>
    public int ForrasKepzesTipusId { get; set; }
    public KepzesTipus? ForrasKepzesTipus { get; set; }
    
    /// <summary>Cél képzéstípus (amelyikre hat)</summary>
    public int CelKepzesTipusId { get; set; }
    public KepzesTipus? CelKepzesTipus { get; set; }
    
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;
}