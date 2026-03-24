using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.DTOs;

/// <summary>Mérés listanézet adatátviteli objektuma.</summary>
public class MeresDto
{
    public int Id { get; set; }
    public string EszkozNev { get; set; } = string.Empty;
    public string MeresTipusNev { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
    public DateTime? KovetkezoDatum { get; set; }
    public string? Eredmeny { get; set; }
    public MeresStatusz Statusz { get; set; }
}
