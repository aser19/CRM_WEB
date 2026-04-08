using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Lejárat értesítő szolgáltatás - feldolgozza és küldi az értesítéseket.</summary>
public interface ILejaratErtesitoService
{
    /// <summary>Feldolgozza az összes lejáró hitelesítést és mérést, és küldi az értesítéseket.</summary>
    Task<ErtesitesFeldolgozasEredmeny> FeldolgozasAsync();
}

/// <summary>Feldolgozás eredménye.</summary>
public class ErtesitesFeldolgozasEredmeny
{
    public int FeldolgozottHitelesitesek { get; set; }
    public int FeldolgozottMeresek { get; set; }
    public int KuldottEmailek { get; set; }
    public int SikertelenEmailek { get; set; }
    public List<string> Hibak { get; set; } = new();
}