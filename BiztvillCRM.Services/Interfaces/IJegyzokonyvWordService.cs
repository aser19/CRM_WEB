using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJegyzokonyvWordService
{
    Task<byte[]> GeneralasAsync(int meresId);
    Task<byte[]> GeneralasAsync(int meresId, string sablonId);
    Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok, string sablonId = "VBF_KIF_MINTA");
}