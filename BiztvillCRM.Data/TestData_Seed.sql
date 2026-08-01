/*
	============================================================================
	BiztvillCRM - Teszt adatok seed script
	============================================================================
	Cél: 1 teszt felhasználó, néhány telephely, munkaszámok, 5-6 különböző
	mérés típus és 5-6 különböző hitelesítés típus, valamint néhány
	munkaszám-hitelesítés összerendelés generálása a meglévő teszt céghez.

	Feltételezett meglévő törzsadatok (a scriptben hivatkozva):
	  - Ceg.Id        = 2  ("Biztovill CRM Demo")
	  - Ugyfel.Id     = 28 ("Árvai Zsolt")   -> ehhez CegId = 2
	  - Hatosag.Id    = 3  ("Budapest Főváros Kormányhivatala")
	  - MeresTipus.Id: 2 (Időszakos VBF), 4 (Norma szerinti Villámvédelem),
					   5 (Egy mérés), 6 (Áramvédő kapcsoló),
					   8 (Hurokimpedancia mérési jegyzőkönyv),
					   9 (Szigetelés ellenállás mérés)
	  - EszkozTipus.Id: 1 (Kútoszlop - Hitelesítés átfolyás),
						2 (Tartály - Szintmérő hitelesítés I.+II. fázis),
						3 (Átfolyásmérő hitelesítés),
						5 (Használati Etalon hitelesítés),
						6 (Tartály - Mérőléc hitelesítés),
						11 (Tartály - Hitelesítés)

	Megjegyzés: a Meresek tábla nem tartalmaz MunkaszamId oszlopot, ezért a
	munkaszám-összerendelés csak a Hitelesitesek táblánál valósul meg
	(kb. a rekordok felénél, ahogy kérve volt).

	A teszt felhasználó belépési adatai:
	  Email / UserName : teszt.ugyintezo@biztovill.hu
	  Jelszó           : Teszt1234!
	============================================================================
*/

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @CegId INT = 2;
DECLARE @UgyfelId INT = 28;
DECLARE @HatosagId INT = 3;

------------------------------------------------------------------------------
-- 1) Teszt felhasználó (AspNetUsers) + Felhasznalo kiegészítő mezők
------------------------------------------------------------------------------
DECLARE @FelhasznaloId NVARCHAR(450) = NEWID();
DECLARE @FelhasznaloRoleId NVARCHAR(450);
SELECT @FelhasznaloRoleId = Id FROM AspNetRoles WHERE Name = 'Felhasznalo';

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'teszt.ugyintezo@biztovill.hu')
BEGIN
	INSERT INTO AspNetUsers
		(Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
		 PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
		 TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
		 Letrehozva, Aktiv, Nev, Beosztas, Telefon, CegId)
	VALUES
		(@FelhasznaloId, 'teszt.ugyintezo@biztovill.hu', 'TESZT.UGYINTEZO@BIZTOVILL.HU',
		 'teszt.ugyintezo@biztovill.hu', 'TESZT.UGYINTEZO@BIZTOVILL.HU', 1,
		 'AQAAAAEAAYagAAAAEO/ynctt7n9lwnTVI/+9rV93fUd2uUCgxu+0RiMlox9/EIyFqDjOijkEJd6COqMgnQ==',
		 NEWID(), NEWID(), NULL, 0,
		 0, 1, 0,
		 GETUTCDATE(), 1, N'Teszt Ügyintéző', N'Ügyintéző', N'+36301234567', @CegId);

	IF @FelhasznaloRoleId IS NOT NULL
	BEGIN
		INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@FelhasznaloId, @FelhasznaloRoleId);
	END

	INSERT INTO FelhasznaloCegek (FelhasznaloId, CegId, Szerep, Hozzaadva)
	VALUES (@FelhasznaloId, @CegId, N'Felhasznalo', GETUTCDATE());
END
ELSE
BEGIN
	SELECT @FelhasznaloId = Id FROM AspNetUsers WHERE Email = 'teszt.ugyintezo@biztovill.hu';
END

------------------------------------------------------------------------------
-- 2) Néhány telephely a teszt ügyfélhez
------------------------------------------------------------------------------
DECLARE @TelephelyTable TABLE (Id INT, Nev NVARCHAR(200));

INSERT INTO Telephelyek (Letrehozva, Nev, Cim, UgyfelId, Kapcsolattarto, Telefon, Email, Aktiv)
OUTPUT inserted.Id, inserted.Nev INTO @TelephelyTable
VALUES
	(GETUTCDATE(), N'Árvai Zsolt - Székhely', N'1111 Budapest, Teszt utca 1.', @UgyfelId, N'Árvai Zsolt', N'+36301112222', N'arvai.zsolt@teszt.hu', 1),
	(GETUTCDATE(), N'Árvai Zsolt - Telephely Kecskemét', N'6000 Kecskemét, Minta utca 2.', @UgyfelId, N'Kovács Béla', N'+36302223333', N'kecskemet@teszt.hu', 1),
	(GETUTCDATE(), N'Árvai Zsolt - Telephely Szeged', N'6720 Szeged, Példa út 3.', @UgyfelId, N'Nagy Éva', N'+36303334444', N'szeged@teszt.hu', 1);

DECLARE @Telephely1 INT, @Telephely2 INT, @Telephely3 INT;
SELECT @Telephely1 = Id FROM @TelephelyTable WHERE Nev = N'Árvai Zsolt - Székhely';
SELECT @Telephely2 = Id FROM @TelephelyTable WHERE Nev = N'Árvai Zsolt - Telephely Kecskemét';
SELECT @Telephely3 = Id FROM @TelephelyTable WHERE Nev = N'Árvai Zsolt - Telephely Szeged';

------------------------------------------------------------------------------
-- 3) Munkaszámok a teszt céghez
------------------------------------------------------------------------------
DECLARE @MunkaszamTable TABLE (Id INT, Szam NVARCHAR(50));

INSERT INTO Munkaszamok (CegId, Letrehozva, Szam, Megnevezes, Aktiv)
OUTPUT inserted.Id, inserted.Szam INTO @MunkaszamTable
VALUES
	(@CegId, GETUTCDATE(), N'MSZ-2026-101', N'Teszt munkaszám - Éves felülvizsgálat', 1),
	(@CegId, GETUTCDATE(), N'MSZ-2026-102', N'Teszt munkaszám - Hitelesítési kampány', 1);

DECLARE @Munkaszam1 INT, @Munkaszam2 INT;
SELECT @Munkaszam1 = Id FROM @MunkaszamTable WHERE Szam = N'MSZ-2026-101';
SELECT @Munkaszam2 = Id FROM @MunkaszamTable WHERE Szam = N'MSZ-2026-102';

------------------------------------------------------------------------------
-- 4) Mérések - 6 különböző MeresTipus
------------------------------------------------------------------------------
INSERT INTO Meresek (Letrehozva, UgyfelId, TelephelyId, MeresTipusId, Datum, KovetkezoDatum, Eredmeny, MeresStatusz, Megjegyzes, Aktiv)
VALUES
	(GETUTCDATE(), @UgyfelId, @Telephely1, 2, '2026-01-15', '2027-01-15', N'Megfelelt', 0, N'Teszt adat - Időszakos VBF', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely1, 4, '2026-02-10', '2027-02-10', N'Megfelelt', 0, N'Teszt adat - Norma szerinti Villámvédelem', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely2, 5, '2026-03-05', NULL, N'Megfelelt', 0, N'Teszt adat - Egy mérés', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely2, 6, '2026-04-20', '2027-04-20', N'Megfelelt', 0, N'Teszt adat - Áramvédő kapcsoló', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely3, 8, '2026-05-12', NULL, N'Megfelelt', 0, N'Teszt adat - Hurokimpedancia mérési jegyzőkönyv', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely3, 9, '2026-06-18', '2027-06-18', N'Megfelelt', 0, N'Teszt adat - Szigetelés ellenállás mérés', 1);

------------------------------------------------------------------------------
-- 5) Hitelesítések - 6 különböző EszkozTipus, felénél munkaszám hozzárendelve
------------------------------------------------------------------------------
INSERT INTO Hitelesitesek
	(Letrehozva, UgyfelId, TelephelyId, EszkozTipusId, HatosagId, MunkaszamId,
	 Darabszam, Datum, LejaratDatum, HitelesitesStatusz, Megjegyzes, Aktiv)
VALUES
	(GETUTCDATE(), @UgyfelId, @Telephely1, 1, @HatosagId, @Munkaszam1, 2, '2026-01-20', '2028-01-20', 0, N'Teszt adat - Kútoszlop hitelesítés (átfolyás)', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely1, 2, @HatosagId, @Munkaszam1, 1, '2026-02-15', '2028-02-15', 0, N'Teszt adat - Tartály szintmérő hitelesítés', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely2, 3, @HatosagId, NULL, 1, '2026-03-10', '2027-03-10', 0, N'Teszt adat - Átfolyásmérő hitelesítés', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely2, 5, @HatosagId, NULL, 3, '2026-04-25', '2027-04-25', 0, N'Teszt adat - Használati Etalon hitelesítés', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely3, 6, @HatosagId, @Munkaszam2, 1, '2026-05-30', '2028-05-30', 0, N'Teszt adat - Tartály mérőléc hitelesítés', 1),
	(GETUTCDATE(), @UgyfelId, @Telephely3, 11, @HatosagId, NULL, 1, '2026-06-22', '2027-06-22', 0, N'Teszt adat - Tartály hitelesítés', 1);

COMMIT TRANSACTION;

------------------------------------------------------------------------------
-- Ellenőrző lekérdezések (opcionális, futtatás után)
------------------------------------------------------------------------------
-- SELECT * FROM AspNetUsers WHERE Email = 'teszt.ugyintezo@biztovill.hu';
-- SELECT * FROM Telephelyek WHERE UgyfelId = 28;
-- SELECT * FROM Munkaszamok WHERE CegId = 2;
-- SELECT * FROM Meresek WHERE UgyfelId = 28 ORDER BY Id DESC;
-- SELECT * FROM Hitelesitesek WHERE UgyfelId = 28 ORDER BY Id DESC;
