-- SQL szkript a KepzesTipusok tábla "Kod" oszlopának hozzáadásához,
-- és a "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" típus
-- stabil "VVN_NEM_BIZONYITVANY" kóddal való megjelöléséhez.
--
-- Háttér: a VillamNemNormaSzerkeszto.razor / Meresek.razor kódja korábban a
-- KepzesTipusok.Nev szöveges mezőre hivatkozva kereste meg ezt a típust. Mivel
-- a Nev mező szabadon szerkeszthető az admin felületen, és több, hasonló nevű
-- (de nem azonos) típus is létrejöhet (pl. Id=2 "Nem norma szerinti
-- Villámvédelmi Felülvizsgáló" és Id=12 "Nem norma szerinti villámvédelmi
-- felülvizsgálói bizonyítvány"), ez a megoldás törékeny volt. A Kod mező egy
-- stabil, nem admin által szerkesztett azonosítót ad, amelyre a program
-- logikája biztonságosan hivatkozhat a Nev helyett/mellett.
--
-- Idempotens: többször is futtatható, nem okoz hibát, ha már le lett futtatva.
--
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

-- 1. "Kod" oszlop hozzáadása, ha még nem létezik
IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[KepzesTipusok]') AND name = 'Kod'
)
BEGIN
	ALTER TABLE [dbo].[KepzesTipusok] ADD [Kod] nvarchar(50) NULL;
	PRINT N'Kod oszlop hozzáadva a KepzesTipusok táblához.';
END
ELSE
BEGIN
	PRINT N'Kod oszlop már létezik a KepzesTipusok táblában.';
END
GO

-- 2. A "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" típus
--    megjelölése a stabil "VVN_NEM_BIZONYITVANY" kóddal.
--    Előnyben részesítjük a pontosan egyező nevű, aktív rekordot; ha ilyen
--    nincs, a "nem norma" részletet tartalmazó, legmagasabb Id-jű (vélhetően
--    legfrissebb) rekordot jelöljük meg, amelyiknek még nincs Kod-ja.
DECLARE @VvnNemKepzesTipusId int;

SELECT TOP 1 @VvnNemKepzesTipusId = Id
FROM [dbo].[KepzesTipusok]
WHERE Nev = N'Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány'
ORDER BY Aktiv DESC, Id DESC;

IF @VvnNemKepzesTipusId IS NULL
BEGIN
	SELECT TOP 1 @VvnNemKepzesTipusId = Id
	FROM [dbo].[KepzesTipusok]
	WHERE Nev LIKE N'%nem norma%villámvédelmi%'
	   OR Nev LIKE N'%villámvédelmi%nem norma%'
	ORDER BY Aktiv DESC, Id DESC;
END

IF @VvnNemKepzesTipusId IS NOT NULL
BEGIN
	UPDATE [dbo].[KepzesTipusok]
	SET Kod = N'VVN_NEM_BIZONYITVANY'
	WHERE Id = @VvnNemKepzesTipusId
	  AND (Kod IS NULL OR Kod <> N'VVN_NEM_BIZONYITVANY');

	PRINT N'KepzesTipus Kod beállítva: Id = ' + CAST(@VvnNemKepzesTipusId AS nvarchar(10)) + N', Kod = VVN_NEM_BIZONYITVANY';
END
ELSE
BEGIN
	PRINT N'FIGYELEM: nem található "nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" nevű KepzesTipus - a Kod nem lett beállítva!';
END
GO

-- 3. Ellenőrzés: minden KepzesTipus és a hozzárendelt Kod kilistázása
SELECT Id, Nev, Kod, Aktiv
FROM [dbo].[KepzesTipusok]
ORDER BY Nev;
GO
