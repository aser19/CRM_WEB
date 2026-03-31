using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Egy elvégzett vagy tervezett mérés adatai.</summary>
public class Meres
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    // Ügyfél és telephely kapcsolat (Eszköz helyett)
    public int UgyfelId { get; set; }
    public int TelephelyId { get; set; }
    
    public int MeresTipusId { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Eredmeny { get; set; }
    public MeresStatusz MeresStatusz { get; set; }
    public string? Megjegyzes { get; set; }

    // Navigációs propertyk
    public Ugyfel? Ugyfel { get; set; }
    public Telephely? Telephely { get; set; }
    public MeresTipus? MeresTipus { get; set; }
}
