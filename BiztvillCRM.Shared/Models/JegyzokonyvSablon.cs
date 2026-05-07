using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>Word fájl alapú sablon (pl. VBF_KIF_MINTA.docx) – az ISablonService használja.</summary>
public class JegyzokonyvSablon
{
    public string Id { get; set; } = string.Empty;
    public string Nev { get; set; } = string.Empty;
    public string FajlNev { get; set; } = string.Empty;
    public string Kategoria { get; set; } = string.Empty;
    public DateTime UtolsoModositas { get; set; }
    public bool Aktiv { get; set; } = true;
}