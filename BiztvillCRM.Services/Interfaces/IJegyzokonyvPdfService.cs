using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

public interface IJegyzokonyvPdfService
{
    byte[] Generalas(MeresJegyzokonyvAdatok adatok);
    Task<byte[]> GeneralasAsync(int meresId);
}