namespace BiztvillCRM.Shared.Models;

/// <summary>Globális SMTP szerver beállítások (csak Admin szerkesztheti).</summary>
public class SmtpBeallitas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string SzerverCim { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool SslHasznalata { get; set; } = true;
    public string FelhasznaloNev { get; set; } = string.Empty;
    public string Jelszo { get; set; } = string.Empty;
    public string KuldoNev { get; set; } = string.Empty;
    public string KuldoEmail { get; set; } = string.Empty;
    public bool Aktiv { get; set; } = true;
}