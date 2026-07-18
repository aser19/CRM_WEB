using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Services;

public class SugoService : ISugoService
{
    private readonly IDbContextFactory<CrmDbContext> _contextFactory;

    public SugoService(IDbContextFactory<CrmDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<SugoKategoria>> GetAllWithTemakAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SugoKategoriak
            .Include(k => k.Temak.OrderBy(t => t.Sorrend))
            .OrderBy(k => k.Sorrend)
            .ToListAsync();
    }

    public async Task<SugoKategoria?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SugoKategoriak
            .Include(k => k.Temak.OrderBy(t => t.Sorrend))
            .FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<int> CreateKategoriaAsync(SugoKategoria kategoria)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        kategoria.Letrehozva = DateTime.UtcNow;
        context.SugoKategoriak.Add(kategoria);
        await context.SaveChangesAsync();

        return kategoria.Id;
    }

    public async Task UpdateKategoriaAsync(SugoKategoria kategoria)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.SugoKategoriak.FindAsync(kategoria.Id)
            ?? throw new InvalidOperationException("Súgó kategória nem található");

        existing.Nev = kategoria.Nev;
        existing.Icon = kategoria.Icon;
        existing.Sorrend = kategoria.Sorrend;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteKategoriaAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var kategoria = await context.SugoKategoriak.FindAsync(id);
        if (kategoria != null)
        {
            context.SugoKategoriak.Remove(kategoria);
            await context.SaveChangesAsync();
        }
    }

    public async Task<SugoTema?> GetTemaByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SugoTemak.FindAsync(id);
    }

    public async Task<int> CreateTemaAsync(SugoTema tema)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        tema.Letrehozva = DateTime.UtcNow;
        context.SugoTemak.Add(tema);
        await context.SaveChangesAsync();

        return tema.Id;
    }

    public async Task UpdateTemaAsync(SugoTema tema)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.SugoTemak.FindAsync(tema.Id)
            ?? throw new InvalidOperationException("Súgó téma nem található");

        existing.SugoKategoriaId = tema.SugoKategoriaId;
        existing.Cim = tema.Cim;
        existing.Leiras = tema.Leiras;
        existing.VideoUrl = tema.VideoUrl;
        existing.Sorrend = tema.Sorrend;
        existing.Modositva = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteTemaAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var tema = await context.SugoTemak.FindAsync(id);
        if (tema != null)
        {
            context.SugoTemak.Remove(tema);
            await context.SaveChangesAsync();
        }
    }

    public async Task SeedIfEmptyAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (await context.SugoKategoriak.AnyAsync())
        {
            return;
        }

        var most = DateTime.UtcNow;
        var kategoriak = new List<SugoKategoria>
        {
            new()
            {
                Nev = "Dashboard", Icon = "Dashboard", Sorrend = 0, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Kezdőoldal áttekintés",
                        Leiras = "Mihez: a rendszerbe belépés után első lépésként ide érkezel, itt látod a legfontosabb, azonnali figyelmet igénylő adatokat.\n" +
                                 "Mit: lejáró/lejárt hitelesítéseket, esedékes karbantartásokat, elvégzendő méréseket és egyéb határidős feladatokat.\n" +
                                 "Hogyan: a bal oldali főmenüből bármikor visszatérhetsz ide a \"Dashboard\" gombra kattintva. A kártyákon/listákon megjelenő tételekre kattintva közvetlenül az adott eszköz vagy modul részletes oldalára jutsz.\n" +
                                 "Kötelező: nincs kitöltendő mező ezen az oldalon, csak megjelenítő felület.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Törzsadatok", Icon = "Business", Sorrend = 1, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Ügyfelek kezelése",
                        Leiras = "Mihez: minden olyan partnered nyilvántartásához, akinek eszközeit, méréseit vagy hitelesítéseit kezeled.\n" +
                                 "Mit: ügyfél neve, székhelye, adószáma, kapcsolattartó neve, telefonszáma, email címe.\n" +
                                 "Hogyan: Törzsadatok → Ügyfelek menüpontban az \"Új ügyfél\" gombbal hozol létre új rekordot, majd a megjelenő űrlapon töltöd ki az adatokat és a \"Mentés\" gombbal rögzíted. Meglévő ügyfelet a listában rákattintva szerkeszthetsz.\n" +
                                 "Kötelező mezők: Ügyfél neve. A többi adat (cím, adószám, kapcsolattartó) ajánlott, de a rendszer nélkülük is engedi a mentést, ha az adott telepítés nem tette kötelezővé.\n" +
                                 "Fontos: egy ügyfélhez több telephely és több eszköz is rendelhető, ezért érdemes az ügyfelet mindig előbb felvinni, mielőtt telephelyet vagy eszközt rögzítenél hozzá.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Telephelyek",
                        Leiras = "Mihez: az ügyfélhez tartozó fizikai helyszínek (telephelyek, üzemek, épületek) nyilvántartásához, ahol az eszközök ténylegesen találhatók.\n" +
                                 "Mit: telephely neve, pontos címe (irányítószám, város, utca, házszám), és az ügyfél, akihez tartozik.\n" +
                                 "Hogyan: Törzsadatok → Telephelyek menüpontban \"Új telephely\" gombbal indítod a felvitelt. Első lépésként ki kell választanod, melyik ügyfélhez tartozik a telephely, majd megadod a címadatokat, végül \"Mentés\".\n" +
                                 "Kötelező mezők: Ügyfél kiválasztása és a telephely neve/címe. Telephely nélkül eszközt nem lehet a helyes helyszínhez rendelni, ezért ezt az ügyfél felvitele után célszerű elvégezni.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Gyártók",
                        Leiras = "Mihez: az eszközök gyártóinak egységes, listából választható nyilvántartásához, hogy ne kelljen minden eszköznél kézzel begépelni a gyártó nevét.\n" +
                                 "Mit: gyártó neve (és opcionálisan egyéb azonosító adatai).\n" +
                                 "Hogyan: Törzsadatok → Gyártók menüpontban \"Új gyártó\" gombbal viheted fel a nevet, majd \"Mentés\". Ezután az eszköz felvitelénél ez a gyártó megjelenik a legördülő listában.\n" +
                                 "Kötelező mező: Gyártó neve. Javasolt az eszközök felvitele előtt előkészíteni a gyakran használt gyártók listáját.",
                        Sorrend = 2, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Eszközök",
                        Leiras = "Mihez: minden nyilvántartott berendezés, gép, mérőműszer rögzítéséhez, amelyeken méréseket, hitelesítéseket vagy karbantartásokat végeznek.\n" +
                                 "Mit: eszköz megnevezése, típusa, gyártója, gyártási száma, telephelye, és az eszköztípushoz tartozó egyéb műszaki paraméterek.\n" +
                                 "Hogyan: Törzsadatok → Eszközök menüpontban \"Új eszköz\" gombbal indítod a felvitelt. Válaszd ki az ügyfelet és a telephelyet, add meg az eszköz típusát (legördülő lista), a gyártóját, azonosítóját, majd \"Mentés\". Az elmentett eszköz adatlapjáról közvetlenül indíthatók a mérések és a hitelesítések rögzítése.\n" +
                                 "Kötelező mezők: Ügyfél, Telephely, Eszköztípus és az eszköz megnevezése. Ezek hiányában az eszköz nem menthető el, mert ezek nélkül nem lehetne beazonosítani, hova és milyen vizsgálat tartozik hozzá.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 3, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Felülvizsgálók",
                        Leiras = "Mihez: azon szakemberek nyilvántartásához, akik a méréseket, hitelesítéseket és felülvizsgálatokat elvégzik, és akiket a jegyzőkönyveken/riportokon fel kell tüntetni.\n" +
                                 "Mit: felülvizsgáló neve, elérhetőségei, esetleges engedélyszáma/jogosultsága.\n" +
                                 "Hogyan: Törzsadatok → Felülvizsgálók menüpontban \"Új felülvizsgáló\" gombbal viszed fel az adatokat, majd \"Mentés\". A mérés/hitelesítés rögzítésekor ebből a listából választható ki, ki végezte az adott vizsgálatot.\n" +
                                 "Kötelező mező: Felülvizsgáló neve, a többi adat kiegészítő jellegű, de ajánlott a pontos dokumentáció miatt kitölteni.",
                        Sorrend = 4, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Eszköztípusok (admin)",
                        Leiras = "Mihez: annak rendszerszintű meghatározásához, milyen kategóriájú eszközök léteznek, és milyen vizsgálatok (mérés, hitelesítés, karbantartás) végezhetők el rajtuk.\n" +
                                 "Mit: eszköztípus neve, és a hozzá kapcsolódó vizsgálati/hitelesítési szabályok.\n" +
                                 "Hogyan: Admin → (Törzsadat karbantartás) menüpontban éred el, csak Admin jogosultsággal. Új eszköztípus felvitele az \"Új\" gombbal, majd a megnevezés és a kapcsolódó beállítások megadása után \"Mentés\".\n" +
                                 "Kötelező mező: Eszköztípus neve. Ezt a listát célszerű a rendszer bevezetésekor egyszer, előre kialakítani, mert minden eszköz felvitelekor innen választanak típust.",
                        Sorrend = 5, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Mérések", Icon = "Speed", Sorrend = 2, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Mérések rögzítése",
                        Leiras = "Mihez: az eszközökön elvégzett méréseknek (pl. érintésvédelmi, szigetelési, egyéb szabványos mérések) a rögzítéséhez és a következő mérési esedékesség automatikus nyomon követéséhez.\n" +
                                 "Mit: mérés dátuma, típusa, eredménye (megfelelt/nem megfelelt, mért érték), az elvégző felülvizsgáló és az érintett eszköz.\n" +
                                 "Hogyan: Mérések → Mérések menüpontban válaszd ki az eszközt, add meg a mérés típusát, dátumát és eredményét, majd \"Mentés\". A rendszer a mérés típusához tartozó szabály alapján automatikusan kiszámolja a következő esedékesség dátumát.\n" +
                                 "Kötelező mezők: Eszköz, Mérés típusa, Mérés dátuma. Eredmény nélkül a mérés rögzíthető, de nem tekinthető lezártnak, ezért javasolt mindig kitölteni.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Kalibrációk",
                        Leiras = "Mihez: a mérőeszközök (nem a vizsgált berendezések, hanem a mérésre használt műszerek) kalibrálási állapotának nyomon követéséhez.\n" +
                                 "Mit: mérőeszköz azonosítója, kalibrálás dátuma, érvényesség lejárata, kalibráló szervezet neve.\n" +
                                 "Hogyan: Mérések → Kalibrációk menüpontban \"Új kalibráció\" gombbal rögzíted az adatokat, majd \"Mentés\". A lejárati dátum alapján a rendszer jelzi, ha közeleg vagy lejárt a kalibráció érvényessége.\n" +
                                 "Kötelező mezők: Mérőeszköz azonosítója és a kalibrálás dátuma. Az érvényesség lejárata nélkül a rendszer nem tud figyelmeztetni a következő kalibráció esedékességére, ezért ezt is meg kell adni.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Jegyzőkönyv készítés",
                        Leiras = "Mihez: a rögzített mérési eredmények alapján hivatalos, kinyomtatható/exportálható jegyzőkönyv elkészítéséhez.\n" +
                                 "Mit: az érintett eszköz(ök), a mérési eredmények, a felülvizsgáló adatai és a választott jegyzőkönyv sablon.\n" +
                                 "Hogyan: Mérések → Jegyzőkönyv készítés menüpontban válaszd ki az eszközt/eszközöket és a kívánt jegyzőkönyv sablont, ellenőrizd az automatikusan behúzott mérési adatokat, majd a \"Jegyzőkönyv generálása\" gombbal állítod elő a végleges dokumentumot.\n" +
                                 "Kötelező: legalább egy lezárt mérési eredmény és egy kiválasztott jegyzőkönyv sablon szükséges a generáláshoz, ezek hiányában a jegyzőkönyv nem készül el.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 2, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Alkatrész sablonok",
                        Leiras = "Mihez: gyakran használt alkatrész-listák előre definiálásához, hogy a jegyzőkönyvek elkészítésekor ne kelljen mindig egyesével felsorolni az alkatrészeket.\n" +
                                 "Mit: sablon neve és a hozzá tartozó alkatrészek listája (megnevezés, mennyiség, egyéb jellemzők).\n" +
                                 "Hogyan: Mérések → Alkatrész sablonok menüpontban \"Új sablon\" gombbal hozod létre a sablont, hozzáadod az alkatrészeket, majd \"Mentés\". A jegyzőkönyv készítésénél ez a sablon egy kattintással behúzható.\n" +
                                 "Kötelező mező: Sablon neve, legalább egy alkatrész tétel megadása ajánlott, különben a sablon üresen nem sok hasznot hoz.",
                        Sorrend = 3, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Hitelesítések", Icon = "VerifiedUser", Sorrend = 3, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Hitelesítések nyilvántartása",
                        Leiras = "Mihez: az eszközök hatósági hitelesítési állapotának és lejárati határidejének nyomon követéséhez.\n" +
                                 "Mit: az érintett eszköz, a hitelesítés dátuma, a hitelesítő hatóság és az érvényesség lejárati dátuma.\n" +
                                 "Hogyan: Hitelesítések → Hitelesítések menüpontban \"Új hitelesítés\" gombbal válaszd ki az eszközt, add meg a hitelesítés és lejárat dátumát, illetve a hatóságot, majd \"Mentés\".\n" +
                                 "Kötelező mezők: Eszköz, Hitelesítés dátuma, Lejárat dátuma. A lejárat dátuma nélkül a rendszer nem tudja jelezni a közelgő vagy lejárt hitelesítést.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Kötelező hitelesítések",
                        Leiras = "Mihez: azon eszközök áttekintéséhez, amelyeknél jogszabály írja elő a rendszeres hatósági hitelesítést.\n" +
                                 "Mit: a listában megjelenik az eszköz, az utolsó hitelesítés dátuma és a következő esedékesség.\n" +
                                 "Hogyan: Hitelesítések → Kötelező hitelesítések menüpontban listaszerűen tekintheted át az érintett eszközöket, és innen közvetlenül átugorhatsz az eszköz adatlapjára új hitelesítés rögzítéséhez.\n" +
                                 "Kötelező: ez egy megjelenítő/riport nézet, közvetlen adatbevitel itt nincs, a szűrők (pl. lejárat szerint) használata opcionális.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Hatóságok",
                        Leiras = "Mihez: a hitelesítést végző hatóságok elérhetőségeinek és adatainak egységes nyilvántartásához.\n" +
                                 "Mit: hatóság neve, elérhetőségei (cím, telefon, email).\n" +
                                 "Hogyan: Hitelesítések → Hatóságok menüpontban \"Új hatóság\" gombbal viszed fel az adatokat, majd \"Mentés\". A hitelesítés rögzítésénél ebből a listából választható ki a hatóság.\n" +
                                 "Kötelező mező: Hatóság neve, a többi elérhetőségi adat kiegészítő.",
                        Sorrend = 2, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Munkavédelem", Icon = "HealthAndSafety", Sorrend = 4, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Munkavédelmi oktatások",
                        Leiras = "Mihez: a dolgozók kötelező munkavédelmi oktatásainak nyilvántartásához és a következő oktatás esedékességének figyeléséhez.\n" +
                                 "Mit: dolgozó neve, oktatás típusa, dátuma és érvényessége.\n" +
                                 "Hogyan: Munkavédelem → Munkavédelmi oktatások menüpontban \"Új oktatás\" gombbal rögzíted a dolgozót, az oktatás típusát és dátumát, majd \"Mentés\".\n" +
                                 "Kötelező mezők: Dolgozó neve, Oktatás típusa, Oktatás dátuma. Ezek hiányában a rendszer nem tudja kiszámolni a következő oktatás esedékességét.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Kockázatértékelések",
                        Leiras = "Mihez: a munkahelyi kockázatok felmérésének és dokumentálásának elvégzéséhez, telephelyenként vagy munkafolyamatonként.\n" +
                                 "Mit: az értékelt terület/folyamat, azonosított kockázatok, javasolt intézkedések és az értékelés dátuma.\n" +
                                 "Hogyan: Munkavédelem → Kockázatértékelések menüpontban \"Új kockázatértékelés\" gombbal hozod létre az értékelést, kitöltöd a felmért kockázatokat és intézkedéseket, majd \"Mentés\".\n" +
                                 "Kötelező mezők: Telephely/terület kiválasztása és az értékelés dátuma. A részletes kockázatlista tartalma a valós helyzettől függ, de legalább egy tétel megadása szükséges az értékelés lezárásához.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Zónatérképek",
                        Leiras = "Mihez: robbanásveszélyes vagy egyéb speciális besorolású zónák térképi megjelenítéséhez és dokumentálásához.\n" +
                                 "Mit: a zóna térkép (feltöltött kép/rajz), a zóna típusa és leírása.\n" +
                                 "Hogyan: Munkavédelem → Zónatérképek menüpontban \"Új zónatérkép\" gombbal töltöd fel a térképet és adod meg a hozzá tartozó adatokat, majd \"Mentés\".\n" +
                                 "Kötelező mezők: Telephely kiválasztása és a feltöltött térkép/kép, enélkül a zóna nem jeleníthető meg vizuálisan.",
                        Sorrend = 2, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Karbantartás", Icon = "Engineering", Sorrend = 5, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Karbantartások kezelése",
                        Leiras = "Mihez: az eszközök rendszeres (ütemezett) és eseti (soron kívüli) karbantartásainak megtervezéséhez, rögzítéséhez és állapotuk nyomon követéséhez.\n" +
                                 "Mit: az érintett eszköz, karbantartás típusa (rendszeres/eseti), dátuma, elvégzett munkák leírása, státusza.\n" +
                                 "Hogyan: Karbantartás → Karbantartások menüpontban \"Új karbantartás\" gombbal válaszd ki az eszközt, add meg a típust és a dátumot, írd le az elvégzett munkát, majd \"Mentés\". A státusz mezővel (pl. tervezett, folyamatban, kész) tudod nyomon követni az előrehaladást.\n" +
                                 "Kötelező mezők: Eszköz, Karbantartás típusa, Dátum. Ezek nélkül a karbantartás nem társítható a megfelelő eszközhöz és időponthoz.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Jogszabályok", Icon = "Gavel", Sorrend = 6, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Jogszabálytár",
                        Leiras = "Mihez: a tevékenységhez kapcsolódó hatályos jogszabályok, rendeletek és szabványok gyors elérhetőségéhez, hivatkozáshoz.\n" +
                                 "Mit: a jogszabály/szabvány megnevezése, száma, rövid leírása, esetleg csatolt dokumentum vagy link.\n" +
                                 "Hogyan: Jogszabályok menüpontban a listát a fejlécben található kereső mezővel tudod szűrni cím vagy szám alapján. Egy tételre kattintva megnyílik a részletes tartalom vagy a csatolt dokumentum.\n" +
                                 "Kötelező: ez elsősorban egy megtekintésre szolgáló nyilvántartás, adatbevitel csak Admin jogosultsággal lehetséges, ha új jogszabályt kell felvinni.",
                        Sorrend = 0, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Munkaszám", Icon = "Assignment", Sorrend = 7, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Munkaszámok kezelése",
                        Leiras = "Mihez: a projektekhez, megrendelésekhez tartozó munkaszámok létrehozásához, amelyekhez méréseket, hitelesítéseket és egyéb tevékenységeket lehet rendelni a könnyebb elszámolhatóság érdekében.\n" +
                                 "Mit: munkaszám azonosítója/neve, kapcsolódó ügyfél, leírás, státusz (nyitott/lezárt).\n" +
                                 "Hogyan: Munkaszám menüpontban \"Új munkaszám\" gombbal hozod létre, kiválasztod az ügyfelet és megadod a leírást, majd \"Mentés\". A mérések és egyéb tevékenységek rögzítésekor a munkaszám kiválasztható, hogy hozzá kapcsolódjanak.\n" +
                                 "Kötelező mezők: Munkaszám azonosítója és az ügyfél kiválasztása. Ezek nélkül a munkaszám nem különböztethető meg egyértelműen a rendszerben.",
                        Sorrend = 0, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Riportok", Icon = "Analytics", Sorrend = 8, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Összefoglaló riport",
                        Leiras = "Mihez: a rendszer teljes állapotának áttekintéséhez egy helyen (eszközszámok, lejáratok, elvégzett tevékenységek).\n" +
                                 "Mit: összesített statisztikák, grafikonok és táblázatok, amelyek automatikusan a rögzített adatokból generálódnak.\n" +
                                 "Hogyan: Riportok → Összefoglaló menüpontban a felső szűrőkkel (pl. időszak, cég) tudod szűkíteni a megjelenített adatokat. Adatbevitel itt nincs, csak megjelenítés.\n" +
                                 "Kötelező: nincs kitöltendő mező, csak admin felhasználók érik el.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Lejáratok riport",
                        Leiras = "Mihez: az összes közelgő vagy már lejárt hitelesítés, mérés és karbantartás egy listában történő áttekintéséhez, a proaktív ütemezés támogatására.\n" +
                                 "Mit: eszköz, tevékenység típusa, lejárat dátuma, hátralévő/túllépett napok száma.\n" +
                                 "Hogyan: Riportok → Lejáratok menüpontban a szűrőkkel (pl. dátumtartomány, tevékenység típusa) szűkítheted a listát, majd az egyes tételekre kattintva közvetlenül az eszköz adatlapjára juthatsz új tevékenység rögzítéséhez.\n" +
                                 "Kötelező: nincs adatbeviteli mező, csak megjelenítés és szűrés.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Cégek statisztikája",
                        Leiras = "Mihez: cégenkénti (ügyfelenkénti) bontásban az eszközök, mérések és egyéb tevékenységek áttekintéséhez.\n" +
                                 "Mit: cégenkénti eszközszám, elvégzett mérések/hitelesítések száma, egyéb aggregált mutatók.\n" +
                                 "Hogyan: Riportok → Cégek statisztikája menüpontban válaszd ki a kívánt céget/cégeket a szűrőben, a táblázat automatikusan frissül.\n" +
                                 "Kötelező: nincs kitöltendő mező, csak megjelenítés.",
                        Sorrend = 2, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Email statisztika",
                        Leiras = "Mihez: a rendszerből kiküldött értesítő emailek (pl. lejárati figyelmeztetések) állapotának nyomon követéséhez.\n" +
                                 "Mit: elküldött email címzettje, tárgya, küldés dátuma, státusza (elküldve/hiba).\n" +
                                 "Hogyan: Riportok → Email statisztika menüpontban a listát dátum vagy státusz szerint szűrheted.\n" +
                                 "Kötelező: nincs kitöltendő mező, csak megjelenítés.",
                        Sorrend = 3, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Aktivitás napló",
                        Leiras = "Mihez: a felhasználók rendszerben végzett tevékenységeinek auditálási célú rögzítéséhez (ki, mikor, mit módosított).\n" +
                                 "Mit: felhasználó neve, tevékenység típusa, érintett rekord, időpont.\n" +
                                 "Hogyan: Riportok → Aktivitás napló menüpontban a listát felhasználó, dátum vagy tevékenység típusa szerint szűrheted.\n" +
                                 "Kötelező: nincs kitöltendő mező, csak megjelenítés, kizárólag Admin jogosultsággal érhető el.",
                        Sorrend = 4, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Beállítások", Icon = "Settings", Sorrend = 9, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Cégbeállítások",
                        Leiras = "Mihez: a saját céged alapadatainak és rendszerszintű beállításainak módosításához.\n" +
                                 "Mit: cégnév, logó, egyéb megjelenítési és rendszerbeállítások.\n" +
                                 "Hogyan: Beállítások → Cégbeállítások menüpontban módosítod a mezőket, majd \"Mentés\" gombbal rögzíted a változásokat.\n" +
                                 "Kötelező mező: Cégnév. Az oldal csak Admin jogosultsággal érhető el.",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Licensz kezelés",
                        Leiras = "Mihez: a cég előfizetésével, licenszelésével kapcsolatos adatok megtekintéséhez és kezeléséhez.\n" +
                                 "Mit: licensz típusa, érvényesség, felhasználószám-korlát.\n" +
                                 "Hogyan: Beállítások → Licensz kezelés menüpontban tekintheted meg az aktuális állapotot, illetve itt tudod frissíteni/megújítani a licenszet, ha a rendszer ezt lehetővé teszi.\n" +
                                 "Kötelező: az oldal csak Admin jogosultsággal érhető el, adatbevitel a licensz típusától függ.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Email beállítások",
                        Leiras = "Mihez: a kimenő email sablonok és az automatikus értesítési szabályok konfigurálásához.\n" +
                                 "Mit: email sablon szövege, tárgya, az értesítés kiváltó feltétele (pl. hány nappal lejárat előtt).\n" +
                                 "Hogyan: Beállítások → Email beállítások menüpontban válaszd ki a szerkesztendő sablont, módosítsd a szöveget/feltételeket, majd \"Mentés\".\n" +
                                 "Kötelező mezők: Sablon tárgya és szövege. Ezek nélkül az adott értesítés nem lesz kiküldve.",
                        Sorrend = 2, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Jegyzőkönyv sablonok (cégadmin)",
                        Leiras = "Mihez: saját, cégre szabott jegyzőkönyv sablonok létrehozásához és szerkesztéséhez, amelyeket a mérési jegyzőkönyvek generálásakor használ a rendszer.\n" +
                                 "Mit: sablon neve, felépítése (fejléc, mezők, alkatrészlisták), formázása.\n" +
                                 "Hogyan: Beállítások → Jegyzőkönyv sablonok menüpontban \"Új sablon\" gombbal hozod létre, szerkesztő felületen állítod össze a tartalmát, majd \"Mentés\".\n" +
                                 "Kötelező mező: Sablon neve. Az oldal CégAdmin vagy Admin jogosultsággal érhető el.",
                        Sorrend = 3, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Admin", Icon = "AdminPanelSettings", Sorrend = 10, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Cégek és felhasználók kezelése",
                        Leiras = "Mihez: rendszergazdaként az összes cég és felhasználó, valamint azok szerepköreinek és jogosultságainak kezeléséhez.\n" +
                                 "Mit: cég neve, felhasználó neve, email címe, szerepköre (pl. Admin, CégAdmin, Üzemeltető).\n" +
                                 "Hogyan: Admin → Cégek, illetve Admin → Felhasználók menüpontban \"Új\" gombbal viszed fel az új rekordot, kiválasztod a szerepkört, majd \"Mentés\". Meglévő rekordot a listában rákattintva szerkeszthetsz vagy inaktiválhatsz.\n" +
                                 "Kötelező mezők: Cégnév, illetve felhasználónál a Név, Email és Szerepkör. Ez a menü kizárólag Admin jogosultsággal érhető el.",
                        VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        Sorrend = 0, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Törzsadat karbantartás (típusok, csoportok)",
                        Leiras = "Mihez: a rendszer szintű, minden cégre érvényes törzsadatok (mérés típusok, képzés típusok, védelmi mód/osztály besorolások, vizsgálatcsoportok, hitelesítési csoportok) karbantartásához.\n" +
                                 "Mit: az adott típus/csoport neve és a hozzá tartozó szabályok (pl. esedékesség számítási logika).\n" +
                                 "Hogyan: Admin menü megfelelő alpontján (pl. Mérés típusok, Képzés típusok) \"Új\" gombbal viszed fel a tételt, majd \"Mentés\".\n" +
                                 "Kötelező mező: a tétel neve. Ezeket a törzsadatokat javasolt a rendszer bevezetésekor, a napi használat megkezdése előtt kialakítani, mert számos más modul ezekre a listákra épül.",
                        Sorrend = 1, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Diagnosztika",
                        Leiras = "Mihez: technikai hibaelhárításhoz szükséges rendszerinformációk megtekintéséhez.\n" +
                                 "Mit: bejelentkezett felhasználó adatai, rendszerkörnyezet, konfigurációs információk.\n" +
                                 "Hogyan: Admin → Diagnosztika menüpontban az oldal automatikusan megjeleníti az adatokat, adatbevitel nincs.\n" +
                                 "Kötelező: nincs, kizárólag Admin jogosultsággal érhető el.",
                        Sorrend = 2, Letrehozva = most
                    },
                    new SugoTema
                    {
                        Cim = "Súgó tartalom szerkesztése",
                        Leiras = "Mihez: a súgó oldal (Súgó menüpont) kategóriáinak és témáinak közvetlen, kód módosítása nélküli szerkesztéséhez.\n" +
                                 "Mit: kategória neve, ikonja és sorrendje, valamint az egyes témák címe, szöveges leírása és opcionális videó linkje.\n" +
                                 "Hogyan: Admin → Súgó tartalom menüpontban a kategóriák listájából kiválasztott kategóriához \"Új téma\" gombbal adhatsz hozzá tartalmat, vagy meglévő témát szerkeszthetsz/törölhetsz.\n" +
                                 "Kötelező mezők: Kategória neve, Téma címe és leírása. A videó link opcionális.",
                        Sorrend = 3, Letrehozva = most
                    }
                }
            },
            new()
            {
                Nev = "Üzemeltető", Icon = "Build", Sorrend = 11, Letrehozva = most,
                Temak = new()
                {
                    new SugoTema
                    {
                        Cim = "Üzemeltetői adatok",
                        Leiras = "Mihez: Üzemeltető szerepkörű felhasználók számára a hozzájuk rendelt eszközök és feladatok korlátozott, célzott áttekintéséhez.\n" +
                                 "Mit: a felhasználóhoz rendelt eszközök listája, azok aktuális állapota, elvégzendő feladatok.\n" +
                                 "Hogyan: Üzemeltető adatok menüpontban a lista automatikusan a bejelentkezett felhasználóhoz rendelt tételeket mutatja, ezekre kattintva érhetők el a részletek.\n" +
                                 "Kötelező: nincs kitöltendő mező, a hozzáférés a rendszergazda által előzetesen beállított jogosultságtól függ.",
                        Sorrend = 0, Letrehozva = most
                    }
                }
            }
        };

        context.SugoKategoriak.AddRange(kategoriak);
        await context.SaveChangesAsync();
    }
}
