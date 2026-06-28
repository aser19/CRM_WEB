-- SQL szkript a felülvizsgáló fájlfeltöltés mezők hozzáadásához
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

-- 1. AlairasPath mező hozzáadása a Felulvizsgalok táblához
IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Felulvizsgalok]') 
	AND name = 'AlairasPath'
)
BEGIN
	ALTER TABLE [dbo].[Felulvizsgalok]
	ADD [AlairasPath] nvarchar(500) NULL;

	PRINT 'AlairasPath mező hozzáadva a Felulvizsgalok táblához';
END
ELSE
BEGIN
	PRINT 'AlairasPath mező már létezik a Felulvizsgalok táblában';
END
GO

-- 2. BizonyitvanyPath mező hozzáadása a FelulvizsgaloKepzesek táblához
IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[FelulvizsgaloKepzesek]') 
	AND name = 'BizonyitvanyPath'
)
BEGIN
	ALTER TABLE [dbo].[FelulvizsgaloKepzesek]
	ADD [BizonyitvanyPath] nvarchar(500) NULL;

	PRINT 'BizonyitvanyPath mező hozzáadva a FelulvizsgaloKepzesek táblához';
END
ELSE
BEGIN
	PRINT 'BizonyitvanyPath mező már létezik a FelulvizsgaloKepzesek táblában';
END
GO

-- Ellenőrzés: új mezők lekérdezése
SELECT 
	c.name AS ColumnName,
	t.name AS DataType,
	c.max_length AS MaxLength,
	c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[Felulvizsgalok]')
AND c.name = 'AlairasPath';

SELECT 
	c.name AS ColumnName,
	t.name AS DataType,
	c.max_length AS MaxLength,
	c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[FelulvizsgaloKepzesek]')
AND c.name = 'BizonyitvanyPath';

GO

PRINT 'Migráció sikeresen lefutott!';
