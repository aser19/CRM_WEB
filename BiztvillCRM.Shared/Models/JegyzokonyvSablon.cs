namespace BiztvillCRM.Shared.Models;

/// <summary>Jegyzőkönyv sablon metaadatok.</summary>
public class JegyzokonyvSablon
{
    public string Id { get; set; } = "";           // Fájlnév kiterjesztés nélkül
    public string Nev { get; set; } = "";          // Megjelenített név
    public string FajlNev { get; set; } = "";      // Teljes fájlnév (.docx)
    public string? Leiras { get; set; }
    public string? Kategoria { get; set; }         // pl. "VBF", "Tűzvédelem", stb.
    public DateTime UtolsoModositas { get; set; }
    public bool Aktiv { get; set; } = true;
}