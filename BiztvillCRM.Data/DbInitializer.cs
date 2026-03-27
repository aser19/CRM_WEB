using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;

namespace BiztvillCRM.Data;

/// <summary>
/// Adatbázis inicializáló: létrehozza a táblákat és feltölti teszt adatokkal,
/// ha az adatbázis még üres. Idempotens – ha már vannak adatok, nem szúr be újra.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(CrmDbContext db)
    {
        // Táblák létrehozása (ha még nem léteznek)
        db.Database.EnsureCreated();

        // Idempotencia: ha már van ügyfél adat, nem szúrunk be újra
        if (db.Ugyfelek.Any())
            return;

        var now = DateTime.Now;

        // -----------------------------------------------------------------------
        // 1. ÜGYFELEK (10 db)
        // -----------------------------------------------------------------------
        var ugyfelek = new List<Ugyfel>
        {
            new() { Nev = "Biztovill Kft.",               UgyfelTipus = UgyfelTipus.Kft,         Cim = "1117 Budapest, Irinyi József u. 4.",         Email = "biztovill@example.hu",         Telefon = "+36 1 234 5678", Adoszam = "11111111-2-41", Aktiv = true,  Letrehozva = now.AddMonths(-18) },
            new() { Nev = "MérésTechnika Zrt.",            UgyfelTipus = UgyfelTipus.Zrt,         Cim = "4025 Debrecen, Piac u. 22.",                 Email = "merestechnika@example.hu",      Telefon = "+36 52 123 456", Adoszam = "22222222-2-09", Aktiv = true,  Letrehozva = now.AddMonths(-14) },
            new() { Nev = "Kovács János",                  UgyfelTipus = UgyfelTipus.Maganszemely, Cim = "6720 Szeged, Klauzál tér 7.",               Email = "kovacs.janos@example.hu",       Telefon = "+36 62 987 654", Adoszam = "33333333-1-06", Aktiv = true,  Letrehozva = now.AddMonths(-10) },
            new() { Nev = "Dunántúli Energia Bt.",         UgyfelTipus = UgyfelTipus.Bt,          Cim = "7621 Pécs, Király u. 15.",                   Email = "dunantulienergia@example.hu",   Telefon = "+36 72 456 789", Adoszam = "44444444-2-02", Aktiv = true,  Letrehozva = now.AddMonths(-22) },
            new() { Nev = "Magyar Gázszolgáltató Rt.",     UgyfelTipus = UgyfelTipus.Rt,          Cim = "3525 Miskolc, Széchenyi u. 12.",             Email = "magyargaz@example.hu",          Telefon = "+36 46 321 654", Adoszam = "55555555-2-05", Aktiv = true,  Letrehozva = now.AddMonths(-30) },
            new() { Nev = "Fehérvári Műszerész Kft.",      UgyfelTipus = UgyfelTipus.Kft,         Cim = "8000 Székesfehérvár, Budai u. 1.",           Email = "fehervarimuszeresz@example.hu", Telefon = "+36 22 654 321", Adoszam = "66666666-2-07", Aktiv = true,  Letrehozva = now.AddMonths(-8)  },
            new() { Nev = "Szabó Péter EV",                UgyfelTipus = UgyfelTipus.Egyeni,      Cim = "9021 Győr, Baross u. 3.",                    Email = "szabo.peter@example.hu",        Telefon = "+36 96 111 222", Adoszam = "77777777-1-08", Aktiv = true,  Letrehozva = now.AddMonths(-6)  },
            new() { Nev = "GreenPower Nonprofit Kft.",     UgyfelTipus = UgyfelTipus.Nonprofit,   Cim = "6000 Kecskemét, Rákóczi u. 20.",             Email = "greenpower@example.hu",         Telefon = "+36 76 333 444", Adoszam = "88888888-2-03", Aktiv = true,  Letrehozva = now.AddMonths(-12) },
            new() { Nev = "Alföldi Hitelesítő Kft.",       UgyfelTipus = UgyfelTipus.Kft,         Cim = "5000 Szolnok, Ady Endre u. 5.",              Email = "alfoldi@example.hu",            Telefon = "+36 56 555 666", Adoszam = "99999999-2-16", Aktiv = true,  Letrehozva = now.AddMonths(-9)  },
            new() { Nev = "Városi Közművek Zrt.",           UgyfelTipus = UgyfelTipus.Zrt,         Cim = "4400 Nyíregyháza, Kossuth tér 1.",           Email = "varosikozmuvek@example.hu",     Telefon = "+36 42 777 888", Adoszam = "10101010-2-15", Aktiv = false, Letrehozva = now.AddMonths(-36) },
        };
        db.Ugyfelek.AddRange(ugyfelek);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 2. TELEPHELYEK (15 db) – 2-3 telephely ügyfelenként
        // -----------------------------------------------------------------------
        var telephelyek = new List<Telephely>
        {
            // Biztovill Kft. (id=1)
            new() { UgyfelId = ugyfelek[0].Id, Nev = "Biztovill – Főtelep",          Cim = "1117 Budapest, Irinyi József u. 4.",    Kapcsolattarto = "Tóth Gábor",    Telefon = "+36 1 234 5679", Email = "totg@biztovill.example.hu",    Aktiv = true, Letrehozva = now.AddMonths(-18) },
            new() { UgyfelId = ugyfelek[0].Id, Nev = "Biztovill – Raktár",           Cim = "1095 Budapest, Soroksári út 48.",       Kapcsolattarto = "Nagy Éva",      Telefon = "+36 1 234 5680", Email = "nagye@biztovill.example.hu",   Aktiv = true, Letrehozva = now.AddMonths(-16) },
            // MérésTechnika Zrt. (id=2)
            new() { UgyfelId = ugyfelek[1].Id, Nev = "MérésTechnika – Debrecen",     Cim = "4025 Debrecen, Piac u. 22.",           Kapcsolattarto = "Balogh Péter",  Telefon = "+36 52 123 457", Email = "baloghp@merestechnika.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-14) },
            new() { UgyfelId = ugyfelek[1].Id, Nev = "MérésTechnika – Nyíregyháza",  Cim = "4400 Nyíregyháza, Sóstói út 10.",      Kapcsolattarto = "Fekete Mária",  Telefon = "+36 42 654 321", Email = "feketm@merestechnika.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-12) },
            // Kovács János (id=3)
            new() { UgyfelId = ugyfelek[2].Id, Nev = "Kovács – Szegedi műhely",      Cim = "6720 Szeged, Klauzál tér 7.",          Kapcsolattarto = "Kovács János",  Telefon = "+36 62 987 654", Email = "kovacs.janos@example.hu",       Aktiv = true, Letrehozva = now.AddMonths(-10) },
            // Dunántúli Energia Bt. (id=4)
            new() { UgyfelId = ugyfelek[3].Id, Nev = "Dunántúli – Pécs",             Cim = "7621 Pécs, Király u. 15.",             Kapcsolattarto = "Varga László",  Telefon = "+36 72 456 790", Email = "vargal@dunantulienergia.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-22) },
            new() { UgyfelId = ugyfelek[3].Id, Nev = "Dunántúli – Kaposvár",         Cim = "7400 Kaposvár, Fő u. 30.",             Kapcsolattarto = "Horváth Anna",  Telefon = "+36 82 111 222", Email = "horvata@dunantulienergia.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-20) },
            // Magyar Gázszolgáltató Rt. (id=5)
            new() { UgyfelId = ugyfelek[4].Id, Nev = "MagyarGáz – Miskolc",          Cim = "3525 Miskolc, Széchenyi u. 12.",       Kapcsolattarto = "Molnár Zoltán", Telefon = "+36 46 321 655", Email = "molnarz@magyargaz.example.hu",  Aktiv = true, Letrehozva = now.AddMonths(-30) },
            new() { UgyfelId = ugyfelek[4].Id, Nev = "MagyarGáz – Eger",             Cim = "3300 Eger, Dobó tér 2.",               Kapcsolattarto = "Kiss Ildikó",   Telefon = "+36 36 444 555", Email = "kissi@magyargaz.example.hu",    Aktiv = true, Letrehozva = now.AddMonths(-28) },
            // Fehérvári Műszerész Kft. (id=6)
            new() { UgyfelId = ugyfelek[5].Id, Nev = "Fehérvári – Székesfehérvár",   Cim = "8000 Székesfehérvár, Budai u. 1.",     Kapcsolattarto = "Simon Béla",    Telefon = "+36 22 654 322", Email = "simonb@fehervarimuszeresz.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-8) },
            // Szabó Péter EV (id=7)
            new() { UgyfelId = ugyfelek[6].Id, Nev = "Szabó – Győri iroda",          Cim = "9021 Győr, Baross u. 3.",              Kapcsolattarto = "Szabó Péter",   Telefon = "+36 96 111 223", Email = "szabo.peter@example.hu",        Aktiv = true, Letrehozva = now.AddMonths(-6)  },
            // GreenPower Nonprofit Kft. (id=8)
            new() { UgyfelId = ugyfelek[7].Id, Nev = "GreenPower – Kecskemét",       Cim = "6000 Kecskemét, Rákóczi u. 20.",       Kapcsolattarto = "Lakatos Norbert", Telefon = "+36 76 333 445", Email = "lakatn@greenpower.example.hu", Aktiv = true, Letrehozva = now.AddMonths(-12) },
            // Alföldi Hitelesítő Kft. (id=9)
            new() { UgyfelId = ugyfelek[8].Id, Nev = "Alföldi – Szolnok",            Cim = "5000 Szolnok, Ady Endre u. 5.",        Kapcsolattarto = "Farkas Tibor",  Telefon = "+36 56 555 667", Email = "farkast@alfoldi.example.hu",    Aktiv = true, Letrehozva = now.AddMonths(-9)  },
            // Városi Közművek Zrt. (id=10)
            new() { UgyfelId = ugyfelek[9].Id, Nev = "Városi Közművek – Nyíregyháza",Cim = "4400 Nyíregyháza, Kossuth tér 1.",     Kapcsolattarto = "Papp Csilla",   Telefon = "+36 42 777 889", Email = "pappc@varosikozmuvek.example.hu", Aktiv = false, Letrehozva = now.AddMonths(-36) },
            new() { UgyfelId = ugyfelek[9].Id, Nev = "Városi Közművek – Kisvárda",   Cim = "4600 Kisvárda, Szent László u. 6.",    Kapcsolattarto = "Oláh Sándor",   Telefon = "+36 45 222 333", Email = "olahs@varosikozmuvek.example.hu",  Aktiv = false, Letrehozva = now.AddMonths(-34) },
        };
        db.Telephelyek.AddRange(telephelyek);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 3. GYÁRTÓK (5 db)
        // -----------------------------------------------------------------------
        var gyartok = new List<Gyarto>
        {
            new() { Nev = "Siemens",          Orszag = "Németország", Weboldal = "https://www.siemens.com",           Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Honeywell",        Orszag = "USA",         Weboldal = "https://www.honeywell.com",         Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Endress+Hauser",   Orszag = "Svájc",       Weboldal = "https://www.endress.com",           Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "ABB",              Orszag = "Svédország",  Weboldal = "https://www.abb.com",               Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Yokogawa",         Orszag = "Japán",       Weboldal = "https://www.yokogawa.com",          Aktiv = true, Letrehozva = now.AddMonths(-24) },
        };
        db.Gyartok.AddRange(gyartok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 4. ESZKÖZÖK (20 db)
        // -----------------------------------------------------------------------
        var eszkozok = new List<Eszkoz>
        {
            // Biztovill Kft.
            new() { UgyfelId = ugyfelek[0].Id, TelephelyId = telephelyek[0].Id, GyartoId = gyartok[0].Id, Nev = "Digitális nyomásmérő",            Tipus = "Nyomásmérő",              GyariSzam = "SM-2023-001", Aktiv = true, Letrehozva = now.AddMonths(-15) },
            new() { UgyfelId = ugyfelek[0].Id, TelephelyId = telephelyek[1].Id, GyartoId = gyartok[2].Id, Nev = "Ipari hőmérsékletmérő",           Tipus = "Hőmérsékletmérő",         GyariSzam = "EH-2022-042", Aktiv = true, Letrehozva = now.AddMonths(-14) },
            // MérésTechnika Zrt.
            new() { UgyfelId = ugyfelek[1].Id, TelephelyId = telephelyek[2].Id, GyartoId = gyartok[1].Id, Nev = "Ultrahangos áramlásmérő",         Tipus = "Áramlásmérő",             GyariSzam = "HW-2021-115", Aktiv = true, Letrehozva = now.AddMonths(-20) },
            new() { UgyfelId = ugyfelek[1].Id, TelephelyId = telephelyek[3].Id, GyartoId = gyartok[4].Id, Nev = "Gázérzékelő – metán",            Tipus = "Gázérzékelő",             GyariSzam = "YK-2022-088", Aktiv = true, Letrehozva = now.AddMonths(-18) },
            new() { UgyfelId = ugyfelek[1].Id, TelephelyId = telephelyek[2].Id, GyartoId = gyartok[3].Id, Nev = "Villamos energiamérő 3 fázisú",   Tipus = "Energiamérő",             GyariSzam = "AB-2023-207", Aktiv = true, Letrehozva = now.AddMonths(-10) },
            // Kovács János
            new() { UgyfelId = ugyfelek[2].Id, TelephelyId = telephelyek[4].Id, GyartoId = gyartok[0].Id, Nev = "Analóg manométer",               Tipus = "Nyomásmérő",              GyariSzam = "SM-2020-330", Aktiv = true, Letrehozva = now.AddMonths(-30) },
            // Dunántúli Energia Bt.
            new() { UgyfelId = ugyfelek[3].Id, TelephelyId = telephelyek[5].Id, GyartoId = gyartok[2].Id, Nev = "PT100 hőmérsékletérzékelő",      Tipus = "Hőmérsékletmérő",         GyariSzam = "EH-2021-501", Aktiv = true, Letrehozva = now.AddMonths(-25) },
            new() { UgyfelId = ugyfelek[3].Id, TelephelyId = telephelyek[6].Id, GyartoId = gyartok[1].Id, Nev = "Mágneses áramlásmérő DN50",      Tipus = "Áramlásmérő",             GyariSzam = "HW-2022-214", Aktiv = true, Letrehozva = now.AddMonths(-22) },
            // Magyar Gázszolgáltató Rt.
            new() { UgyfelId = ugyfelek[4].Id, TelephelyId = telephelyek[7].Id, GyartoId = gyartok[4].Id, Nev = "Gázkromatográf",                 Tipus = "Gázelemző",               GyariSzam = "YK-2020-055", Aktiv = true, Letrehozva = now.AddMonths(-40) },
            new() { UgyfelId = ugyfelek[4].Id, TelephelyId = telephelyek[8].Id, GyartoId = gyartok[3].Id, Nev = "Differenciálnyomás-mérő",        Tipus = "Nyomásmérő",              GyariSzam = "AB-2021-399", Aktiv = true, Letrehozva = now.AddMonths(-35) },
            new() { UgyfelId = ugyfelek[4].Id, TelephelyId = telephelyek[7].Id, GyartoId = gyartok[0].Id, Nev = "CO/CO₂ gázérzékelő kombó",      Tipus = "Gázérzékelő",             GyariSzam = "SM-2022-178", Aktiv = true, Letrehozva = now.AddMonths(-16) },
            // Fehérvári Műszerész Kft.
            new() { UgyfelId = ugyfelek[5].Id, TelephelyId = telephelyek[9].Id, GyartoId = gyartok[2].Id, Nev = "Rezgéssebesség-mérő",            Tipus = "Rezgésmérő",              GyariSzam = "EH-2023-612", Aktiv = true, Letrehozva = now.AddMonths(-7)  },
            new() { UgyfelId = ugyfelek[5].Id, TelephelyId = telephelyek[9].Id, GyartoId = gyartok[1].Id, Nev = "Digitális multiméter",           Tipus = "Villamos mérő",           GyariSzam = "HW-2022-441", Aktiv = true, Letrehozva = now.AddMonths(-8)  },
            // Szabó Péter EV
            new() { UgyfelId = ugyfelek[6].Id, TelephelyId = telephelyek[10].Id, GyartoId = gyartok[3].Id, Nev = "Hordozható kalibrátor",         Tipus = "Kalibrátor",              GyariSzam = "AB-2023-050", Aktiv = true, Letrehozva = now.AddMonths(-5)  },
            // GreenPower Nonprofit Kft.
            new() { UgyfelId = ugyfelek[7].Id, TelephelyId = telephelyek[11].Id, GyartoId = gyartok[4].Id, Nev = "Napelemtelep teljesítménymérő",  Tipus = "Energiamérő",             GyariSzam = "YK-2023-301", Aktiv = true, Letrehozva = now.AddMonths(-11) },
            new() { UgyfelId = ugyfelek[7].Id, TelephelyId = telephelyek[11].Id, GyartoId = gyartok[0].Id, Nev = "Szélsebességmérő anemométer",   Tipus = "Környezeti mérő",         GyariSzam = "SM-2022-555", Aktiv = true, Letrehozva = now.AddMonths(-13) },
            // Alföldi Hitelesítő Kft.
            new() { UgyfelId = ugyfelek[8].Id, TelephelyId = telephelyek[12].Id, GyartoId = gyartok[2].Id, Nev = "Referencia nyomásmérő",         Tipus = "Nyomásmérő",              GyariSzam = "EH-2022-900", Aktiv = true, Letrehozva = now.AddMonths(-8)  },
            new() { UgyfelId = ugyfelek[8].Id, TelephelyId = telephelyek[12].Id, GyartoId = gyartok[1].Id, Nev = "Referencia hőmérsékletmérő",    Tipus = "Hőmérsékletmérő",         GyariSzam = "HW-2021-870", Aktiv = true, Letrehozva = now.AddMonths(-9)  },
            // Városi Közművek Zrt.
            new() { UgyfelId = ugyfelek[9].Id, TelephelyId = telephelyek[13].Id, GyartoId = gyartok[3].Id, Nev = "Vízóra DN100",                  Tipus = "Áramlásmérő",             GyariSzam = "AB-2019-777", Aktiv = false, Letrehozva = now.AddMonths(-50) },
            new() { UgyfelId = ugyfelek[9].Id, TelephelyId = telephelyek[14].Id, GyartoId = gyartok[4].Id, Nev = "Elektromos fogyasztásmérő",     Tipus = "Energiamérő",             GyariSzam = "YK-2020-112", Aktiv = false, Letrehozva = now.AddMonths(-45) },
        };
        db.Eszkozok.AddRange(eszkozok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 5. MÉRÉS TÍPUSOK (5 db)
        // -----------------------------------------------------------------------
        var meresTipusok = new List<MeresTipus>
        {
            new() { Nev = "Nyomáspróba",                    Leiras = "Nyomástartó edény és csővezeték nyomáspróbája",         ErvenyessegHonap = 12, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Hőmérséklet kalibráció",         Leiras = "Hőmérsékletmérő eszközök kalibrálása referencia alapján", ErvenyessegHonap = 6,  Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Áramlásmérés",                   Leiras = "Folyadék- és gázáramlás hitelesítő mérése",              ErvenyessegHonap = 12, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Gázszivárgás vizsgálat",         Leiras = "Éghető és mérgező gázok szivárgásának ellenőrzése",      ErvenyessegHonap = 3,  Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Villamos szigetelés vizsgálat",  Leiras = "Villamos berendezések szigetelési ellenállásának mérése", ErvenyessegHonap = 24, Letrehozva = now.AddMonths(-24) },
        };
        db.MeresTipusok.AddRange(meresTipusok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 6. MÉRÉSEK (30 db) – különböző státuszok, lejárt / hamarosan lejáró / jövőbeli
        // -----------------------------------------------------------------------
        var meresek = new List<Meres>
        {
            // --- Elvégzett, aktív (következő dátum a jövőben) ---
            new() { EszkozId = eszkozok[0].Id,  MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-10), KovetkezoDatum = now.AddMonths(2),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Nincs rendellenesség",          Letrehozva = now.AddMonths(-10) },
            new() { EszkozId = eszkozok[1].Id,  MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(-4),  KovetkezoDatum = now.AddMonths(2),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Eltérés 0,2°C – javítva",      Letrehozva = now.AddMonths(-4)  },
            new() { EszkozId = eszkozok[2].Id,  MeresTipusId = meresTipusok[2].Id, Datum = now.AddMonths(-8),  KovetkezoDatum = now.AddMonths(4),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Hitelesítési jegy kiadva",      Letrehozva = now.AddMonths(-8)  },
            new() { EszkozId = eszkozok[3].Id,  MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(-2),  KovetkezoDatum = now.AddMonths(1),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Szivárgás nem észlelhető",      Letrehozva = now.AddMonths(-2)  },
            new() { EszkozId = eszkozok[4].Id,  MeresTipusId = meresTipusok[4].Id, Datum = now.AddMonths(-18), KovetkezoDatum = now.AddMonths(6),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Szigetelési ellenállás: 500 MΩ",Letrehozva = now.AddMonths(-18) },
            new() { EszkozId = eszkozok[5].Id,  MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-6),  KovetkezoDatum = now.AddMonths(6),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Nyomáspróba átment",            Letrehozva = now.AddMonths(-6)  },
            new() { EszkozId = eszkozok[6].Id,  MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(-3),  KovetkezoDatum = now.AddMonths(3),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Pontosság ±0,1°C",              Letrehozva = now.AddMonths(-3)  },
            new() { EszkozId = eszkozok[7].Id,  MeresTipusId = meresTipusok[2].Id, Datum = now.AddMonths(-12), KovetkezoDatum = now.AddMonths(0),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Kalibrálás elvégezve",          Letrehozva = now.AddMonths(-12) },
            new() { EszkozId = eszkozok[8].Id,  MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(-1),  KovetkezoDatum = now.AddMonths(2),   MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "CO szint rendben",              Letrehozva = now.AddMonths(-1)  },
            new() { EszkozId = eszkozok[9].Id,  MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-20), KovetkezoDatum = now.AddMonths(-8),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Nyomáspróba sikeres",           Letrehozva = now.AddMonths(-20) },
            new() { EszkozId = eszkozok[10].Id, MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(-5),  KovetkezoDatum = now.AddMonths(-2),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Gázérzékelő rendben",           Letrehozva = now.AddMonths(-5)  },

            // --- Hamarosan lejáró (következő dátum 30 napon belül) ---
            new() { EszkozId = eszkozok[11].Id, MeresTipusId = meresTipusok[4].Id, Datum = now.AddMonths(-23), KovetkezoDatum = now.AddDays(15),    MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Hamarosan megújítandó",         Letrehozva = now.AddMonths(-23) },
            new() { EszkozId = eszkozok[12].Id, MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(-5),  KovetkezoDatum = now.AddDays(10),    MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "10 napon belül lejár",          Letrehozva = now.AddMonths(-5)  },
            new() { EszkozId = eszkozok[13].Id, MeresTipusId = meresTipusok[2].Id, Datum = now.AddMonths(-11), KovetkezoDatum = now.AddDays(25),    MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "25 napon belül lejár",          Letrehozva = now.AddMonths(-11) },
            new() { EszkozId = eszkozok[14].Id, MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-11), KovetkezoDatum = now.AddDays(5),     MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "5 napon belül lejár – sürgős!",  Letrehozva = now.AddMonths(-11) },

            // --- Már lejárt (következő dátum a múltban) ---
            new() { EszkozId = eszkozok[15].Id, MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(-4),  KovetkezoDatum = now.AddMonths(-1),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Lejárt – megújítás szükséges",  Letrehozva = now.AddMonths(-4)  },
            new() { EszkozId = eszkozok[16].Id, MeresTipusId = meresTipusok[4].Id, Datum = now.AddMonths(-26), KovetkezoDatum = now.AddMonths(-2),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "Lejárt – ütemezni kell",        Letrehozva = now.AddMonths(-26) },
            new() { EszkozId = eszkozok[17].Id, MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(-8),  KovetkezoDatum = now.AddMonths(-2),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Megfelelő",    Megjegyzes = "2 hónapja lejárt",              Letrehozva = now.AddMonths(-8)  },
            new() { EszkozId = eszkozok[18].Id, MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-15), KovetkezoDatum = now.AddMonths(-3),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Nem megfelelő", Megjegyzes = "Szivárgás észlelve – javítás szükséges", Letrehozva = now.AddMonths(-15) },
            new() { EszkozId = eszkozok[19].Id, MeresTipusId = meresTipusok[2].Id, Datum = now.AddMonths(-14), KovetkezoDatum = now.AddMonths(-2),  MeresStatusz = MeresStatusz.Elvegezve, Eredmeny = "Nem megfelelő", Megjegyzes = "Eszköz csere folyamatban",      Letrehozva = now.AddMonths(-14) },

            // --- Tervezett (jövőbeli) ---
            new() { EszkozId = eszkozok[0].Id,  MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(1),   KovetkezoDatum = now.AddMonths(4),   MeresStatusz = MeresStatusz.Tervezett,  Eredmeny = null,           Megjegyzes = "Ütemezett gázszivárgás vizsgálat", Letrehozva = now.AddDays(-5)    },
            new() { EszkozId = eszkozok[2].Id,  MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(2),   KovetkezoDatum = now.AddMonths(8),   MeresStatusz = MeresStatusz.Tervezett,  Eredmeny = null,           Megjegyzes = "Éves kalibráció",               Letrehozva = now.AddDays(-3)    },
            new() { EszkozId = eszkozok[5].Id,  MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(3),   KovetkezoDatum = now.AddMonths(15),  MeresStatusz = MeresStatusz.Tervezett,  Eredmeny = null,           Megjegyzes = "Tervezett nyomáspróba",         Letrehozva = now.AddDays(-2)    },

            // --- Folyamatban ---
            new() { EszkozId = eszkozok[8].Id,  MeresTipusId = meresTipusok[4].Id, Datum = now.AddDays(-1),    KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Folyamatban, Eredmeny = null,          Megjegyzes = "Vizsgálat folyamatban",         Letrehozva = now.AddDays(-1)    },
            new() { EszkozId = eszkozok[11].Id, MeresTipusId = meresTipusok[2].Id, Datum = now,                KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Folyamatban, Eredmeny = null,          Megjegyzes = "Ma megkezdett mérés",           Letrehozva = now                },

            // --- Elutasítva ---
            new() { EszkozId = eszkozok[9].Id,  MeresTipusId = meresTipusok[3].Id, Datum = now.AddMonths(-3),  KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Elutasitva, Eredmeny = "Elutasítva", Megjegyzes = "Hiányos dokumentáció",          Letrehozva = now.AddMonths(-3)  },
            new() { EszkozId = eszkozok[15].Id, MeresTipusId = meresTipusok[0].Id, Datum = now.AddMonths(-7),  KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Elutasitva, Eredmeny = "Elutasítva", Megjegyzes = "Eszköz nem hozzáférhető",       Letrehozva = now.AddMonths(-7)  },
            new() { EszkozId = eszkozok[18].Id, MeresTipusId = meresTipusok[1].Id, Datum = now.AddMonths(-9),  KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Elutasitva, Eredmeny = "Elutasítva", Megjegyzes = "Ügyfél lemondta",               Letrehozva = now.AddMonths(-9)  },
            new() { EszkozId = eszkozok[19].Id, MeresTipusId = meresTipusok[4].Id, Datum = now.AddMonths(-11), KovetkezoDatum = null,               MeresStatusz = MeresStatusz.Elutasitva, Eredmeny = "Elutasítva", Megjegyzes = "Nem megfelelő feltételek",      Letrehozva = now.AddMonths(-11) },
        };
        db.Meresek.AddRange(meresek);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 7. HATÓSÁGOK (3 db)
        // -----------------------------------------------------------------------
        var hatosagok = new List<Hatosag>
        {
            new() { Nev = "Budapest Főváros Kormányhivatala",              Rovidites = "BFKH",  Cim = "1051 Budapest, Sas u. 19.",     Weboldal = "https://bfkh.gov.hu",  Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Magyar Energetikai és Közmű-szabályozási Hivatal", Rovidites = "MEKH",  Cim = "1081 Budapest, II. János Pál pápa tér 7.", Weboldal = "https://mekh.hu", Aktiv = true, Letrehozva = now.AddMonths(-24) },
            new() { Nev = "Nemzeti Földügyi Központ Hatóság",              Rovidites = "NFKH",  Cim = "1149 Budapest, Bosnyák tér 5.", Weboldal = "https://nfkh.gov.hu",  Aktiv = true, Letrehozva = now.AddMonths(-24) },
        };
        db.Hatosagok.AddRange(hatosagok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 8. HITELESÍTÉSEK (15 db) – különböző státuszok, lejárt és hamarosan lejáró
        // -----------------------------------------------------------------------
        var hitelesitesek = new List<Hitelesites>
        {
            // Elfogadva – érvényes
            new() { EszkozId = eszkozok[0].Id,  HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2024-0101", Datum = now.AddMonths(-10), LejaratDatum = now.AddMonths(2),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Érvényes hitelesítés",           Letrehozva = now.AddMonths(-10) },
            new() { EszkozId = eszkozok[1].Id,  HatosagId = hatosagok[1].Id, Ugyszam = "MEKH-2024-0215", Datum = now.AddMonths(-4),  LejaratDatum = now.AddMonths(8),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Elfogadva, érvényes",            Letrehozva = now.AddMonths(-4)  },
            new() { EszkozId = eszkozok[2].Id,  HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2023-0550", Datum = now.AddMonths(-8),  LejaratDatum = now.AddMonths(4),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Jóváhagyva",                     Letrehozva = now.AddMonths(-8)  },
            new() { EszkozId = eszkozok[4].Id,  HatosagId = hatosagok[1].Id, Ugyszam = "MEKH-2022-1102", Datum = now.AddMonths(-18), LejaratDatum = now.AddMonths(6),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Megfelelt",                      Letrehozva = now.AddMonths(-18) },
            new() { EszkozId = eszkozok[6].Id,  HatosagId = hatosagok[2].Id, Ugyszam = "NFKH-2024-0033", Datum = now.AddMonths(-3),  LejaratDatum = now.AddMonths(9),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Hitelesítő jegy mellékelve",     Letrehozva = now.AddMonths(-3)  },
            new() { EszkozId = eszkozok[8].Id,  HatosagId = hatosagok[1].Id, Ugyszam = "MEKH-2023-0788", Datum = now.AddMonths(-12), LejaratDatum = now.AddMonths(3),   HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "Érvényes, megújítás hamarosan",  Letrehozva = now.AddMonths(-12) },
            // Hamarosan lejáró (Elfogadva, de LejaratDatum 30 napon belül)
            new() { EszkozId = eszkozok[11].Id, HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2022-0190", Datum = now.AddMonths(-23), LejaratDatum = now.AddDays(20),    HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "20 nap múlva lejár!",            Letrehozva = now.AddMonths(-23) },
            new() { EszkozId = eszkozok[13].Id, HatosagId = hatosagok[2].Id, Ugyszam = "NFKH-2023-0400", Datum = now.AddMonths(-11), LejaratDatum = now.AddDays(7),     HitelesitesStatusz = HitelesitesStatusz.Elfogadva, Megjegyzes = "1 héten belül lejár – sürgős!",  Letrehozva = now.AddMonths(-11) },
            // Lejárt
            new() { EszkozId = eszkozok[9].Id,  HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2021-0933", Datum = now.AddMonths(-20), LejaratDatum = now.AddMonths(-8),  HitelesitesStatusz = HitelesitesStatusz.Lejart,    Megjegyzes = "Lejárt – megújítás folyamatban", Letrehozva = now.AddMonths(-20) },
            new() { EszkozId = eszkozok[15].Id, HatosagId = hatosagok[1].Id, Ugyszam = "MEKH-2021-0655", Datum = now.AddMonths(-26), LejaratDatum = now.AddMonths(-2),  HitelesitesStatusz = HitelesitesStatusz.Lejart,    Megjegyzes = "Lejárt",                         Letrehozva = now.AddMonths(-26) },
            new() { EszkozId = eszkozok[18].Id, HatosagId = hatosagok[2].Id, Ugyszam = "NFKH-2020-0011", Datum = now.AddMonths(-15), LejaratDatum = now.AddMonths(-3),  HitelesitesStatusz = HitelesitesStatusz.Lejart,    Megjegyzes = "Lejárt – eszköz inaktív",        Letrehozva = now.AddMonths(-15) },
            new() { EszkozId = eszkozok[19].Id, HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2019-1200", Datum = now.AddMonths(-14), LejaratDatum = now.AddMonths(-2),  HitelesitesStatusz = HitelesitesStatusz.Lejart,    Megjegyzes = "Lejárt eszköz",                  Letrehozva = now.AddMonths(-14) },
            // Függőben
            new() { EszkozId = eszkozok[14].Id, HatosagId = hatosagok[1].Id, Ugyszam = "MEKH-2025-0001", Datum = now.AddDays(-5),   LejaratDatum = null,               HitelesitesStatusz = HitelesitesStatusz.Fuggoben,  Megjegyzes = "Benyújtva, várakozás alatt",     Letrehozva = now.AddDays(-5)    },
            new() { EszkozId = eszkozok[16].Id, HatosagId = hatosagok[2].Id, Ugyszam = "NFKH-2025-0002", Datum = now.AddDays(-3),   LejaratDatum = null,               HitelesitesStatusz = HitelesitesStatusz.Fuggoben,  Megjegyzes = "Dokumentumok feldolgozás alatt", Letrehozva = now.AddDays(-3)    },
            // Elutasítva
            new() { EszkozId = eszkozok[17].Id, HatosagId = hatosagok[0].Id, Ugyszam = "BFKH-2024-0777", Datum = now.AddMonths(-7),  LejaratDatum = null,               HitelesitesStatusz = HitelesitesStatusz.Elutasitva, Megjegyzes = "Hiányos dokumentáció – visszaküldve", Letrehozva = now.AddMonths(-7) },
        };
        db.Hitelesitesek.AddRange(hitelesitesek);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 9. TANÚSÍTVÁNYOK (8 db)
        // -----------------------------------------------------------------------
        var tanusitványok = new List<Tanusitvany>
        {
            new() { UgyfelId = ugyfelek[0].Id, Nev = "ISO 9001:2015 Minőségirányítás",           Szam = "ISO9001-2024-001", KiadoDatum = now.AddMonths(-6),  LejaratDatum = now.AddMonths(30), Megjegyzes = "Tanúsított minőségirányítási rendszer",          Letrehozva = now.AddMonths(-6)  },
            new() { UgyfelId = ugyfelek[1].Id, Nev = "ISO/IEC 17025:2017 Laboratórium",          Szam = "LAB-2023-0042",   KiadoDatum = now.AddMonths(-12), LejaratDatum = now.AddMonths(24), Megjegyzes = "Akkreditált kalibrálólaboratórium",               Letrehozva = now.AddMonths(-12) },
            new() { UgyfelId = ugyfelek[1].Id, Nev = "MSZ EN ISO 5167 Áramlásmérés",             Szam = "MSZEN-2022-0115", KiadoDatum = now.AddMonths(-18), LejaratDatum = now.AddDays(25),   Megjegyzes = "Hamarosan lejár – 25 nap múlva",                  Letrehozva = now.AddMonths(-18) },
            new() { UgyfelId = ugyfelek[3].Id, Nev = "ISO 14001:2015 Környezetirányítás",        Szam = "ISO14001-2023-033",KiadoDatum = now.AddMonths(-9),  LejaratDatum = now.AddMonths(27), Megjegyzes = "Érvényes tanúsítvány",                            Letrehozva = now.AddMonths(-9)  },
            new() { UgyfelId = ugyfelek[4].Id, Nev = "MSZ EN 1775 Gázellátás szabvány",          Szam = "MSZEN-2021-0299", KiadoDatum = now.AddMonths(-30), LejaratDatum = now.AddMonths(-6), Megjegyzes = "Lejárt – megújítás szükséges",                    Letrehozva = now.AddMonths(-30) },
            new() { UgyfelId = ugyfelek[5].Id, Nev = "ISO 45001:2018 Munkahelyi egészség",       Szam = "ISO45001-2024-007",KiadoDatum = now.AddMonths(-4),  LejaratDatum = now.AddMonths(32), Megjegyzes = "Friss tanúsítvány",                               Letrehozva = now.AddMonths(-4)  },
            new() { UgyfelId = ugyfelek[7].Id, Nev = "MSZ EN 61010 Villamos biztonság",          Szam = "MSZEN-2023-0588", KiadoDatum = now.AddMonths(-8),  LejaratDatum = now.AddDays(10),   Megjegyzes = "10 napon belül lejár – sürgős megújítás!",        Letrehozva = now.AddMonths(-8)  },
            new() { UgyfelId = ugyfelek[8].Id, Nev = "OIML R 111 Hitelesítési referencia",       Szam = "OIML-2022-0044",  KiadoDatum = now.AddMonths(-15), LejaratDatum = now.AddMonths(9),  Megjegyzes = "OIML akkreditáció – érvényes",                    Letrehozva = now.AddMonths(-15) },
        };
        db.Tanusitvanyok.AddRange(tanusitványok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 10. KÉPZÉSEK (6 db)
        // -----------------------------------------------------------------------
        var kepzesek = new List<Kepzes>
        {
            new() { Nev = "Villamos biztonsági alapképzés",         Datum = now.AddMonths(-12), LejaratDatum = now.AddMonths(12),  Resztvevo = "Tóth Gábor, Nagy Éva, Simon Béla",              Megjegyzes = "MSZ EN 50110 szerinti villamossági oktatás",  Letrehozva = now.AddMonths(-12) },
            new() { Nev = "Gázszerelő szakmai továbbképzés",        Datum = now.AddMonths(-6),  LejaratDatum = now.AddMonths(18),  Resztvevo = "Molnár Zoltán, Varga László",                   Megjegyzes = "Gázipari munkák biztonságos végzése",         Letrehozva = now.AddMonths(-6)  },
            new() { Nev = "Mérés technikus tanfolyam",              Datum = now.AddMonths(-3),  LejaratDatum = now.AddMonths(21),  Resztvevo = "Balogh Péter, Fekete Mária, Farkas Tibor",      Megjegyzes = "Mérőeszközök kezelése és karbantartása",      Letrehozva = now.AddMonths(-3)  },
            new() { Nev = "Tűzvédelmi oktatás",                    Datum = now.AddMonths(-2),  LejaratDatum = now.AddDays(20),    Resztvevo = "Kiss Ildikó, Horváth Anna, Lakatos Norbert",    Megjegyzes = "Kötelező éves tűzvédelmi oktatás – lejár!",   Letrehozva = now.AddMonths(-2)  },
            new() { Nev = "ISO 9001 belső auditor képzés",          Datum = now.AddMonths(-18), LejaratDatum = now.AddMonths(-6),  Resztvevo = "Szabó Péter",                                   Megjegyzes = "Lejárt – megújítás szükséges",                Letrehozva = now.AddMonths(-18) },
            new() { Nev = "Munkavédelmi vezető felkészítő",        Datum = now.AddMonths(1),   LejaratDatum = null,               Resztvevo = "Papp Csilla, Oláh Sándor",                      Megjegyzes = "Tervezett képzés – következő hónapban",       Letrehozva = now.AddDays(-7)    },
        };
        db.Kepzesek.AddRange(kepzesek);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 11. KARBANTARTÁSOK (10 db)
        // -----------------------------------------------------------------------
        var karbantartasok = new List<Karbantartas>
        {
            new() { EszkozId = eszkozok[0].Id,  Datum = now.AddMonths(-11), KovetkezoDatum = now.AddMonths(1),  Leiras = "Éves megelőző karbantartás – szenzor tisztítás, tömítések ellenőrzése",    Elvegzo = "Tóth Gábor",    Letrehozva = now.AddMonths(-11) },
            new() { EszkozId = eszkozok[1].Id,  Datum = now.AddMonths(-5),  KovetkezoDatum = now.AddMonths(1),  Leiras = "Kalibrálás és firmware frissítés",                                          Elvegzo = "Nagy Éva",      Letrehozva = now.AddMonths(-5)  },
            new() { EszkozId = eszkozok[2].Id,  Datum = now.AddMonths(-9),  KovetkezoDatum = now.AddMonths(3),  Leiras = "Ultrahangos szonda csere, mérőcella ellenőrzése",                            Elvegzo = "Balogh Péter",  Letrehozva = now.AddMonths(-9)  },
            new() { EszkozId = eszkozok[5].Id,  Datum = now.AddMonths(-7),  KovetkezoDatum = now.AddMonths(5),  Leiras = "Manométer olaj csere, skála hitelesítés",                                    Elvegzo = "Kovács János",  Letrehozva = now.AddMonths(-7)  },
            new() { EszkozId = eszkozok[8].Id,  Datum = now.AddMonths(-13), KovetkezoDatum = now.AddDays(15),   Leiras = "Kromatográf kolonna csere – következő karbantartás hamarosan",               Elvegzo = "Molnár Zoltán", Letrehozva = now.AddMonths(-13) },
            new() { EszkozId = eszkozok[9].Id,  Datum = now.AddMonths(-22), KovetkezoDatum = now.AddMonths(-10), Leiras = "Membrán csere, nyomáskapcsoló beállítás – LEJÁRT KARBANTARTÁS",            Elvegzo = "Molnár Zoltán", Letrehozva = now.AddMonths(-22) },
            new() { EszkozId = eszkozok[11].Id, Datum = now.AddMonths(-25), KovetkezoDatum = now.AddDays(5),    Leiras = "Rezgésszenzorok újrakalibrálása – 5 nap múlva esedékes!",                    Elvegzo = "Simon Béla",    Letrehozva = now.AddMonths(-25) },
            new() { EszkozId = eszkozok[14].Id, Datum = now.AddMonths(-4),  KovetkezoDatum = now.AddMonths(8),  Leiras = "Kalibrátor akkumulátor csere, szoftverfrissítés",                            Elvegzo = "Szabó Péter",   Letrehozva = now.AddMonths(-4)  },
            new() { EszkozId = eszkozok[15].Id, Datum = now.AddMonths(-10), KovetkezoDatum = now.AddMonths(2),  Leiras = "Napelemtelep inverter szerviz, kapcsolatok ellenőrzése",                     Elvegzo = "Lakatos Norbert", Letrehozva = now.AddMonths(-10) },
            new() { EszkozId = eszkozok[16].Id, Datum = now.AddMonths(-14), KovetkezoDatum = now.AddMonths(-2), Leiras = "Szélmérő lapátrendszer csere – LEJÁRT, megújítandó",                        Elvegzo = "Lakatos Norbert", Letrehozva = now.AddMonths(-14) },
        };
        db.Karbantartasok.AddRange(karbantartasok);
        db.SaveChanges();

        // -----------------------------------------------------------------------
        // 12. JOGSZABÁLYOK (5 db) – reális magyar jogszabályok
        // -----------------------------------------------------------------------
        var jogszabalyok = new List<Jogszabaly>
        {
            new() { Szam = "365/2014. (XII. 30.) Korm. rendelet", Cim = "A villamosenergia-ipari építésügyi hatósági engedélyezési eljárásokról",                       HatalyosDatum = new DateTime(2015, 1, 1),  Url = "https://njt.hu/jogszabaly/2014-365-20-22", Aktiv = true,  Megjegyzes = "Villamos energetikai engedélyek",                Letrehozva = now.AddMonths(-24) },
            new() { Szam = "2007. évi LXXXVI. törvény",            Cim = "A villamos energiáról szóló törvény (VET)",                                                   HatalyosDatum = new DateTime(2008, 1, 1),  Url = "https://njt.hu/jogszabaly/2007-86-00-00", Aktiv = true,  Megjegyzes = "A villamosenergia-szektor alapszabályozása",      Letrehozva = now.AddMonths(-24) },
            new() { Szam = "2008. évi XL. törvény",                Cim = "A földgázellátásról szóló törvény (GET)",                                                     HatalyosDatum = new DateTime(2008, 7, 1),  Url = "https://njt.hu/jogszabaly/2008-40-00-00", Aktiv = true,  Megjegyzes = "Gázipari tevékenységek szabályozása",            Letrehozva = now.AddMonths(-24) },
            new() { Szam = "127/1991. (X. 9.) Korm. rendelet",    Cim = "A mérésügyről szóló törvény végrehajtásáról szóló rendelet",                                   HatalyosDatum = new DateTime(1991, 10, 9), Url = "https://njt.hu/jogszabaly/1991-127-20-22", Aktiv = true,  Megjegyzes = "Mérőeszközök hitelesítési követelményei",        Letrehozva = now.AddMonths(-24) },
            new() { Szam = "1991. évi XLV. törvény",               Cim = "A mérésügyről szóló törvény",                                                                 HatalyosDatum = new DateTime(1992, 1, 1),  Url = "https://njt.hu/jogszabaly/1991-45-00-00", Aktiv = true,  Megjegyzes = "Mérésügy, hitelesítés, mintavétel alapszabálya", Letrehozva = now.AddMonths(-24) },
        };
        db.Jogszabalyok.AddRange(jogszabalyok);
        db.SaveChanges();
    }
}
