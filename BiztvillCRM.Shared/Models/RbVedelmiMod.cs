namespace BiztvillCRM.Shared.Models;

using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>Alkalmazási csoport / gázcsoport (pl. "IIA", "IIB", "IIC")</summary>
    public string? Gazcsoport { get; set; }

    /// <summary>Porcsoport (pl. "IIIA", "IIIB", "IIIC")</summary>
    public string? Porcsoport { get; set; }

    /// <summary>Hőmérséklet osztály (pl. "T6" vagy "T80°C")</summary>
    public string? HomersOsztaly { get; set; }

    /// <summary>Engedélyezett zónák, vesszővel elválasztva (pl. "1,2,21,22")</summary>
    public string? EngedelyezettZonak { get; set; }

    /// <summary>Aktív-e</summary>
    public bool Aktiv { get; set; } = true;

    /// <summary>Felülvizsgálatra vár - automatikusan létrehozott érték, amit még nem ellenőrzött az admin</summary>
    public bool FelulvizsgalasraVar { get; set; } = false;

    public DateTime Letrehozva { get; set; } = DateTime.Now;

    /// <summary>Az EngedelyezettZonak mező vesszővel tagolt listaként.</summary>
    public List<string> EngedelyezettZonakLista =>
        (EngedelyezettZonak ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
}
