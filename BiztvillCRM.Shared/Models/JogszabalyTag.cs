namespace BiztvillCRM.Shared.Models;

/// <summary>Jogszabályhoz rendelhető tag/címke.</summary>
public class JogszabalyTag
{
    public int Id { get; set; }
    public string Nev { get; set; } = string.Empty;
    /// <summary>Megjelenítési szín hex-kódban, pl. "#1565C0".</summary>
    public string Szin { get; set; } = "#607D8B";
}