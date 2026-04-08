using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Email sablon egy adott értesítés típushoz.</summary>
public class EmailSablon
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public EmailErtesitesTipus Tipus { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string Targy { get; set; } = string.Empty;
    public string Szoveg { get; set; } = string.Empty;
    public bool Aktiv { get; set; } = true;

    /// <summary>Céghez tartozás - null esetén globális sablon.</summary>
    public int? CegId { get; set; }
    public Ceg? Ceg { get; set; }
}