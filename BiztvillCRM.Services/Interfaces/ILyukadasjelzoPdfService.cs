using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// A "Túlnyomással működtetett lyukadásjelző időszakos vizsgálatáról" szóló
/// vizsgálati jegyzőkönyv PDF generálása.
/// </summary>
public interface ILyukadasjelzoPdfService
{
    /// <summary>Legenerálja a jegyzőkönyv PDF-jét a megadott adatok, illetve az aktuális cég bélyegzője alapján.</summary>
    Task<byte[]> TulnyomasosGeneralasAsync(TulnyomasosLyukadasjelzoAdatok adatok);
}
