using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface INavAdoszamService
{
    /// <summary>
    /// Adószám alapján lekérdezi az adóalany adatait a NAV Online Számla API-ból,
    /// a bejelentkezett felhasználó cégéhez tartozó technikai felhasználóval.
    /// </summary>
    Task<NavAdoszamEredmeny> LekerdezesByAdoszamAsync(string adoszam, int cegId);
}