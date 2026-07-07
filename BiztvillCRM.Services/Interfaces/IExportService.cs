using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Adatok exportálása Excel és PDF formátumba.</summary>
public interface IExportService
{
    /// <summary>Hitelesítések exportálása Excel formátumba.</summary>
    Task<byte[]> ExportHitelesitesekExcelAsync(List<Hitelesites> hitelesitesek);

    /// <summary>Hitelesítések exportálása PDF formátumba.</summary>
    Task<byte[]> ExportHitelesitesekPdfAsync(List<Hitelesites> hitelesitesek);

    /// <summary>Mérések exportálása Excel formátumba.</summary>
    Task<byte[]> ExportMeresekExcelAsync(List<Meres> meresek);

    /// <summary>Mérések exportálása PDF formátumba.</summary>
    Task<byte[]> ExportMeresekPdfAsync(List<Meres> meresek);

    /// <summary>Üzemeltető adatok exportálása Excel formátumba.</summary>
    Task<byte[]> ExportUzemeltetoAdatokExcelAsync(List<UzemeltetoAdat> adatok);

    /// <summary>Üzemeltető adatok exportálása PDF formátumba.</summary>
    Task<byte[]> ExportUzemeltetoAdatokPdfAsync(List<UzemeltetoAdat> adatok);

    /// <summary>Felhasználók exportálása Excel formátumba.</summary>
    Task<byte[]> ExportFelhasznalokExcelAsync(List<Felhasznalo> felhasznalok);

    /// <summary>Felhasználók exportálása PDF formátumba.</summary>
    Task<byte[]> ExportFelhasznalokPdfAsync(List<Felhasznalo> felhasznalok);
}
