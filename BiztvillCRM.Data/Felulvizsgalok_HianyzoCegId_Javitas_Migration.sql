-- SQL szkript a Felulvizsgalok tábla hiányos (NULL) CegId és Letrehozva adatainak javítására
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!
-- Idempotens: többszöri futtatás esetén sem okoz hibát / duplikációt.

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

-- 1. Hiányzó CegId pótlása azoknál a felülvizsgálóknál, ahol NULL
--    (a hiányos sorok az adott céghez tartoznak, amire a többi, azonos adatú sor is mutat)
IF EXISTS (
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[Felulvizsgalok]')
	AND name = 'CegId'
)
BEGIN
	UPDATE f
	SET f.CegId = helyes.CegId
	FROM [dbo].[Felulvizsgalok] f
	CROSS APPLY (
		SELECT TOP (1) f2.CegId
		FROM [dbo].[Felulvizsgalok] f2
		WHERE f2.Email = f.Email
		  AND f2.CegId IS NOT NULL
		ORDER BY f2.Id
	) helyes
	WHERE f.CegId IS NULL;

	PRINT 'Felulvizsgalok.CegId hiányzó értékei pótolva az Email alapján megegyező sorokból.';
END
GO

-- 2. Ha az Email alapján nem volt megfeleltethető CegId (nincs egyező pár), állítsuk az 1-es (alapértelmezett) cégre,
--    hogy ne maradjon NULL érték a táblában.
IF EXISTS (
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[Felulvizsgalok]')
	AND name = 'CegId'
)
BEGIN
	UPDATE [dbo].[Felulvizsgalok]
	SET CegId = 1
	WHERE CegId IS NULL;

	PRINT 'Felulvizsgalok.CegId még mindig hiányzó értékei alapértelmezett (1) cégre állítva.';
END
GO

-- 3. Hiányzó Letrehozva mező pótlása az aktuális időponttal, ha NULL maradt
IF EXISTS (
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[Felulvizsgalok]')
	AND name = 'Letrehozva'
)
BEGIN
	UPDATE [dbo].[Felulvizsgalok]
	SET Letrehozva = GETDATE()
	WHERE Letrehozva IS NULL;

	PRINT 'Felulvizsgalok.Letrehozva hiányzó értékei az aktuális dátumra állítva.';
END
GO
