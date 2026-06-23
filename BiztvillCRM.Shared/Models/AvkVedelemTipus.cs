using System.ComponentModel.DataAnnotations.Schema;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Áramvédő kapcsoló (RCD/RCCB/RCBO) típus és paraméterei.
/// </summary>
public class AvkVedelemTipus
{
    public int Id { get; set; }

    /// <summary>Típus megnevezése (pl. F202A-25/0,03, DS201 C16 A30)</summary>
    public string Nev { get; set; } = "";

    /// <summary>ÁVK típuskód: AC, A, B, F</summary>
    public string TipusKod { get; set; } = "AC";

    /// <summary>Névleges áram In [A]</summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal In { get; set; }

    /// <summary>Névleges kioldóáram IΔn [mA]</summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal IDn { get; set; }

    /// <summary>Névleges feszültség Un [V]</summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal Un { get; set; } = 230;

    /// <summary>Pólusszám (1, 2, 3, 4)</summary>
    public int Polusszam { get; set; } = 2;

    /// <summary>Leírás (opcionális)</summary>
    public string? Leiras { get; set; }

    /// <summary>Aktív-e</summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>Felülvizsgálatra vár - automatikusan létrehozott eszköz</summary>
    public bool FelulvizsgalasraVar { get; set; } = false;

    public DateTime Letrehozva { get; set; } = DateTime.Now;
}