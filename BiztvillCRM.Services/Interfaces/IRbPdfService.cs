using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Rb (robbanásbiztos) "Egyedi felülvizsgálati lap" PDF generálása.</summary>
public interface IRbPdfService
{
    /// <summary>
    /// Legenerálja a PDF-et: elől egy előlappal, majd egy összesítő (hibalista + hiányosság lista) lappal
    /// (minden berendezés egy sorban), majd soronként (készülékenként) egy-egy részletes oldallal.
    /// </summary>
    /// <param name="sorok">A felülvizsgált Rb berendezések.</param>
    /// <param name="cegNev">A kiállító cég neve (tenant).</param>
    /// <param name="cegCim">A kiállító cég címe.</param>
    /// <param name="cegWeb">A kiállító cég weboldala/email címe.</param>
    /// <param name="jegyzokonyvSzam">A jegyzőkönyv/dokumentum száma.</param>
    /// <param name="targyLeiras">A vizsgálat tárgyának rövid leírása (pl. "B111 jelű, 50 m3-es pentán tároló tartály első RB felülvizsgálata"). Opcionális.</param>
    /// <param name="megrendeloNev">A megrendelő (vizsgált telephely tulajdonosa) neve, amely az előlapon jelenik meg. Opcionális.</param>
    /// <param name="keszultDatum">A dokumentum elkészültének dátuma. Ha nincs megadva, a mai dátum kerül felhasználásra.</param>
    /// <param name="vizsgalatTipusa">A "Vizsgálat típusa" szabad szöveg a 2. oldalon (a jegyzőkönyv készítésekor adandó meg).</param>
    /// <param name="vizsgalatHelyszine">A vizsgálat helyszíne (megrendelő telephelye/címe) a 2. oldalon.</param>
    /// <param name="vizsgalatIdopontja">A vizsgálat/felülvizsgálat időpontja (a mérés dátuma) a 2. oldalon.</param>
    /// <param name="alairoNev">A felülvizsgálatot ténylegesen aláíró személy neve (felülvizsgáló vagy ellenőr).</param>
    /// <param name="alairoBizonyitvany">Az aláíró személy bizonyítványszáma.</param>
    /// <param name="alairoBeosztas">Az aláíró személy beosztása (pl. "Robb. berendezés kezelője" vagy "Ellenőr").</param>
    /// <param name="kijeloltJogszabalyok">A jegyzőkönyv kitöltésekor kiválasztott rendeletek és szabványok (2. fejezet). Csak a Kivalasztva=true elemek kerülnek a PDF-be.</param>
    /// <param name="rbBevezetes">A "3. A felülvizsgálat leírása" fejezet "Bevezetés" (3.1) szövege. Ha nincs megadva, alapértelmezett szöveg kerül a PDF-be.</param>
    /// <param name="rbTalaltAllapotok">A "3. A felülvizsgálat leírása" fejezet "A talált állapotok leírása" (3.2) szövege. Ha nincs megadva, alapértelmezett szöveg kerül a PDF-be.</param>
    /// <param name="rbAtexTanusitvanyMegvan">3.3.1: A gyártmány rendelkezik-e ATEX jelzésű tanúsítvánnyal, vagy gyártói nyilatkozattal.</param>
    /// <param name="rbVedelmiModEgyezik">3.3.1: A gyártmány dokumentációjában feltüntetett védelmi mód egyezik-e az adattáblán szereplő védelmi móddal.</param>
    /// <param name="rbVedelmiModMegfelelTersegbesorolasnak">3.3.1: A védelmi mód megfelel-e az adatlapon megjelölt térségbesorolásnak.</param>
    /// <param name="rbAlkalmazasiCsoportHomersOsztalyMegfelelo">3.3.1: A gyártmány alkalmazási csoportja és hőmérsékleti osztálya megfelelő-e.</param>
    /// <param name="rbReszMinositesFelulbiralas">A "4 Minősítés" fejezet 8 részminősítésének kézi felülbírálása (kulcs: "1"."8", érték: true = Megfelelő).</param>
    /// <param name="rbFoMinositesFelulbiralas">A "4 Minősítés" fejezet összesített minősítésének kézi felülbírálása. Null esetén a 8 rész alapján számított érték kerül felhasználásra.</param>
    /// <param name="rbMinositesMegjegyzes">A "4 Minősítés" fejezet alatti megjegyzés szövege. Ha nincs megadva, alapértelmezett szöveg kerül a PDF-be.</param>
    /// <param name="cegBelyegzoKep">A kiállító cég bélyegzőjének képe (byte tömb), ha rögzítve van. Ha meg van adva, minden aláírás blokk mellett megjelenik.</param>
    /// <param name="alairoAlairasKep">Az aláíró (2. oldali "Auditor") személy aláírás képe (byte tömb), ha rögzítve van.</param>
    /// <param name="felulvizsgaloAlairasKepek">A soronkénti "felülvizsgálatot végezte" aláírók aláírás képei, kulcs a felülvizsgáló neve (a RbSor.VizsgalatotVegezte mezővel egyezően).</param>
    byte[] Generalas(List<RbSor> sorok, string cegNev, string cegCim, string cegWeb, string jegyzokonyvSzam, string? targyLeiras = null, string? megrendeloNev = null, DateTime? keszultDatum = null,
        string? vizsgalatTipusa = null, string? vizsgalatHelyszine = null, DateTime? vizsgalatIdopontja = null,
        string? alairoNev = null, string? alairoBizonyitvany = null, string? alairoBeosztas = null,
        List<KijeloltJogszabaly>? kijeloltJogszabalyok = null, string? rbBevezetes = null, string? rbTalaltAllapotok = null,
        bool rbAtexTanusitvanyMegvan = true, bool rbVedelmiModEgyezik = true,
        bool rbVedelmiModMegfelelTersegbesorolasnak = true, bool rbAlkalmazasiCsoportHomersOsztalyMegfelelo = true,
        Dictionary<string, bool>? rbReszMinositesFelulbiralas = null, bool? rbFoMinositesFelulbiralas = null, string? rbMinositesMegjegyzes = null,
        byte[]? cegBelyegzoKep = null, byte[]? alairoAlairasKep = null, Dictionary<string, byte[]>? felulvizsgaloAlairasKepek = null);
}
