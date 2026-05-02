using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
    
    /// <summary>Jegyzőkönyv prefix (pl. "VBF", "HME", "ÉV", "HK2")</summary>
    [MaxLength(10)]
    [RegularExpression(@"^[A-ZÁÉÍÓÖŐÚÜŰ0-9]+$", ErrorMessage = "A prefix csak nagybetűket és számokat tartalmazhat (pl. VBF, HME2, ÉV)!")]
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
    public List<MeresTipusJogszabaly> Jogszabalyok { get; set; } = new();

    /// <summary>
    /// Berendezés típus → alapértelmezett határidő mapping (JSON)
    /// Kulcsok: "50kW","32A","VMBSZ","RV300","RV_EGYEB","EGYEB"
    /// Értékek: "3EV","6EV","EGYEB"
    /// </summary>
    public string? BerendezesHataridoMappingJson { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Dictionary<string, string> BerendezesHataridoMapping
    {
        get => string.IsNullOrEmpty(BerendezesHataridoMappingJson)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(BerendezesHataridoMappingJson) ?? new();
        set => BerendezesHataridoMappingJson = JsonSerializer.Serialize(value);
    }
}
