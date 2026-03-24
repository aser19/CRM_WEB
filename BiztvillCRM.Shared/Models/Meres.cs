using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Egy elvégzett vagy tervezett mérés adatai.</summary>
public class Meres
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public int EszkozId { get; set; }
    public int MeresTipusId { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Eredmeny { get; set; }
    public MeresStatusz MeresStatusz { get; set; }
    public string? Megjegyzes { get; set; }

    // Navigációs propertyk
    public Eszkoz? Eszkoz { get; set; }
    public MeresTipus? MeresTipus { get; set; }
}
