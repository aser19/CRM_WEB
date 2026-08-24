using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Az Időszakos VBF ("Használatbavételt megelőző" / "Kifejezetten" stb.) fő jegyzőkönyv PDF generálása.
/// A "VBF_KIF_MINTA.docx" Word sablon vizuális elrendezését követi.
/// </summary>
public interface IVbfPdfService
{
    /// <summary>Legenerálja a fő VBF jegyzőkönyv PDF-jét a mérés adatai és a JegyzokonyvAdatok űrlap alapján.</summary>
    Task<byte[]> GeneralasAsync(int meresId, JegyzokonyvAdatok adatok, string sablonId = "VBF_KIF_MINTA",
        byte[]? cegBelyegzoKep = null, Dictionary<string, byte[]>? felulvizsgaloAlairasKepek = null);
}
