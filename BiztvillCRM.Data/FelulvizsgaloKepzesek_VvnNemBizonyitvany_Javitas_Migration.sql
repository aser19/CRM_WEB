-- SQL szkript a "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" FelulvizsgaloKepzesek
-- sorainak (FelulvizsgaloId + BizonyitvanyPath) hiányának diagnosztizálásához és pótlásához.
--
-- Háttér: a VillamNemNormaSzerkeszto.razor / Meresek.razor PDF-generálása a felelős (és opcionálisan
-- a segítő/ellenőr) felülvizsgálóhoz a FelulvizsgaloKepzesek táblában keres egy aktív sort, amelynek
-- KepzesTipusId-ja a "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" KepzesTipus-hoz
-- tartozik, és amelynek BizonyitvanyPath mezője nem üres. Ha ilyen sor nincs, a PDF nem találja a
-- bizonyítványt, és a "A felelős felülvizsgálónak nincs feltöltve..." figyelmeztetés jelenik meg.
--
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

-- 0. Minden KepzesTipus kilistázása, hogy ellenőrizhető legyen, pontosan milyen névvel
--    (esetleges elgépeléssel / extra szóközzel / eltérő ékezettel) szerepel az admin felületen
--    felvitt "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" típus.
SELECT Id, Nev, Aktiv, LEN(Nev) AS NevHossz
FROM [dbo].[KepzesTipusok]
ORDER BY Nev;
GO

DECLARE @VvnNemKepzesTipusNev nvarchar(200) = N'Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány';
DECLARE @VvnNemKepzesTipusId int;

-- 1. A KepzesTipus azonosítójának lekérdezése (ha nincs ilyen típus, létrehozzuk)
SELECT @VvnNemKepzesTipusId = Id
FROM [dbo].[KepzesTipusok]
WHERE Nev = @VvnNemKepzesTipusNev;

IF @VvnNemKepzesTipusId IS NULL
BEGIN
	-- FIGYELEM: a KepzesTipusok tábla Lejar, TovabbkepzesKotelezo és
	-- TovabbkepzesCsakFelulvizsgalonak oszlopai NOT NULL, ezeket kötelező megadni.
	-- Ha a 0. lépés eredménye szerint már létezik hasonló nevű típus (pl. elírással),
	-- NE futtassa ezt a blokkot, hanem inkább az admin felületen javítsa a nevet
	-- pontosan erre: "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány".
	INSERT INTO [dbo].[KepzesTipusok] (Letrehozva, Nev, Aktiv, Lejar, TovabbkepzesKotelezo, TovabbkepzesCsakFelulvizsgalonak)
	VALUES (GETUTCDATE(), @VvnNemKepzesTipusNev, 1, 0, 0, 0);

	SET @VvnNemKepzesTipusId = SCOPE_IDENTITY();

	PRINT N'KepzesTipus "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" létrehozva, Id = ' + CAST(@VvnNemKepzesTipusId AS nvarchar(10));
END
ELSE
BEGIN
	PRINT N'KepzesTipus már létezik, Id = ' + CAST(@VvnNemKepzesTipusId AS nvarchar(10));
END
GO

-- 2. Diagnosztika: mely aktív felülvizsgálóknak nincs ehhez a KepzesTipus-hoz tartozó,
--    BizonyitvanyPath-tel rendelkező aktív FelulvizsgaloKepzesek sora
DECLARE @VvnNemKepzesTipusId2 int;
SELECT @VvnNemKepzesTipusId2 = Id FROM [dbo].[KepzesTipusok]
WHERE Nev = N'Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány';

SELECT
	f.Id AS FelulvizsgaloId,
	f.Nev AS FelulvizsgaloNev,
	f.Aktiv AS FelulvizsgaloAktiv,
	fk.Id AS FelulvizsgaloKepzesekId,
	fk.BizonyitvanyPath,
	fk.Aktiv AS KepzesAktiv
FROM [dbo].[Felulvizsgalok] f
LEFT JOIN [dbo].[FelulvizsgaloKepzesek] fk
	ON fk.FelulvizsgaloId = f.Id
	AND fk.KepzesTipusId = @VvnNemKepzesTipusId2
	AND fk.Aktiv = 1
WHERE f.Aktiv = 1
	AND (fk.Id IS NULL OR fk.BizonyitvanyPath IS NULL OR LTRIM(RTRIM(fk.BizonyitvanyPath)) = '');
GO

-- 2b. Minden felülvizsgálóhoz tartozó, BÁRMILYEN (akár inaktív, akár üres útvonalú) VVN-NEM
--     bizonyítvány sor listázása - ez segít eldönteni, hogy a KepzesTipusId rossz-e,
--     vagy az Aktiv=0, vagy a BizonyitvanyPath üres a hibás soron.
SELECT
	f.Id AS FelulvizsgaloId,
	f.Nev AS FelulvizsgaloNev,
	fk.Id AS FelulvizsgaloKepzesekId,
	fk.KepzesTipusId,
	kt.Nev AS KepzesTipusNev,
	fk.BizonyitvanyPath,
	fk.Aktiv AS KepzesAktiv
FROM [dbo].[FelulvizsgaloKepzesek] fk
INNER JOIN [dbo].[Felulvizsgalok] f ON f.Id = fk.FelulvizsgaloId
LEFT JOIN [dbo].[KepzesTipusok] kt ON kt.Id = fk.KepzesTipusId
WHERE kt.Nev LIKE N'%Nem norma szerinti%'
   OR kt.Nev LIKE N'%vill%nem norma%';
GO

-- 3. Pótló INSERT sablon: minden olyan aktív felülvizsgálóhoz, akinek hiányzik a bizonyítvány sora,
--    létrehoz egy hiányzó FelulvizsgaloKepzesek rekordot (BizonyitvanyPath/BizonyitvanySzam/kelte NULL marad,
--    ezeket kézzel vagy feltöltéssel kell pótolni az alkalmazásban).
--
--    FIGYELEM: csak akkor futtassa, ha valóban hiányzó SOR-t kell pótolni (lásd a 2. lépés eredményét),
--    és ne akkor, ha a sor megvan, csak a BizonyitvanyPath hiányzik (azt UPDATE-tel kell pótolni, lásd 4.).
/*
DECLARE @VvnNemKepzesTipusId3 int;
SELECT @VvnNemKepzesTipusId3 = Id FROM [dbo].[KepzesTipusok]
WHERE Nev = N'Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány';

INSERT INTO [dbo].[FelulvizsgaloKepzesek]
	(Letrehozva, FelulvizsgaloId, KepzesTipusId, BizonyitvanySzam, BizonyitvanyKelte, BizonyitvanyLejarat, BizonyitvanyPath, Megjegyzes, Aktiv)
SELECT
	GETUTCDATE(), f.Id, @VvnNemKepzesTipusId3, NULL, NULL, NULL, NULL, N'Automatikusan pótolt sor - töltse fel a bizonyítványt!', 1
FROM [dbo].[Felulvizsgalok] f
WHERE f.Aktiv = 1
	AND NOT EXISTS (
		SELECT 1 FROM [dbo].[FelulvizsgaloKepzesek] fk
		WHERE fk.FelulvizsgaloId = f.Id
		  AND fk.KepzesTipusId = @VvnNemKepzesTipusId3
	);
*/
GO

-- 4. Ha a sor megvan, csak a BizonyitvanyPath hiányzik: itt adja meg konkrétan, melyik
--    felülvizsgálóhoz (Id alapján) milyen fájl elérési utat (relatív az Uploads mappához) kell beállítani.
--    Töltse ki a @FelulvizsgaloId és @BizonyitvanyPath értékeket a 2. lépés eredménye alapján, majd futtassa.
/*
DECLARE @FelulvizsgaloId int = 0; -- <-- Ide írja be a Felulvizsgalo Id-ját
DECLARE @BizonyitvanyPath nvarchar(500) = N''; -- <-- Ide írja be a fájl elérési útját
DECLARE @VvnNemKepzesTipusId4 int;
SELECT @VvnNemKepzesTipusId4 = Id FROM [dbo].[KepzesTipusok]
WHERE Nev = N'Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány';

UPDATE [dbo].[FelulvizsgaloKepzesek]
SET BizonyitvanyPath = @BizonyitvanyPath,
	Modositva = GETUTCDATE()
WHERE FelulvizsgaloId = @FelulvizsgaloId
	AND KepzesTipusId = @VvnNemKepzesTipusId4
	AND Aktiv = 1;
*/
GO
