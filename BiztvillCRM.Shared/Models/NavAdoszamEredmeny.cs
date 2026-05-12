namespace BiztvillCRM.Shared.Models;

public class NavAdoszamEredmeny
{
    public bool Sikeres { get; set; }
    public string? HibaSzoveg { get; set; }

    public string? Adoszam { get; set; }
    public string? CegNev { get; set; }
    public string? IranyitoSzam { get; set; }
    public string? Telepules { get; set; }
    public string? Kozterulet { get; set; }
    public string? KozteruletJellege { get; set; }
    public string? Hazszam { get; set; }

    public string FormattaltCim =>
        string.Join(" ", new[]
        {
            IranyitoSzam,
            Telepules != null ? Telepules + "," : null,
            Kozterulet,
            KozteruletJellege,
            Hazszam
        }.Where(s => !string.IsNullOrWhiteSpace(s)));
}