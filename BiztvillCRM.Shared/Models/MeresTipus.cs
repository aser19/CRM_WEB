namespace BiztvillCRM.Shared.Models;

/// <summary>Mérés típusának törzsadatai (pl. nyomásmérés, hőmérsékletmérés).</summary>
public class MeresTipus
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public string? Leiras { get; set; }
    /// <summary>Érvényességi idő hónapokban.</summary>
    public int ErvenyessegHonap { get; set; }
}
