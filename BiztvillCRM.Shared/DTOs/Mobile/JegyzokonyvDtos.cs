namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>Jegyzőkönyv (Meres) listaelem, csak megtekintéshez (mobil app).</summary>
public class JegyzokonyvListItemDto
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public string? MeresTipusNev { get; set; }
    public string MeresStatusz { get; set; } = string.Empty;
    public string? Eredmeny { get; set; }
}

/// <summary>Jegyzőkönyv (Meres) részletes adatai, csak megtekintéshez (mobil app).</summary>
public class JegyzokonyvDetailDto : JegyzokonyvListItemDto
{
    public string? Megjegyzes { get; set; }
    public string? JegyzokonyvAdatokJson { get; set; }
    public string? VillamNemNormaAdatokJson { get; set; }
    public List<JegyzokonyvMellekletDto> Mellekletek { get; set; } = new();
}

public class JegyzokonyvMellekletDto
{
    public int Id { get; set; }
    public string Tipus { get; set; } = string.Empty;
    public string TipusNev { get; set; } = string.Empty;
    public string Szam { get; set; } = string.Empty;
    public string Statusz { get; set; } = string.Empty;
}
