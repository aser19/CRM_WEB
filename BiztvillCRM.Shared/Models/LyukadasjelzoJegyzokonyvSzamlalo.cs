namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Cégenkénti és évenkénti lyukadásjelző jegyzőkönyv-sorszám számlálót tároló entitás.
/// Hasonló a MunkaszamSzamlaloho, de külön számlálót biztosít, hogy ne ütközzön a munkaszámokkal.
/// </summary>
public class LyukadasjelzoJegyzokonyvSzamlalo
{
    public int Id { get; set; }
    public int CegId { get; set; }
    public int Ev { get; set; }
    public int UtolsoSorszam { get; set; }

    public Ceg? Ceg { get; set; }
}
