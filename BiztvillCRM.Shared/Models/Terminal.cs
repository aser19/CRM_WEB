namespace BiztvillCRM.Shared.Models;

/// <summary>Terminál/végpont törzsadatai.</summary>
public class Terminal
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Azonosito { get; set; }
    public string? IpCim { get; set; }
    public int? TelephelyId { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;

    // Navigációs property
    public Telephely? Telephely { get; set; }
}