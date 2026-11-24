using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Az önálló ÁVK (áram-védőkapcsolók működésének ellenőrzése) jegyzőkönyv PDF generálása.</summary>
public interface IAvkPdfService
{
    /// <summary>Legenerálja az ÁVK jegyzőkönyv PDF-jét a mérés adatai és a JegyzokonyvAdatok űrlap alapján.</summary>
    Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok);
}
