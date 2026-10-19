using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Web.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(Felhasznalo felhasznalo, IList<string> szerepkorok);
}
