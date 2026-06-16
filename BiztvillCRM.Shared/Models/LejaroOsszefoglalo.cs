namespace BiztvillCRM.Shared.Models;

/// <summary>Egy ügyfél következő N napban lejáró tételeinek összesítője.</summary>
public class LejaroOsszefoglalo
{
    public int UgyfelId { get; set; }
    public string UgyfelNev { get; set; } = "";
    public int NapokSzama { get; set; } = 30;

    public List<Meres> Meresek { get; set; } = new();
    public List<Hitelesites> Hitelesitesek { get; set; } = new();
    public List<Karbantartas> Karbantartasok { get; set; } = new();
    public List<Kockazatertekeles> Kockazatok { get; set; } = new();
    public List<Zonaterkep> Zonaterkepek { get; set; } = new();

    public bool VanTetel => Meresek.Any() || Hitelesitesek.Any() ||
                            Karbantartasok.Any() || Kockazatok.Any() || Zonaterkepek.Any();
    public int OsszesSzam => Meresek.Count + Hitelesitesek.Count +
                             Karbantartasok.Count + Kockazatok.Count + Zonaterkepek.Count;
}