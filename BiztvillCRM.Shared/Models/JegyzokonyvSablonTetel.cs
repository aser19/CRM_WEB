using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

/// <summary>Egy ellenőrzési tétel a jegyzőkönyv adott oldalán (pl. 5. oldal, 60364-6 vizsgálatok).</summary>
public class JegyzokonyvSablonTetel
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }

    /// <summary>Melyik méréstype-hoz tartozik (pl. VBF = 1).</summary>
    public int MeresTipusId { get; set; }
    public MeresTipus? MeresTipus { get; set; }

    /// <summary>Melyik wizard-oldalon jelenik meg (pl. 5, 6, 7).</summary>
    public int OldalSzam { get; set; }

    /// <summary>Kategória / csoport felirata (pl. "60364-6 Vizsgálatok", "OTSZ ellenőrzések").</summary>
    [MaxLength(200)]
    public string Kategoria { get; set; } = string.Empty;

    /// <summary>Sorrend a listán belül.</summary>
    public int Sorrend { get; set; }

    /// <summary>A tétel felirata (pl. "1.1 Tápvezeték keresztmetszet ellenőrzése").</summary>
    [MaxLength(500)]
    public string Felirat { get; set; } = string.Empty;

    /// <summary>Elérhető értékek pontosvesszővel elválasztva (pl. "MF;NMF;NA" vagy "MF;NMF").</summary>
    [MaxLength(200)]
    public string LehetsegesErtekek { get; set; } = "MF;NMF;NA";

    /// <summary>Az alapértelmezett kiválasztott érték (pl. "MF").</summary>
    [MaxLength(50)]
    public string AlapertelmezettErtek { get; set; } = "MF";

    /// <summary>Legyen-e Megjegyzés mező is a tétel mellett.</summary>
    public bool VanMegjegyzesMezo { get; set; } = true;

    /// <summary>Aktív-e (inaktív tételek nem jelennek meg a formban).</summary>
    public bool Aktiv { get; set; } = true;

    // -- Számított property --
    public List<string> ErtekLista =>
        LehetsegesErtekek.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
}