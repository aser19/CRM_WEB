namespace BiztvillCRM.Shared.Models;

public class UgyfelLekerdezesViewModel
{
    public string UgyfelNev { get; set; } = "";
    public List<TelephelyAdatok> Telephelyek { get; set; } = new();
}

public class TelephelyAdatok
{
    public string Nev { get; set; } = "";
    public string Cim { get; set; } = "";
    public List<MeresOsszefoglalo> Meresek { get; set; } = new();
    public List<HitelesitesOsszefoglalo> Hitelesitesek { get; set; } = new();
    public List<KarbantartasOsszefoglalo> Karbantartasok { get; set; } = new();
    public List<ZonaterkepOsszefoglalo> Zonaterkepek { get; set; } = new();
    public List<KockazatertekelesOsszefoglalo> Kockazatertekelesek { get; set; } = new();
}

public class MeresOsszefoglalo
{
    public DateTime Datum { get; set; }
    public string Tipus { get; set; } = "";
    public string? Eredmeny { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string Statusz { get; set; } = "";
}

public class HitelesitesOsszefoglalo
{
    public string EszkozTipusNev { get; set; } = "";
    public string? EszkozAzonosito { get; set; }
    public int Darabszam { get; set; }
    public DateTime Datum { get; set; }
    public DateTime? LejaratDatum { get; set; }

    /// <summary>Ha van hitelesítési csoport rendelve, itt jönnek a közbenső vizsgálatok</summary>
    public List<CsoportTagLejaratReszlet> KozbensoVizsgalatok { get; set; } = new();

    /// <summary>Ha egyes eszközöknek eltérő lejáratuk van (pl. javítás után), itt jönnek</summary>
    public List<HitelesitesReszlet> EgyediLejaratok { get; set; } = new();
}

public class KarbantartasOsszefoglalo
{
    public string TipusNev { get; set; } = "";
    public DateTime? KovetkezoDatum { get; set; }
    public bool Elvegezve { get; set; }
}

public class ZonaterkepOsszefoglalo
{
    public string Megnevezes { get; set; } = "";
    public string ZonaTipus { get; set; } = "";
    public DateTime? ErvenyessegVege { get; set; }
    public bool Aktiv { get; set; }
}

public class KockazatertekelesOsszefoglalo
{
    public string Megnevezes { get; set; } = "";
    public DateTime ErtekelesDatuma { get; set; }
    public DateTime? KovetkezoFelulvizsgalat { get; set; }
    public string KockazatiSzint { get; set; } = "";
    public string Statusz { get; set; } = "";
}