namespace BiztvillCRM.Shared.Dtos.Mobile;

/// <summary>Hitelesítés listaelem, csak megtekintéshez (mobil app).</summary>
public class HitelesitesListItemDto
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }
    public string? UgyfelNev { get; set; }
    public string? TelephelyNev { get; set; }
    public string? EszkozTipusNev { get; set; }
    public string? EszkozAzonosito { get; set; }
    public string HitelesitesStatusz { get; set; } = string.Empty;
    public bool VanMunkalap { get; set; }
    public bool VanBizonyitvany { get; set; }
}

/// <summary>Hitelesítés részletes adatai, csak megtekintéshez (mobil app).</summary>
public class HitelesitesDetailDto : HitelesitesListItemDto
{
    public string? Megjegyzes { get; set; }
    public string? HatosagNev { get; set; }
    public int MunkalapFajlokSzama { get; set; }
    public int BizonyitvanyFajlokSzama { get; set; }
}
