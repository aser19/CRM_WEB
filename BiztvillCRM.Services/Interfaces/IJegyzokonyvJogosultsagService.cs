using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJegyzokonyvJogosultsagService
{
    Task<List<Felulvizsgalo>> GetJogosultFelulvizsgalokAsync(int meresTipusId);
    Task<string> GeneralBizonyitvanySzovegetAsync(int meresTipusId, int felulvizsgaloId);
    Task<string> GeneralTovabbkepzesSzovegetAsync(int meresTipusId, int felulvizsgaloId);
    
    /// <summary>Ellenőrzi, hogy van-e jogosult felülvizsgáló és visszaadja a hiányzó képzéseket</summary>
    Task<(bool VanJogosult, List<string> HianyzoKepzesek)> EllenorizJogosultsagotAsync(int meresTipusId);
}