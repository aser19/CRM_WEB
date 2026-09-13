using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IMeresService
{
    Task<List<Meres>> GetAllAsync(bool includeInaktivak = false);
    Task<List<Meres>> GetInaktivakAsync();
    Task<Meres?> GetByIdAsync(int id);

    /// <summary>
    /// Tenant-szűrés nélkül tölti be a mérést Id alapján. Kizárólag olyan publikus, token-alapú
    /// lekérdezésekhez használható, ahol a hozzáférés-ellenőrzés már a token/ügyfél egyeztetésével megtörtént
    /// (pl. anonim "lekérdezés" oldal PDF letöltés), mert ott nincs bejelentkezett felhasználó / cég-kontextus.
    /// </summary>
    Task<Meres?> GetByIdPublicAsync(int id);
    Task<Meres> CreateAsync(Meres meres);
    Task<Meres> UpdateAsync(Meres meres);
    Task DeleteAsync(int id);

    Task<JegyzokonyvAdatok?> BetoltJegyzokonyvAdatokAsync(int meresId);
    Task MentesJegyzokonyvAdatokkalAsync(int meresId, JegyzokonyvAdatok adatok);
    Task MentesJegyzokonyvAdatokkalEsStatuszAsync(int meresId, JegyzokonyvAdatok adatok, MeresStatusz statusz, string? eredmeny);
    Task StatuszFrissitesAsync(int meresId, MeresStatusz statusz, string? eredmeny);

    /// <summary>
    /// Ellenőrzi, hogy létezik-e már hasonló mérés (duplikáció).
    /// </summary>
    Task<Meres?> EllenorizDuplikaciot(int ugyfelId, int telephelyId, int meresTipusId, DateTime ujMeresDatum);

    /// <summary>
    /// Inaktívvá tesz egy mérést.
    /// </summary>
    Task InaktivvaTesz(int meresId);

    /// <summary>
    /// Be- vagy kikapcsolja egy mérés aktív állapotát.
    /// </summary>
    Task AktivAllapotValtasAsync(int meresId, bool aktiv);

    /// <summary>
    /// Ellenőrzi, hogy létezik-e ugyanazokkal az adatokkal (ügyfél, telephely, méréstípus) rendelkező,
    /// az adott mérésnél frissőbb dátumú aktív mérés.
    /// </summary>
    Task<bool> VanFrissebbAktivAsync(int meresId);

    /// <summary>
    /// Duplikál egy meglévő mérést: minden adata megegyezik az eredetivel, a megjegyzésébe
    /// bekerül a "- copy" toldalék. Az új mérés aktívan, önálló Id-vel jön létre.
    /// </summary>
    Task<Meres> DuplikalAsync(int meresId);

    // --- Tagek ---
    Task<List<MeresTag>> GetAllTagekAsync();
    Task<MeresTag> CreateTagAsync(MeresTag tag);
    Task UpdateTagAsync(MeresTag tag);
    Task DeleteTagAsync(int id);
    Task SetTagekAsync(int meresId, List<int> tagIds);
}
