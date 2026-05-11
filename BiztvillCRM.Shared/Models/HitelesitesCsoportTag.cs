using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy hitelesítési csoport egyik közbenső vizsgálata.
/// Pl. "Tömörségi vizsgálat – 60 hónap, kötelező".
/// </summary>
public class HitelesitesCsoportTag
{
    public int Id { get; set; }

    public int HitelesitesCsoportId { get; set; }
    public HitelesitesCsoport? HitelesitesCsoport { get; set; }

    /// <summary>A közbenső vizsgálat eszköztípusa (pl. Tartály tömörségi vizsgálat)</summary>
    public int EszkozTipusId { get; set; }
    public EszkozTipus? EszkozTipus { get; set; }

    /// <summary>Kötelező-e a főhitelesítés feltételeként?</summary>
    public bool Kotelezo { get; set; } = true;

    /// <summary>Megjelenítési sorrend</summary>
    public int Sorrend { get; set; } = 0;

    [MaxLength(500)]
    public string? Megjegyzes { get; set; }
}