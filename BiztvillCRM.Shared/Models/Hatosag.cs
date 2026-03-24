namespace BiztvillCRM.Shared.Models;

/// <summary>Hitelesítő hatóság törzsadatai.</summary>
public class Hatosag
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Rovidites { get; set; }
    public string? Cim { get; set; }
    public string? Weboldal { get; set; }
    public bool Aktiv { get; set; } = true;
}
