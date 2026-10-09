using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// A "Nem Norma szerinti Villámvédelmi Minősítő Irat" PDF generálása (címlap, főadatok, tartalomjegyzék, ...).
/// </summary>
public interface IVillamNemNormaPdfService
{
    /// <summary>Legenerálja a Nem Norma szerinti Villámvédelmi Minősítő Irat PDF-jét.</summary>
    /// <param name="adatok">A jegyzőkönyv adatai.</param>
    /// <param name="cegBelyegzoKep">A kiállító cég bélyegzőképe.</param>
    /// <param name="alairoAlairasKep">Az aláíró felülvizsgáló aláírásképe.</param>
    /// <param name="fenykepek">Az 1. mellékletbe kerülő fényképek/skiccek bájtjai (max. 5 db).</param>
    /// <param name="kepBizonyitvanyok">A 2. mellékletbe kerülő, kép formátumú bizonyítványok (címke, bájtok) párosai.</param>
    /// <param name="pdfBizonyitvanyok">A dokumentum végére fűzendő, PDF formátumú bizonyítványok bájtjai.</param>
    byte[] Generalas(
        VillamNemNormaAdatok adatok,
        byte[]? cegBelyegzoKep = null,
        byte[]? alairoAlairasKep = null,
        List<byte[]>? fenykepek = null,
        List<(string Cimke, byte[] Kep)>? kepBizonyitvanyok = null,
        List<byte[]>? pdfBizonyitvanyok = null);
}
