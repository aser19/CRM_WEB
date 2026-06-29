using BiztvillCRM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BiztvillCRM.Services;

/// <summary>
/// Fájlkezelő szolgáltatás - fájlok mentése, törlése strukturált mappákban
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _uploadsPath;

    public FileStorageService(string contentRootPath, ILogger<FileStorageService> logger)
    {
        _logger = logger;
        // Uploads mappa a projekt gyökerében
        _uploadsPath = Path.Combine(contentRootPath, "Uploads");

        // Uploads mappa létrehozása, ha nem létezik
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<string?> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string cegNev,
        string felulvizsgaloNev,
        string category,
        string[] allowedExtensions,
        int maxSizeMB = 5)
    {
        try
        {
            // Fájlnév és kiterjesztés validálása
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Nem engedélyezett fájlkiterjesztés: {Extension}", extension);
                return null;
            }

            // BrowserFileStream esetén a stream-et először memóriába kell másolni
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Fájlméret ellenőrzése
            if (memoryStream.Length > maxSizeMB * 1024 * 1024)
            {
                _logger.LogWarning("A fájl túl nagy: {Size} MB", memoryStream.Length / 1024 / 1024);
                return null;
            }

            // Mappa struktúra létrehozása: Uploads\CégNév\FelülvizsgálóNév\Kategória
            var safeCegNev = MakeSafeFileName(cegNev);
            var safeFelulvizsgaloNev = MakeSafeFileName(felulvizsgaloNev);
            var safeCategory = MakeSafeFileName(category);

            var directoryPath = Path.Combine(_uploadsPath, safeCegNev, safeFelulvizsgaloNev, safeCategory);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Egyedi fájlnév generálása (timestamp + eredeti név)
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeFileName = MakeSafeFileName(Path.GetFileNameWithoutExtension(fileName));
            var uniqueFileName = $"{timestamp}_{safeFileName}{extension}";
            var fullPath = Path.Combine(directoryPath, uniqueFileName);

            // Fájl mentése
            using (var fileStreamOut = new FileStream(fullPath, FileMode.Create))
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(fileStreamOut);
            }

            // Relatív útvonal visszaadása
            var relativePath = Path.Combine(safeCegNev, safeFelulvizsgaloNev, safeCategory, uniqueFileName);
            _logger.LogInformation("Fájl sikeresen mentve: {Path}", relativePath);

            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba a fájl mentése során: {FileName}", fileName);
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        try
        {
            var fullPath = Path.Combine(_uploadsPath, relativePath);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("Fájl törölve: {Path}", relativePath);
                return true;
            }

            _logger.LogWarning("A törlendő fájl nem létezik: {Path}", relativePath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba a fájl törlése során: {Path}", relativePath);
            return false;
        }
    }

    public bool FileExists(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        var fullPath = Path.Combine(_uploadsPath, relativePath);
        return File.Exists(fullPath);
    }

    public string GetFullPath(string relativePath)
    {
        return Path.Combine(_uploadsPath, relativePath);
    }

    public async Task<Stream?> GetFileStreamAsync(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return null;

        try
        {
            var fullPath = Path.Combine(_uploadsPath, relativePath);
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("A fájl nem létezik: {Path}", relativePath);
                return null;
            }

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba a fájl stream lekérdezése során: {Path}", relativePath);
            return null;
        }
    }

    public async Task<string?> SaveHitelesitesFileAsync(
        Stream fileStream,
        string fileName,
        string cegNev,
        string ugyfelNev,
        string category,
        string[] allowedExtensions,
        int maxSizeMB = 10)
    {
        try
        {
            // Fájlnév és kiterjesztés validálása
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Nem engedélyezett fájlkiterjesztés: {Extension}", extension);
                return null;
            }

            // BrowserFileStream esetén a stream-et először memóriába kell másolni
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Fájlméret ellenőrzése
            if (memoryStream.Length > maxSizeMB * 1024 * 1024)
            {
                _logger.LogWarning("A fájl túl nagy: {Size} MB", memoryStream.Length / 1024 / 1024);
                return null;
            }

            // Mappa struktúra létrehozása: Uploads\CégNév\ÜgyfélNév\Kategória
            var safeCegNev = MakeSafeFileName(cegNev);
            var safeUgyfelNev = MakeSafeFileName(ugyfelNev);
            var safeCategory = MakeSafeFileName(category);

            var directoryPath = Path.Combine(_uploadsPath, safeCegNev, safeUgyfelNev, safeCategory);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Egyedi fájlnév generálása (timestamp + eredeti név)
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeFileName = MakeSafeFileName(Path.GetFileNameWithoutExtension(fileName));
            var uniqueFileName = $"{timestamp}_{safeFileName}{extension}";
            var fullPath = Path.Combine(directoryPath, uniqueFileName);

            // Fájl mentése
            using (var fileStreamOut = new FileStream(fullPath, FileMode.Create))
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(fileStreamOut);
            }

            // Relatív útvonal visszaadása
            var relativePath = Path.Combine(safeCegNev, safeUgyfelNev, safeCategory, uniqueFileName);
            _logger.LogInformation("Hitelesítés fájl sikeresen mentve: {Path}", relativePath);

            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba a hitelesítés fájl mentése során: {FileName}", fileName);
            return null;
        }
    }

    /// <summary>
    /// Biztonságos fájlnév/mappanév készítése (nem engedélyezett karakterek eltávolítása)
    /// </summary>
    private static string MakeSafeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return safeName.Trim();
    }
}
