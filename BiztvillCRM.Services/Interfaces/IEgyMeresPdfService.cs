using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Az "Egy mérés" jegyzőkönyv PDF generálása – a meglévő Word export mellett választható alternatíva.</summary>
public interface IEgyMeresPdfService
{
    /// <summary>Legenerálja az "Egy mérés" jegyzőkönyv PDF-jét a mérés adatai és a JegyzokonyvAdatok űrlap alapján.</summary>
    Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok);
}
