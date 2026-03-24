namespace BiztvillCRM.Shared.Models;

/// <summary>Vonatkozó jogszabály törzsadatai.</summary>
public class Jogszabaly
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Szam { get; set; } = string.Empty;
    public string Cim { get; set; } = string.Empty;
    public DateTime? HatalyosDatum { get; set; }
    public string? Url { get; set; }
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;
}
