using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

public class MeresTipus
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nev { get; set; } = "";
    
    [MaxLength(1000)]
    public string? Leiras { get; set; }
    
    public int? ErvenyessegHonap { get; set; }
    
    [MaxLength(10)]
    [RegularExpression(@"^[A-ZÁÉÍÓÖŐÚÜŰ0-9]+$", ErrorMessage = "A prefix csak nagybetűket és számokat tartalmazhat!")]
    public string? JegyzokonyvPrefix { get; set; }
    
    [MaxLength(50)]
    public string? SablonId { get; set; }
    
    [MaxLength(100)]
    public string? OcrModelId { get; set; }

    [MaxLength(20)]
    public string? MellekletTipusKod { get; set; }

    /// <summary>
    /// Főmérés = önálló sablont kaphat. Ha false, csak kiegészítő jegyzőkönyv (pl. Hurokimpedancia).
    /// </summary>
    public bool FoMeres { get; set; } = true;

    public bool Aktiv { get; set; } = true;
    
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }
    
    public List<MeresTipusKepzesKovetelemeny> KepzesKovetelemenyei { get; set; } = new();
    public List<MeresTipusJogszabaly> Jogszabalyok { get; set; } = new();
}
