namespace BiztvillCRM.Shared.Models;

public class Kockazatertekeles
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Megnevezes { get; set; } = string.Empty;
    public DateTime ErtekelesDatuma { get; set; }
    public DateTime? KovetkezoFelulvizsgalat { get; set; }
    public KockazatiSzint KockazatiSzint { get; set; } = KockazatiSzint.Alacsony;
    public string? Leiras { get; set; }
    public string? Intezkedesek { get; set; }
    public string? FelelosNeve { get; set; }
    public KockazatStatusz Statusz { get; set; } = KockazatStatusz.Folyamatban;
    public bool Aktiv { get; set; } = true;

    // Ügyfél, Telephely, Cég
    public int UgyfelId { get; set; }
    public Ugyfel Ugyfel { get; set; } = null!;
    
    public int? TelephelyId { get; set; }
    public Telephely? Telephely { get; set; }
    
    public int CegId { get; set; }
    public Ceg Ceg { get; set; } = null!;
}

public enum KockazatiSzint
{
    Alacsony = 1,
    Kozepes = 2,
    Magas = 3,
    Kritikus = 4
}

public enum KockazatStatusz
{
    Folyamatban = 0,
    Lezart = 1,
    FelulvizsgalatraVar = 2
}