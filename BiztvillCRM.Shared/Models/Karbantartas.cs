namespace BiztvillCRM.Shared.Models;

/// <summary>Karbantartási esemény - ügyfélhez és telephelyhez tartozik.</summary>
public class Karbantartas
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    // Cég (tenant)
    public int CegId { get; set; }
    public Ceg? Ceg { get; set; }

    // Ügyfél és telephely kapcsolat
    public int UgyfelId { get; set; }
    public Ugyfel? Ugyfel { get; set; }
    
    public int TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }

    // Típus kapcsolat
    public int KarbantartasTipusId { get; set; }
    public KarbantartasTipus? KarbantartasTipus { get; set; }

    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Leiras { get; set; }
    public string? Elvegzo { get; set; }
    public bool Elvegezve { get; set; } = false;
}