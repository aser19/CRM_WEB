namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Érintésvédelmi mód és osztály kombinációja (pl. I/0Ω, II/Ω, III/Ω)
/// Használat: Mérési pontok táblázatában
/// </summary>
public class ErintesvedelmiModOsztaly
{
    public int Id { get; set; }

    /// <summary>Mód/osztály megnevezése (pl. I/0Ω, II/Ω, III/Ω)</summary>
    public string Nev { get; set; } = "";

    /// <summary>Leírás (opcionális)</summary>
    public string? Leiras { get; set; }

    /// <summary>Aktív-e</summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>Sorrend megjelenítéskor</summary>
    public int Sorrend { get; set; }

    public DateTime Letrehozva { get; set; } = DateTime.Now;
}