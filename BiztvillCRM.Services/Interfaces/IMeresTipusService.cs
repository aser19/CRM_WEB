using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresTipusService
{
    Task<List<MeresTipus>> GetAllAsync();
    Task<MeresTipus?> GetByIdAsync(int id);
    
    // ✅ ÚJ METÓDUS
    Task<List<MeresTipus>> GetAllWithKovetelemenyekAsync();
    Task UpdateWithKovetelemenyekAsync(MeresTipus tipus);
    
    Task<int> CreateAsync(MeresTipus tipus);
    Task UpdateAsync(MeresTipus tipus);
    Task DeleteAsync(int id);

    Task<List<MeresTipusJogszabaly>> GetJogszabalyokByTipusIdAsync(int meresTipusId);
    Task MentJogszabalyHozzarendelesekAsync(int meresTipusId, List<JogszabalyHozzarendeles> hozzarendelesek);

    /// <summary>Egy adott jogszabályhoz/szabványhoz tartozó összes méréstípus-hozzárendelés lekérdezése.</summary>
    Task<List<MeresTipusJogszabaly>> GetMeresTipusHozzarendelesekByJogszabalyIdAsync(int jogszabalyId);

    /// <summary>
    /// Egy adott jogszabályhoz/szabványhoz tartozó méréstípus-hozzárendelések mentése.
    /// A meglévő hozzárendeléseket törli, majd a megadott listát menti be helyettük.
    /// </summary>
    Task MentMeresTipusHozzarendelesekAsync(int jogszabalyId, List<MeresTipusHozzarendeles> hozzarendelesek);
}

/// <summary>Egy méréstípushoz rendelt jogszabály/szabvány hozzárendelési állapota.</summary>
public record JogszabalyHozzarendeles(int JogszabalyId, bool AlapertelmezettKivalasztva);

/// <summary>Egy jogszabályhoz/szabványhoz rendelt méréstípus hozzárendelési állapota.</summary>
public record MeresTipusHozzarendeles(int MeresTipusId, bool AlapertelmezettKivalasztva);