using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface INyilvanosLekerdezesService
{
    Task<UgyfelLekerdezesViewModel?> LekerdezesByTokenAsync(string token, bool tartalmazzaInaktivakat = false);
    Task<string> UjTokenGeneralasAsync(int ugyfelId);
    Task TokenDeaktivalasAsync(int tokenId);
    Task<List<UgyfelLekerdezesiToken>> GetTokenekByUgyfelAsync(int ugyfelId);
}
