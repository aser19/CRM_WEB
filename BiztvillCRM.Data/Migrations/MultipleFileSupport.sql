-- =====================================================
-- BIZTOVILL CRM - MULTIPLE FILE SUPPORT MIGRATION
-- Dátum: 2024-12-15
-- Verzió: 1.0
-- =====================================================
-- Leírás: 
-- Hozzáadja a MunkalapPaths és BizonyitvanyPaths oszlopokat
-- a Hitelesitesek táblához többfájl támogatás céljából.
-- Visszafele kompatibilis a meglévő MunkalapPath és 
-- BizonyitvanyPath mezőkkel.
-- =====================================================

USE [BiztvillCRM]  -- ← MÓDOSÍTSD az adatbázis nevére!
GO

BEGIN TRANSACTION;
GO

PRINT '========================================';
PRINT 'Multiple File Support Migration START';
PRINT '========================================';
GO

-- =====================================================
-- 1. Új oszlopok hozzáadása
-- =====================================================

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') 
	AND name = 'MunkalapPaths'
)
BEGIN
	ALTER TABLE [dbo].[Hitelesitesek]
	ADD [MunkalapPaths] NVARCHAR(MAX) NULL;

	PRINT '✓ Hitelesitesek.MunkalapPaths oszlop hozzáadva.';
END
ELSE
BEGIN
	PRINT 'ℹ Hitelesitesek.MunkalapPaths oszlop már létezik - átugorva.';
END
GO

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') 
	AND name = 'BizonyitvanyPaths'
)
BEGIN
	ALTER TABLE [dbo].[Hitelesitesek]
	ADD [BizonyitvanyPaths] NVARCHAR(MAX) NULL;

	PRINT '✓ Hitelesitesek.BizonyitvanyPaths oszlop hozzáadva.';
END
ELSE
BEGIN
	PRINT 'ℹ Hitelesitesek.BizonyitvanyPaths oszlop már létezik - átugorva.';
END
GO

-- =====================================================
-- 2. Adatmigráció: Munkalap
-- =====================================================

DECLARE @migratedMunkalap INT;

UPDATE [dbo].[Hitelesitesek]
SET [MunkalapPaths] = '["' + REPLACE([MunkalapPath], '"', '\"') + '"]'
WHERE [MunkalapPath] IS NOT NULL 
  AND [MunkalapPath] <> ''
  AND ([MunkalapPaths] IS NULL OR [MunkalapPaths] = '');

SET @migratedMunkalap = @@ROWCOUNT;

PRINT '✓ ' + CAST(@migratedMunkalap AS NVARCHAR(10)) + ' MunkalapPath érték migrálva JSON formátumba.';
GO

-- =====================================================
-- 3. Adatmigráció: Bizonyítvány
-- =====================================================

DECLARE @migratedBizonyitvany INT;

UPDATE [dbo].[Hitelesitesek]
SET [BizonyitvanyPaths] = '["' + REPLACE([BizonyitvanyPath], '"', '\"') + '"]'
WHERE [BizonyitvanyPath] IS NOT NULL 
  AND [BizonyitvanyPath] <> ''
  AND ([BizonyitvanyPaths] IS NULL OR [BizonyitvanyPaths] = '');

SET @migratedBizonyitvany = @@ROWCOUNT;

PRINT '✓ ' + CAST(@migratedBizonyitvany AS NVARCHAR(10)) + ' BizonyitvanyPath érték migrálva JSON formátumba.';
GO

-- =====================================================
-- 4. Migration history bejegyzés (opcionális - csak ha van EF Core migrations)
-- =====================================================

-- Ellenőrizzük, hogy létezik-e a migrations history tábla
IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
BEGIN
	IF NOT EXISTS (
		SELECT * FROM [__EFMigrationsHistory] 
		WHERE [MigrationId] = N'20241215000000_AddMultipleFileSupportForDocuments'
	)
	BEGIN
		INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
		VALUES (N'20241215000000_AddMultipleFileSupportForDocuments', N'8.0.0');

		PRINT '✓ Migration history frissítve.';
	END
	ELSE
	BEGIN
		PRINT 'ℹ Migration history már tartalmazza ezt a bejegyzést.';
	END
END
ELSE
BEGIN
	PRINT 'ℹ __EFMigrationsHistory tábla nem található - átugorva (nem probléma).';
END
GO

COMMIT TRANSACTION;
GO

PRINT '========================================';
PRINT '✅ Migration sikeresen befejezve!';
PRINT '========================================';
GO

-- =====================================================
-- ELLENŐRZÉS (opcionális - futtatható külön a migration után):
-- =====================================================
/*
SELECT TOP 10
	Id,
	Datum,
	MunkalapPath AS 'Régi_Munkalap',
	MunkalapPaths AS 'Új_Munkalap_JSON',
	BizonyitvanyPath AS 'Régi_Bizonyitvany',
	BizonyitvanyPaths AS 'Új_Bizonyitvany_JSON'
FROM [dbo].[Hitelesitesek]
WHERE MunkalapPaths IS NOT NULL OR BizonyitvanyPaths IS NOT NULL
ORDER BY Id DESC;

-- Összesítés:
SELECT 
	COUNT(*) AS 'Összes_Hitelesítés',
	SUM(CASE WHEN MunkalapPaths IS NOT NULL THEN 1 ELSE 0 END) AS 'Munkalap_JSON',
	SUM(CASE WHEN BizonyitvanyPaths IS NOT NULL THEN 1 ELSE 0 END) AS 'Bizonyítvány_JSON'
FROM [dbo].[Hitelesitesek];
*/

-- =====================================================
-- ROLLBACK SCRIPT (csak ha visszaalakítod a kódot!)
-- =====================================================
/*
BEGIN TRANSACTION;

ALTER TABLE [dbo].[Hitelesitesek] DROP COLUMN IF EXISTS [MunkalapPaths];
ALTER TABLE [dbo].[Hitelesitesek] DROP COLUMN IF EXISTS [BizonyitvanyPaths];

-- Migration history törlése (csak ha létezik a tábla)
IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
BEGIN
    DELETE FROM [__EFMigrationsHistory] 
    WHERE [MigrationId] = N'20241215000000_AddMultipleFileSupportForDocuments';
END

COMMIT TRANSACTION;

PRINT 'Rollback befejezve.';
*/

-- =====================================================
-- MEGJEGYZÉSEK:
-- 
-- ✅ BIZTONSÁGOS:
-- - A migration visszafele kompatibilis
-- - A régi MunkalapPath és BizonyitvanyPath mezők MEGMARADNAK
-- - Az új kód először az új listákat olvassa, aztán fallback a régire
-- 
-- 📝 HASZNÁLAT:
-- 1. Nyisd meg SQL Server Management Studio-t
-- 2. Kapcsolódj az adatbázishoz
-- 3. Módosítsd a USE [BiztvillCRM] sort a saját adatbázis nevedre
-- 4. Futtasd le ezt a scriptet (F5)
-- 5. Ellenőrizd az eredményt a ELLENŐRZÉS blokk lefuttatásával
-- 
-- ⚠️ FONTOS:
-- - NE futtasd produkción tesztelés nélkül!
-- - Készíts backup-ot előtte!
-- - Ha bármi hiba van, ROLLBACK-et végrehajthatod
-- =====================================================
