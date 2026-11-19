-- SQL szkript a céglogó fájlfeltöltés mező hozzáadásához
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!

USE [CRM]; -- Az éles adatbázis neve (biztovill.database.windows.net szerver)
GO

-- LogoPath mező hozzáadása a Cegek táblához
IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Cegek]') 
	AND name = 'LogoPath'
)
BEGIN
	ALTER TABLE [dbo].[Cegek]
	ADD [LogoPath] nvarchar(500) NULL;

	PRINT 'LogoPath mező hozzáadva a Cegek táblához';
END
ELSE
BEGIN
	PRINT 'LogoPath mező már létezik a Cegek táblában';
END
GO

-- Ellenőrzés
SELECT c.name AS OszlopNev, t.name AS Tipus, c.max_length, c.is_nullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[Cegek]')
AND c.name = 'LogoPath';
GO
