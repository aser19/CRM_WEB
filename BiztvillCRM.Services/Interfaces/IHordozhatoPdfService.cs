using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IHordozhatoPdfService
{
    Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok);
}
