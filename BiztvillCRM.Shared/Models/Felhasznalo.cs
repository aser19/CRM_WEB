using Microsoft.AspNetCore.Identity;

namespace BiztvillCRM.Shared.Models;

/// <summary>Felhasználó - kibővített Identity user.</summary>
public class Felhasznalo : IdentityUser
{
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }
    public DateTime? UtolsoBelepes { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Beosztas { get; set; }
    public new string? Telefon { get; set; }  // 'new' mert IdentityUser-ben is van PhoneNumber
    public bool Aktiv { get; set; } = true;

    // Céghez tartozás (tenant)
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;
}