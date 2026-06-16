using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface ILejaroOsszefoglaloService
{
    /// <summary>Összegyűjti az ügyfél következő N napban lejáró tételeit.</summary>
    Task<LejaroOsszefoglalo> GetOsszefoglaloAsync(int ugyfelId, int napokSzama = 30);

    /// <summary>Összefoglaló email küldése az összes lejáró tételről.</summary>
    Task<bool> KuldOsszefoglaloEmailtAsync(int ugyfelId, string cimzett, int napokSzama = 30, int? cegId = null);
}