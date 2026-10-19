namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>
/// Felülvizsgáló lista elem - csak megtekintéshez (mobil app).
/// </summary>
public class FelulvizsgaloListItemDto
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    public string Jogosultsag { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; }
    public bool VanAlairas { get; set; }
}

/// <summary>
/// Felülvizsgáló részletes adatai, csak megtekintéshez (mobil app).
/// </summary>
public class FelulvizsgaloDetailDto : FelulvizsgaloListItemDto
{
    public List<FelulvizsgaloKepzesDto> Kepzesek { get; set; } = new();
}

/// <summary>
/// Felülvizsgáló egy képzése/bizonyítványa.
/// </summary>
public class FelulvizsgaloKepzesDto
{
    public int Id { get; set; }
    public string? KepzesTipusNev { get; set; }
    public string? BizonyitvanySzam { get; set; }
    public DateTime? BizonyitvanyKelte { get; set; }
    public DateTime? BizonyitvanyLejarat { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; }
    public bool VanBizonyitvanyFajl { get; set; }
}
