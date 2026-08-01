using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Rb (robbanásbiztos) "Egyedi felülvizsgálati lap" PDF generálása.</summary>
public interface IRbPdfService
{
    /// <summary>
    /// Legenerálja a PDF-et, soronként (készülékenként) egy oldallal.
    /// </summary>
    /// <param name="sorok">A felülvizsgált Rb berendezések.</param>
    /// <param name="cegNev">A kiállító cég neve (tenant).</param>
    /// <param name="cegCim">A kiállító cég címe.</param>
    /// <param name="cegWeb">A kiállító cég weboldala/email címe.</param>
    /// <param name="jegyzokonyvSzam">A jegyzőkönyv/dokumentum száma.</param>
    byte[] Generalas(List<RbSor> sorok, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam);
}
