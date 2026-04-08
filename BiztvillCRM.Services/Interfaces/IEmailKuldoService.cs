using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Email küldés szolgáltatás.</summary>
public interface IEmailKuldoService
{
    /// <summary>Egyedi email küldése.</summary>
    Task<bool> KuldAsync(string cimzett, string targy, string szoveg, int? cegId = null);
    
    /// <summary>Sablon alapú email küldése placeholder-ek helyettesítésével.</summary>
    Task<bool> KuldSablonbolAsync(
        EmailErtesitesTipus tipus,
        string cimzett,
        Dictionary<string, string> placeholderek,
        int? cegId = null,
        int? hitelesitesId = null,
        int? meresId = null);
}