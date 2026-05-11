using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Egy vizsgálatcsoport egyik tagja: megmondja, hogy melyik méréstípus szükséges,
/// milyen gyakorisággal, és kötelező-e a főhitelesítéshez.
/// </summary>
public class MeresCsoportTag
{
    public int Id { get; set; }

    public int MeresCsoportId { get; set; }
    public MeresCsoport? MeresCsoport { get; set; }

    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }

    /// <summary>Kötelező-e a főhitelesítés előtt elvégezni?</summary>
    public bool Kotelezo { get; set; } = true;

    /// <summary>Megjelenítési sorrend</summary>
    public int Sorrend { get; set; } = 0;

    [MaxLength(500)]
    public string? Megjegyzes { get; set; }
}