namespace BiztvillCRM.Shared.Models;

/// <summary>Képzés/oktatás esemény adatai.</summary>
public class Kepzes
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public DateTime? Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public string? Resztvevo { get; set; }
    public string? Megjegyzes { get; set; }
}
