using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Hibavédelmi mérési jegyzőkönyv (HVM) PDF generálása – a "Hibavédelem_csakmeres" Word sablon PDF megfelelője.</summary>
public interface IHvmPdfService
{
    /// <summary>Legenerálja a HVM PDF-et a mérés adatai és a melléklet (HvmAdatok) alapján.</summary>
    Task<byte[]> GeneralasAsync(int meresId, HvmAdatok adatok);
}
