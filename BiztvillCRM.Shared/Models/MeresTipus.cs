using System.ComponentModel.DataAnnotations;

namespace BiztvillCRM.Shared.Models;

public class MeresTipus
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nev { get; set; } = "";
    
    /// <summary>Leírás (opcionális)</summary>
    [MaxLength(1000)]
    public string? Leiras { get; set; }
    
    /// <summary>Érvényesség hónapokban (pl. 12 = 1 év, 24 = 2 év)</summary>
    public int? ErvenyessegHonap { get; set; }
    
    /// <summary>Jegyzőkönyv prefix (pl. "VBF", "HME", "ÉV")</summary>
    [MaxLength(10)]
    public string? JegyzokonyvPrefix { get; set; }
    
    /// <summary>Word sablon azonosító (pl. "VBF_KIF_MINTA")</summary>
    [MaxLength(50)]
    public string? SablonId { get; set; }
    
    /// <summary>Azure Document Intelligence OCR Model ID (pl. "JegyzokonyvFelismeres_HME")</summary>
    [MaxLength(100)]
    public string? OcrModelId { get; set; }
    
    public bool Aktiv { get; set; } = true;
    
    public DateTime Letrehozva { get; set; } = DateTime.UtcNow;
    public DateTime? Modositva { get; set; }
    
    /// <summary>Képzési követelmények</summary>
    public List<MeresTipusKepzesKovetelemeny> KepzesKovetelemenyei { get; set; } = new();
}
