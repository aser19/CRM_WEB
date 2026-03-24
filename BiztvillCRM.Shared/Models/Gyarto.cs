namespace BiztvillCRM.Shared.Models;

/// <summary>Eszközgyártó törzsadatai.</summary>
public class Gyarto
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Orszag { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; } = true;
}
