using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// A "Nem Norma szerinti Villámvédelmi Minősítő Irat" PDF generálása (címlap, főadatok, tartalomjegyzék, ...).
/// </summary>
public interface IVillamNemNormaPdfService
{
    /// <summary>Legenerálja a Nem Norma szerinti Villámvédelmi Minősítő Irat PDF-jét.</summary>
    byte[] Generalas(VillamNemNormaAdatok adatok, byte[]? cegBelyegzoKep = null, byte[]? alairoAlairasKep = null);
}
