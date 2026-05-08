namespace BiztvillCRM.Shared.Models;

public class MuszerSor
{
    public int? EszkozId { get; set; }   // ← ÚJ: visszatöltéshez kell
    public string Tipus { get; set; } = "";
    public string GyariSzam { get; set; } = "";
    public string Kalibralas { get; set; } = "";
}