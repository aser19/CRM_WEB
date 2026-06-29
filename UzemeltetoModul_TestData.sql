-- =============================================
-- Üzemeltető modul - Teszt adatok
-- Létrehozva: 2026.06.29
-- Leírás: Minta sablonok és adatok az Üzemeltető modulhoz
-- =============================================

-- Előfeltételek ellenőrzése
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoSablonok]'))
BEGIN
	PRINT 'HIBA: A UzemeltetoSablonok tábla nem létezik! Először futtasd le a UzemeltetoModul_Migration.sql scriptet!';
	RETURN;
END

-- Változók deklarálása
DECLARE @CegId INT;
DECLARE @AdminUserId NVARCHAR(450);
DECLARE @UzemeltetoUserId NVARCHAR(450);
DECLARE @SablonId1 INT;
DECLARE @SablonId2 INT;

-- =============================================
-- 1. Első cég és admin felhasználó keresése
-- =============================================
SELECT TOP 1 @CegId = Id FROM Cegek WHERE Aktiv = 1 ORDER BY Id;
SELECT TOP 1 @AdminUserId = Id FROM AspNetUsers ORDER BY Id;

IF @CegId IS NULL OR @AdminUserId IS NULL
BEGIN
	PRINT 'HIBA: Nem található aktív cég vagy felhasználó!';
	RETURN;
END

PRINT 'Teszt adatok létrehozása...';
PRINT 'CegId: ' + CAST(@CegId AS NVARCHAR(10));
PRINT 'AdminUserId: ' + @AdminUserId;
PRINT '';

BEGIN TRANSACTION;

-- =============================================
-- 2. Sablon #1: Benzinkút ellenőrzések
-- =============================================
INSERT INTO UzemeltetoSablonok 
	(Letrehozva, Nev, Leiras, JogszabalyiHivatkozas, EllenorzesiIdoszakHonap, Aktiv, CegId, LetrehozoFelhasznaloId)
VALUES 
	(GETDATE(), 
	 'Benzinkút ellenőrzések', 
	 'Üzemanyagtöltő állomások kötelező ellenőrzései és hitelesítései',
	 '54/2004. (XII. 23.) GKM rendelet',
	 12,
	 1,
	 @CegId,
	 @AdminUserId);

SET @SablonId1 = SCOPE_IDENTITY();
PRINT 'Sablon létrehozva: Benzinkút ellenőrzések (ID: ' + CAST(@SablonId1 AS NVARCHAR(10)) + ')';

-- Benzinkút sablon mezői
INSERT INTO UzemeltetoSablonMezok 
	(UzemeltetoSablonId, MezoNev, MezoTipus, Kotelezo, Sorrend, Sugo)
VALUES 
	(@SablonId1, 'Eszköz típusa', 'Text', 1, 1, 'Pl. mérőóra, pumpa, tartály'),
	(@SablonId1, 'Gyártási szám', 'Text', 1, 2, 'Az eszköz egyedi azonosítója'),
	(@SablonId1, 'Ellenőrzés dátuma', 'Datum', 1, 3, 'Az ellenőrzés elvégzésének dátuma'),
	(@SablonId1, 'Hitelesítés érvényes', 'Datum', 1, 4, 'A hitelesítés lejárati dátuma'),
	(@SablonId1, 'Megfelelt', 'Boolean', 1, 5, 'Az eszköz megfelelt-e az ellenőrzésen'),
	(@SablonId1, 'Hitelesítő szervezet', 'Text', 1, 6, 'A hitelesítést végző szervezet neve'),
	(@SablonId1, 'Hatósági engedély száma', 'Text', 0, 7, 'Hatósági engedély azonosítója');

PRINT '  - 7 mező hozzáadva';

-- =============================================
-- 3. Sablon #2: Tűzvédelmi ellenőrzések
-- =============================================
INSERT INTO UzemeltetoSablonok 
	(Letrehozva, Nev, Leiras, JogszabalyiHivatkozas, EllenorzesiIdoszakHonap, Aktiv, CegId, LetrehozoFelhasznaloId)
VALUES 
	(GETDATE(), 
	 'Tűzvédelmi ellenőrzések', 
	 'Tűzoltó készülékek és egyéb tűzvédelmi eszközök rendszeres ellenőrzése',
	 '54/2014. (XII. 5.) BM rendelet',
	 6,
	 1,
	 @CegId,
	 @AdminUserId);

SET @SablonId2 = SCOPE_IDENTITY();
PRINT 'Sablon létrehozva: Tűzvédelmi ellenőrzések (ID: ' + CAST(@SablonId2 AS NVARCHAR(10)) + ')';

-- Tűzvédelmi sablon mezői
INSERT INTO UzemeltetoSablonMezok 
	(UzemeltetoSablonId, MezoNev, MezoTipus, Kotelezo, Sorrend, Sugo)
VALUES 
	(@SablonId2, 'Eszköz megnevezése', 'Text', 1, 1, 'Pl. poroltó, CO2 oltó, haboltó'),
	(@SablonId2, 'Elhelyezés helye', 'Text', 1, 2, 'Pontos helymeghatározás'),
	(@SablonId2, 'Ellenőrzés dátuma', 'Datum', 1, 3, 'Az ellenőrzés elvégzésének dátuma'),
	(@SablonId2, 'Következő ellenőrzés', 'Datum', 1, 4, 'Következő kötelező ellenőrzés dátuma'),
	(@SablonId2, 'Állapot megfelelő', 'Boolean', 1, 5, 'Az eszköz állapota megfelelő-e'),
	(@SablonId2, 'Nyomás (bar)', 'Szam', 0, 6, 'A tűzoltó készülék nyomása'),
	(@SablonId2, 'Hiányosságok', 'Text', 0, 7, 'Észlelt hiányosságok leírása');

PRINT '  - 7 mező hozzáadva';

-- =============================================
-- 4. Sablon #3: Villamos berendezések
-- =============================================
INSERT INTO UzemeltetoSablonok 
	(Letrehozva, Nev, Leiras, JogszabalyiHivatkozas, EllenorzesiIdoszakHonap, Aktiv, CegId, LetrehozoFelhasznaloId)
VALUES 
	(GETDATE(), 
	 'Villamos berendezések felülvizsgálata', 
	 'Érintésvédelmi és túláramvédelmi vizsgálatok',
	 '4/2012. (II. 24.) NGM rendelet',
	 24,
	 1,
	 @CegId,
	 @AdminUserId);

DECLARE @SablonId3 INT = SCOPE_IDENTITY();
PRINT 'Sablon létrehozva: Villamos berendezések felülvizsgálata (ID: ' + CAST(@SablonId3 AS NVARCHAR(10)) + ')';

-- Villamos sablon mezői
INSERT INTO UzemeltetoSablonMezok 
	(UzemeltetoSablonId, MezoNev, MezoTipus, Kotelezo, Sorrend, Sugo)
VALUES 
	(@SablonId3, 'Berendezés megnevezése', 'Text', 1, 1, 'Villamos berendezés típusa'),
	(@SablonId3, 'Felülvizsgálat dátuma', 'Datum', 1, 2, 'A felülvizsgálat időpontja'),
	(@SablonId3, 'Felülvizsgáló szervezet', 'Text', 1, 3, 'Akkreditált szervezet neve'),
	(@SablonId3, 'Jegyzőkönyv száma', 'Text', 1, 4, 'Felülvizsgálati jegyzőkönyv azonosítója'),
	(@SablonId3, 'Érintésvédelem megfelelő', 'Boolean', 1, 5, 'Érintésvédelem eredménye'),
	(@SablonId3, 'Túláramvédelem megfelelő', 'Boolean', 1, 6, 'Túláramvédelem eredménye'),
	(@SablonId3, 'Megjegyzés', 'Text', 0, 7, 'Egyéb észrevételek');

PRINT '  - 7 mező hozzáadva';

-- =============================================
-- 5. Minta adat létrehozása (ha van üzemeltető)
-- =============================================
-- Üzemeltető felhasználó keresése
SELECT TOP 1 @UzemeltetoUserId = Id 
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.[Name] = 'Uzemelteto';

IF @UzemeltetoUserId IS NOT NULL
BEGIN
	-- Minta adat a benzinkút sablonhoz
	INSERT INTO UzemeltetoAdatok 
		(Letrehozva, UzemeltetoSablonId, RogzitesDatum, KovetkezoEsedekesseg, 
		 Statusz, CegId, RogzitoFelhasznaloId, MezoErtekekJson, Aktiv)
	VALUES 
		(GETDATE(), 
		 @SablonId1, 
		 GETDATE(), 
		 DATEADD(MONTH, 12, GETDATE()),
		 'Elvégezve',
		 @CegId,
		 @UzemeltetoUserId,
		 '{"1":"Mérőóra","2":"ABC-12345","3":"' + CONVERT(VARCHAR(10), GETDATE(), 23) + '","4":"' + CONVERT(VARCHAR(10), DATEADD(MONTH, 12, GETDATE()), 23) + '","5":"True","6":"MKEH","7":"HE-2026-001"}',
		 1);

	PRINT '';
	PRINT 'Minta adat létrehozva az Üzemeltető felhasználóhoz.';
END
ELSE
BEGIN
	PRINT '';
	PRINT 'Üzemeltető felhasználó nem található, minta adat nem került létrehozásra.';
END

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'Teszt adatok sikeresen létrehozva!';
PRINT '========================================';
PRINT '';
PRINT 'Létrehozott sablonok:';
PRINT '  1. Benzinkút ellenőrzések (7 mező, 12 hónapos ciklus)';
PRINT '  2. Tűzvédelmi ellenőrzések (7 mező, 6 hónapos ciklus)';
PRINT '  3. Villamos berendezések felülvizsgálata (7 mező, 24 hónapos ciklus)';
PRINT '';
PRINT 'Most már használhatod az Üzemeltető modult!';
PRINT '';
PRINT 'Admin felület: /admin/uzemelteto-sablonok';
PRINT 'Üzemeltető felület: /uzemelteto/adatok';
PRINT '';

GO
