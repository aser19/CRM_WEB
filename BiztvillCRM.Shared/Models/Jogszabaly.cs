using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Jogszabály/szabvány törzsadatai.</summary>
public class Jogszabaly
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Szam { get; set; } = string.Empty;
    public string Cim { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public JogszabalyTipus Tipus { get; set; } = JogszabalyTipus.Jogszabaly;
    public DateTime? HatalyosKezdet { get; set; }
    public DateTime? HatalyosVege { get; set; }
    public string? Url { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;

    /// <summary>Jelenleg hatályos-e (nincs vége dátum vagy a vége a jövőben van)</summary>
    public bool JelenlegHatalyos => HatalyosVege == null || HatalyosVege > DateTime.Today;
}