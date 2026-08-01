/*
	============================================================================
	BiztvillCRM - Kiegészítő teszt adatok: eszközök, kalibrálás,
	felülvizsgálók/bizonyítványok, munkavédelmi oktatás, kockázatértékelés,
	zónatérkép, karbantartás
	============================================================================
	Alap: BiztvillCRM.Data\TestData_Seed.sql-ban létrehozott teszt cég/ügyfél/telephelyek

	Meglévő törzsadatok, amikre hivatkozunk:
	  - Ceg.Id        = 2  ("Biztovill CRM Demo")
	  - Ugyfel.Id     = 28 ("Árvai Zsolt")
	  - Telephelyek: 38 (Székhely), 39 (Kecskemét), 40 (Szeged)
	  - Gyarto.Id: 1 (Gilbarco), 2 (Metrel), 3 (Hectronic), 4 (Tokheim/Dover), 5 (Kyoritsu)
	  - KepzesTipus.Id: 1 (Érintésvédelmi felülv.), 4 (Norma szerinti Villámv. felülv.),
						6 (Erősáramú berendezés felülv.), 7 (Villamos Biztonsági Felülv.),
						9 (Villanyszerelő)
	  - KarbantartasTipus.Id: 2 (Negyedéves, 3 hó), 3 (Féléves, 6 hó), 4 (Éves, 12 hó)

	Tartalom:
	  - 6 eszköz (2 érvényes kalibrálású + 4 az elvárt tartományban vegyesen),
		közülük néhánynak lejárt (2+ éven túli) kalibrálása
	  - Minden eszközhöz kalibrációs esemény(ek)
	  - 3 felülvizsgáló különböző jogosultsággal, mindegyiknek 1-2 bizonyítvány (KepzesTipus)
	  - 2 munkavédelmi oktatás
	  - 2 kockázatértékelés
	  - 2 zónatérkép
	  - 3 karbantartás (vegyesen tervezett/elvégzett)
	============================================================================
*/

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @CegId INT = 2;
DECLARE @UgyfelId INT = 28;
DECLARE @Telephely1 INT = 38; -- Székhely
DECLARE @Telephely2 INT = 39; -- Kecskemét
DECLARE @Telephely3 INT = 40; -- Szeged

------------------------------------------------------------------------------
-- 1) Eszközök (mérőeszközök / berendezések)
------------------------------------------------------------------------------
DECLARE @EszkozTable TABLE (Id INT, Nev NVARCHAR(200));

INSERT INTO Eszkozok (Letrehozva, Nev, GyariSzam, Tipus, Kategoria, GyartoId, UgyfelId, TelephelyId, Aktiv)
OUTPUT inserted.Id, inserted.Nev INTO @EszkozTable
VALUES
	(GETUTCDATE(), N'MI 3325 Multiservicer', N'SN-2024-0001', N'MI 3325', N'Műszer', 2, @UgyfelId, @Telephely1, 1),
	(GETUTCDATE(), N'MI 3125 Industrial', N'SN-2021-0450', N'MI 3125', N'Műszer', 2, @UgyfelId, @Telephely1, 1),
	(GETUTCDATE(), N'KEW 3128 Szigetelésmérő', N'SN-2019-0093', N'KEW 3128', N'Műszer', 5, @UgyfelId, @Telephely2, 1),
	(GETUTCDATE(), N'Gilbarco Kútoszlop Etalon', N'SN-2023-0777', N'Etalon-500', N'Berendezés', 1, @UgyfelId, @Telephely2, 1),
	(GETUTCDATE(), N'Tokheim Átfolyásmérő Etalon', N'SN-2020-0210', N'FlowRef-20', N'Berendezés', 4, @UgyfelId, @Telephely3, 1),
	(GETUTCDATE(), N'Hectronic Szintmérő Vizsgáló', N'SN-2018-0044', N'LevelCheck-3', N'Berendezés', 3, @UgyfelId, @Telephely3, 1);

DECLARE @Eszkoz1 INT, @Eszkoz2 INT, @Eszkoz3 INT, @Eszkoz4 INT, @Eszkoz5 INT, @Eszkoz6 INT;
SELECT @Eszkoz1 = Id FROM @EszkozTable WHERE Nev = N'MI 3325 Multiservicer';
SELECT @Eszkoz2 = Id FROM @EszkozTable WHERE Nev = N'MI 3125 Industrial';
SELECT @Eszkoz3 = Id FROM @EszkozTable WHERE Nev = N'KEW 3128 Szigetelésmérő';
SELECT @Eszkoz4 = Id FROM @EszkozTable WHERE Nev = N'Gilbarco Kútoszlop Etalon';
SELECT @Eszkoz5 = Id FROM @EszkozTable WHERE Nev = N'Tokheim Átfolyásmérő Etalon';
SELECT @Eszkoz6 = Id FROM @EszkozTable WHERE Nev = N'Hectronic Szintmérő Vizsgáló';

------------------------------------------------------------------------------
-- 2) Kalibrációk - érvényes és lejárt (2+ éven túli) is
------------------------------------------------------------------------------
INSERT INTO Kalibraciok (Letrehozva, EszkozId, Datum, KovetkezoDatum, Bizonyitvany, Elvegzo, Megjegyzes, Sikeres)
VALUES
	-- Érvényes kalibrálások
	(GETUTCDATE(), @Eszkoz1, DATEADD(MONTH, -6, GETUTCDATE()), DATEADD(MONTH, 6, GETUTCDATE()), N'KAL-2025-1001', N'Metrel Kalibráló Labor', N'Teszt adat - érvényes kalibrálás', 1),
	(GETUTCDATE(), @Eszkoz2, DATEADD(MONTH, -3, GETUTCDATE()), DATEADD(MONTH, 9, GETUTCDATE()), N'KAL-2025-1002', N'Metrel Kalibráló Labor', N'Teszt adat - érvényes kalibrálás', 1),
	(GETUTCDATE(), @Eszkoz4, DATEADD(MONTH, -1, GETUTCDATE()), DATEADD(MONTH, 11, GETUTCDATE()), N'KAL-2025-1003', N'Gilbarco Szerviz Kft.', N'Teszt adat - érvényes kalibrálás', 1),

	-- Lejárt kalibrálások (2+ éven túli - következő dátum több mint 2 éve elmúlt)
	(GETUTCDATE(), @Eszkoz3, DATEADD(YEAR, -4, GETUTCDATE()), DATEADD(YEAR, -3, GETUTCDATE()), N'KAL-2021-0093', N'Kyoritsu Kalibráló Központ', N'Teszt adat - lejárt kalibrálás (3+ éve)', 1),
	(GETUTCDATE(), @Eszkoz5, DATEADD(YEAR, -3, GETUTCDATE()), DATEADD(YEAR, -2, DATEADD(MONTH, -6, GETUTCDATE())), N'KAL-2022-0210', N'Tokheim Kalibráló Szerviz', N'Teszt adat - lejárt kalibrálás (2,5+ éve)', 1),
	(GETUTCDATE(), @Eszkoz6, DATEADD(YEAR, -5, GETUTCDATE()), DATEADD(YEAR, -4, GETUTCDATE()), N'KAL-2020-0044', N'Hectronic Kalibráló Labor', N'Teszt adat - lejárt kalibrálás (4+ éve)', 1);

------------------------------------------------------------------------------
-- 3) Felülvizsgálók - különböző jogosultsággal
--    FelulvizsgaloJogosultsag: Segito = 1, Felelos = 2, Ellenor = 3
------------------------------------------------------------------------------
DECLARE @FelulvizsgaloTable TABLE (Id INT, Nev NVARCHAR(200));

INSERT INTO Felulvizsgalok (Letrehozva, Nev, Jogosultsag, Email, Telefon, Megjegyzes, Aktiv, CegId)
OUTPUT inserted.Id, inserted.Nev INTO @FelulvizsgaloTable
VALUES
	(GETUTCDATE(), N'Kovács Béla', 3, N'kovacs.bela@biztovill-teszt.hu', N'+36301111111', N'Teszt adat - ellenőr jogosultságú felülvizsgáló', 1, @CegId),
	(GETUTCDATE(), N'Nagy Éva', 2, N'nagy.eva@biztovill-teszt.hu', N'+36302222222', N'Teszt adat - felelős felülvizsgáló', 1, @CegId),
	(GETUTCDATE(), N'Tóth Gábor', 1, N'toth.gabor@biztovill-teszt.hu', N'+36303333333', N'Teszt adat - segítő felülvizsgáló', 1, @CegId);

DECLARE @Felulvizsgalo1 INT, @Felulvizsgalo2 INT, @Felulvizsgalo3 INT;
SELECT @Felulvizsgalo1 = Id FROM @FelulvizsgaloTable WHERE Nev = N'Kovács Béla';
SELECT @Felulvizsgalo2 = Id FROM @FelulvizsgaloTable WHERE Nev = N'Nagy Éva';
SELECT @Felulvizsgalo3 = Id FROM @FelulvizsgaloTable WHERE Nev = N'Tóth Gábor';

------------------------------------------------------------------------------
-- 4) Felülvizsgálói bizonyítványok (mindenféle - érvényes és lejárt is)
------------------------------------------------------------------------------
INSERT INTO FelulvizsgaloKepzesek
	(Letrehozva, FelulvizsgaloId, KepzesTipusId, BizonyitvanySzam, BizonyitvanyKelte, BizonyitvanyLejarat, Megjegyzes, Aktiv)
VALUES
	(GETUTCDATE(), @Felulvizsgalo1, 4, N'NORV-2024-0011', DATEADD(YEAR, -1, GETUTCDATE()), DATEADD(YEAR, 4, GETUTCDATE()), N'Teszt adat - Norma szerinti Villámvédelmi felülvizsgáló bizonyítvány', 1),
	(GETUTCDATE(), @Felulvizsgalo1, 7, N'VBF-2023-0044', DATEADD(YEAR, -2, GETUTCDATE()), DATEADD(YEAR, 3, GETUTCDATE()), N'Teszt adat - Villamos Biztonsági Felülvizsgáló bizonyítvány', 1),
	(GETUTCDATE(), @Felulvizsgalo2, 1, N'ERV-2022-0077', DATEADD(YEAR, -3, GETUTCDATE()), DATEADD(YEAR, -1, GETUTCDATE()), N'Teszt adat - Érintésvédelmi felülvizsgáló bizonyítvány (lejárt)', 1),
	(GETUTCDATE(), @Felulvizsgalo2, 6, N'ERB-2024-0099', DATEADD(MONTH, -8, GETUTCDATE()), DATEADD(YEAR, 4, GETUTCDATE()), N'Teszt adat - Erősáramú berendezés felülvizsgáló bizonyítvány', 1),
	(GETUTCDATE(), @Felulvizsgalo3, 9, N'VSZ-2021-0033', DATEADD(YEAR, -4, GETUTCDATE()), DATEADD(YEAR, -2, GETUTCDATE()), N'Teszt adat - Villanyszerelő bizonyítvány (lejárt)', 1);

------------------------------------------------------------------------------
-- 5) Munkavédelmi oktatások
------------------------------------------------------------------------------
INSERT INTO MunkavedelmiOktatasok
	(Letrehozva, Megnevezes, Leiras, OktatasDatuma, KovetkezoOktatas, IdoszakHonap, OktatoNeve, Megjegyzes, Aktiv, UgyfelId, TelephelyId, CegId)
VALUES
	(GETUTCDATE(), N'Éves munkavédelmi oktatás - Székhely', N'Kötelező éves munkavédelmi oktatás valamennyi dolgozó részére', DATEADD(MONTH, -4, GETUTCDATE()), DATEADD(MONTH, 8, GETUTCDATE()), 12, N'Kovács Béla', N'Teszt adat', 1, @UgyfelId, @Telephely1, @CegId),
	(GETUTCDATE(), N'Munkavédelmi oktatás - Kecskemét telephely', N'Telephelyi munkavédelmi oktatás', DATEADD(MONTH, -1, GETUTCDATE()), DATEADD(MONTH, 11, GETUTCDATE()), 12, N'Nagy Éva', N'Teszt adat', 1, @UgyfelId, @Telephely2, @CegId);

------------------------------------------------------------------------------
-- 6) Kockázatértékelések
------------------------------------------------------------------------------
INSERT INTO Kockazatertekelesek
	(Letrehozva, Megnevezes, ErtekelesDatuma, KovetkezoFelulvizsgalat, KockazatiSzint, Leiras, Intezkedesek, FelelosNeve, Statusz, Aktiv, UgyfelId, TelephelyId, CegId)
VALUES
	(GETUTCDATE(), N'Kútoszlop üzemi kockázatértékelés - Székhely', DATEADD(MONTH, -5, GETUTCDATE()), DATEADD(MONTH, 7, GETUTCDATE()), 2, N'Teszt adat - üzemi kockázatértékelés töltőállomás területén', N'Rendszeres ellenőrzés, robbanásveszélyes zóna jelölés felülvizsgálata', N'Kovács Béla', 0, 1, @UgyfelId, @Telephely1, @CegId),
	(GETUTCDATE(), N'Tárolótartály kockázatértékelés - Szeged', DATEADD(MONTH, -2, GETUTCDATE()), DATEADD(MONTH, 10, GETUTCDATE()), 3, N'Teszt adat - tárolótartály környezeti kockázatértékelés', N'Tömörségi próba gyakoriságának növelése', N'Tóth Gábor', 1, 1, @UgyfelId, @Telephely3, @CegId);
	-- Statusz: 0 = Folyamatban, 1 = Lezart, 2 = FelulvizsgalatraVar

------------------------------------------------------------------------------
-- 7) Zónatérképek
------------------------------------------------------------------------------
INSERT INTO Zonaterkepek
	(Letrehozva, Megnevezes, ZonaTipus, Leiras, FajlNev, FajlUtvonal, ErvenyessegKezdete, ErvenyessegVege, Aktiv, UgyfelId, TelephelyId, CegId)
VALUES
	(GETUTCDATE(), N'Zónatérkép - Székhely töltőállomás', 1, N'Teszt adat - Zone1 besorolású robbanásveszélyes zóna térkép', N'zonaterkep_szekhely.pdf', N'/uploads/zonaterkepek/zonaterkep_szekhely.pdf', DATEADD(YEAR, -2, GETUTCDATE()), DATEADD(YEAR, 3, GETUTCDATE()), 1, @UgyfelId, @Telephely1, @CegId),
	(GETUTCDATE(), N'Zónatérkép - Szeged tárolótér', 2, N'Teszt adat - Zone2 besorolású zóna térkép', N'zonaterkep_szeged.pdf', N'/uploads/zonaterkepek/zonaterkep_szeged.pdf', DATEADD(YEAR, -1, GETUTCDATE()), DATEADD(YEAR, 4, GETUTCDATE()), 1, @UgyfelId, @Telephely3, @CegId);

------------------------------------------------------------------------------
-- 8) Karbantartások
------------------------------------------------------------------------------
INSERT INTO Karbantartasok
	(Letrehozva, CegId, UgyfelId, TelephelyId, KarbantartasTipusId, Datum, KovetkezoDatum, Leiras, Elvegzo, Statusz, Aktiv)
VALUES
	(GETUTCDATE(), @CegId, @UgyfelId, @Telephely1, 3, DATEADD(MONTH, -5, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()), N'Teszt adat - féléves karbantartás elvégezve', N'Kovács Béla', 2, 1),
	(GETUTCDATE(), @CegId, @UgyfelId, @Telephely2, 4, DATEADD(MONTH, -11, GETUTCDATE()), DATEADD(MONTH, 1, GETUTCDATE()), N'Teszt adat - éves karbantartás közelgő', N'Nagy Éva', 0, 1),
	(GETUTCDATE(), @CegId, @UgyfelId, @Telephely3, 2, GETUTCDATE(), DATEADD(MONTH, 3, GETUTCDATE()), N'Teszt adat - negyedéves karbantartás folyamatban', N'Tóth Gábor', 1, 1);

COMMIT TRANSACTION;

------------------------------------------------------------------------------
-- Ellenőrző lekérdezések (opcionális, futtatás után)
------------------------------------------------------------------------------
-- SELECT * FROM Eszkozok WHERE UgyfelId = 28;
-- SELECT k.*, e.Nev FROM Kalibraciok k JOIN Eszkozok e ON k.EszkozId = e.Id WHERE e.UgyfelId = 28;
-- SELECT * FROM Felulvizsgalok WHERE CegId = 2;
-- SELECT fk.*, f.Nev FROM FelulvizsgaloKepzesek fk JOIN Felulvizsgalok f ON fk.FelulvizsgaloId = f.Id WHERE f.CegId = 2;
-- SELECT * FROM MunkavedelmiOktatasok WHERE UgyfelId = 28;
-- SELECT * FROM Kockazatertekelesek WHERE UgyfelId = 28;
-- SELECT * FROM Zonaterkepek WHERE UgyfelId = 28;
-- SELECT * FROM Karbantartasok WHERE UgyfelId = 28;
