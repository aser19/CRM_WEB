using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Email küldés szolgáltatás.</summary>
public interface IEmailKuldoService
{
    /// <summary>Egyedi email küldése.</summary>
    Task<bool> KuldAsync(string cimzett, string targy, string szoveg, int? cegId = null);

    /// <summary>Email küldése csatolmánnyal és opcionális másolattal (CC).</summary>
    Task<bool> KuldCsatolmannyalAsync(
        string cimzett,
        string targy,
        string szoveg,
        IEnumerable<(string FileName, byte[] Content)> csatolmanyok,
        string? cc = null,
        int? cegId = null,
        int? meresId = null);

    /// <summary>Sablon alapú email küldése placeholder-ek helyettesítésével.</summary>
    Task<bool> KuldSablonbolAsync(
        EmailErtesitesTipus tipus,
        string cimzett,
        Dictionary<string, string> placeholderek,
        int? cegId = null,
        int? hitelesitesId = null,
        int? meresId = null,
        int? karbantartasId = null);
}