namespace BiztvillCRM.Shared.Models;

public class MunkavedelmiOktatas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Megnevezes { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    public DateTime OktatasDatuma { get; set; }
    public DateTime? KovetkezoOktatas { get; set; }
    public int IdoszakHonap { get; set; } = 12;
    public string? OktatoNeve { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;

    // Ügyfél, Telephely, Cég (mint a mérésnél)
    public int UgyfelId { get; set; }
    public Ugyfel Ugyfel { get; set; } = null!;
    
    public int? TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }
    
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;

    // Résztvevők (szöveges)
    public List<MunkavedelmiOktatasResztvevo> Resztvevok { get; set; } = new();
}

/// <summary>
/// Oktatáson résztvevő (szöveges, jegyzőkönyvhöz).
/// </summary>
public class MunkavedelmiOktatasResztvevo
{
    public int Id { get; set; }
    public int MunkavedelmiOktatasId { get; set; }
    public MunkavedelmiOktatas MunkavedelmiOktatas { get; set; } = null!;
    
    public string Nev { get; set; } = string.Empty;
    public string? Beosztas { get; set; }
    public string? Megjegyzes { get; set; }
}