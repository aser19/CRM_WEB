namespace BiztvillCRM.Shared.Models;

/// <summary>
/// Rb (robbanásbiztos) berendezések "Védelmi mód" típusa (pl. "Ex ia IIC T4", "Ex d IIB T6").
/// </summary>
public class RbVedelmiMod
{
    public int Id { get; set; }

    /// <summary>Védelmi mód megnevezése (pl. "Ex ia IIC T4")</summary>
    public string Nev { get; set; } = "";

    /// <summary>Leírás (opcionális)</summary>
    public string? Leiras { get; set; }

    /// <summary>Aktív-e</summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>Felülvizsgálatra vár - automatikusan létrehozott érték, amit még nem ellenőrzött az admin</summary>
    public bool FelulvizsgalasraVar { get; set; } = false;

    public DateTime Letrehozva { get; set; } = DateTime.Now;
}
