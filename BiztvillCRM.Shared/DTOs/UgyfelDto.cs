using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.DTOs;

/// <summary>Ügyfél listanézet adatátviteli objektuma.</summary>
public class UgyfelDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string? Adoszam { get; set; }
    public string? Cim { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public UgyfelTipus UgyfelTipus { get; set; }
    public bool Aktiv { get; set; }
    /// <summary>Az ügyfélhez tartozó telephelyek száma.</summary>
    public int TelephelyekSzama { get; set; }
}
