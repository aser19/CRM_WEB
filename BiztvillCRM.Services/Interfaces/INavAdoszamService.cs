using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface INavAdoszamService
{
    /// <summary>
    /// Adószám alapján lekérdezi az adóalany adatait a NAV API-ból,
    /// a céghez tartozó saját technikai felhasználóval.
    /// </summary>
    Task<NavAdoszamEredmeny> LekerdezesByAdoszamAsync(string adoszam, int cegId);
}