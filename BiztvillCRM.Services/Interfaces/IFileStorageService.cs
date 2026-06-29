namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Fájlkezelő szolgáltatás interfész - fájlok mentése, törlése strukturált mappákban
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Fájl mentése strukturált mappába
    /// </summary>
    /// <param name="file">Feltöltött fájl</param>
    /// <param name="cegNev">Cég neve (mappa struktúra része)</param>
    /// <param name="felulvizsgaloNev">Felülvizsgáló neve (mappa struktúra része)</param>
    /// <param name="category">Kategória (pl.: "alairas", "bizonyitvanyok")</param>
    /// <param name="allowedExtensions">Engedélyezett fájlkiterjesztések (pl.: [".jpg", ".png", ".pdf"])</param>
    /// <param name="maxSizeMB">Maximum fájlméret MB-ban</param>
    /// <returns>Relatív fájl elérési út vagy null hiba esetén</returns>
    Task<string?> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string cegNev,
        string felulvizsgaloNev,
        string category,
        string[] allowedExtensions,
        int maxSizeMB = 5);

    /// <summary>
    /// Fájl törlése
    /// </summary>
    /// <param name="relativePath">Relatív fájl elérési út</param>
    Task<bool> DeleteFileAsync(string? relativePath);

    /// <summary>
    /// Fájl létezésének ellenőrzése
    /// </summary>
    /// <param name="relativePath">Relatív fájl elérési út</param>
    bool FileExists(string? relativePath);

    /// <summary>
    /// Teljes fájl elérési út lekérdezése
    /// </summary>
    /// <param name="relativePath">Relatív fájl elérési út</param>
    string GetFullPath(string relativePath);

    /// <summary>
    /// Fájl stream lekérdezése
    /// </summary>
    /// <param name="relativePath">Relatív fájl elérési út</param>
    Task<Stream?> GetFileStreamAsync(string? relativePath);

    /// <summary>
    /// Hitelesítés fájl mentése strukturált mappába (Cég\Ügyfél\kategória)
    /// </summary>
    /// <param name="fileStream">Fájl stream</param>
    /// <param name="fileName">Fájl neve</param>
    /// <param name="cegNev">Cég neve</param>
    /// <param name="ugyfelNev">Ügyfél neve</param>
    /// <param name="category">Kategória (pl.: "munkalapok", "bizonyitványok")</param>
    /// <param name="allowedExtensions">Engedélyezett fájlkiterjesztések</param>
    /// <param name="maxSizeMB">Maximum fájlméret MB-ban</param>
    /// <returns>Relatív fájl elérési út vagy null hiba esetén</returns>
    Task<string?> SaveHitelesitesFileAsync(
        Stream fileStream,
        string fileName,
        string cegNev,
        string ugyfelNev,
        string category,
        string[] allowedExtensions,
        int maxSizeMB = 10);
}
