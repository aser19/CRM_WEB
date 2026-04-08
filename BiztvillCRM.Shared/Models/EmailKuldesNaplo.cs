using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Email küldés napló - követéshez és hibakereséshez.</summary>
public class EmailKuldesNaplo
{
    public int Id { get; set; }
    public DateTime Kuldve { get; set; }

    public int? CegId { get; set; }
    public EmailErtesitesTipus Tipus { get; set; }
    public string Cimzett { get; set; } = string.Empty;
    public string Targy { get; set; } = string.Empty;
    public bool Sikeres { get; set; }
    public string? Hiba { get; set; }

    /// <summary>Hivatkozás a hitelesítésre vagy mérésre (opcionális).</summary>
    public int? HitelesitesId { get; set; }
    public int? MeresId { get; set; }
}