using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiztvillCRM.Shared.Models;

/// <summary>Felhasználó - kibővített Identity user.</summary>
public class Felhasznalo : IdentityUser
{
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }
    public DateTime? UtolsoBelepes { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Beosztas { get; set; }
    public string? Telefon { get; set; }
    public bool Aktiv { get; set; } = true;

    /// <summary>Elsődleges / alapértelmezett cég (visszafelé kompatibilitás).</summary>
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    /// <summary>Összes kezelt cég (many-to-many).</summary>
    public List<FelhasznaloCeg> Cegek { get; set; } = new();

    /// <summary>Aktív/kiválasztott cég session-szinten.</summary>
    [NotMapped]
    public int? AktualisCegId { get; set; }
}