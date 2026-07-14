-- =====================================================
-- MULTIPLE FILE SUPPORT - EGYSZERŰSÍTETT SCRIPT
-- =====================================================

USE [BiztvillCRM]  -- ← MÓDOSÍTSD az adatbázis nevére!
GO

PRINT '========================================';
PRINT 'Multiple File Support Migration START';
PRINT '========================================';

-- =====================================================
-- 1. Új oszlopok hozzáadása
-- =====================================================

PRINT 'Lépés 1: Új oszlopok hozzáadása...';

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') 
	AND name = 'MunkalapPaths'
)
BEGIN
	ALTER TABLE [dbo].[Hitelesitesek]
	ADD [MunkalapPaths] NVARCHAR(MAX) NULL;

	PRINT '  ✓ MunkalapPaths oszlop hozzáadva.';
END
ELSE
BEGIN
	PRINT '  ℹ MunkalapPaths oszlop már létezik.';
END

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') 
	AND name = 'BizonyitvanyPaths'
)
BEGIN
	ALTER TABLE [dbo].[Hitelesitesek]
	ADD [BizonyitvanyPaths] NVARCHAR(MAX) NULL;

	PRINT '  ✓ BizonyitvanyPaths oszlop hozzáadva.';
END
ELSE
BEGIN
	PRINT '  ℹ BizonyitvanyPaths oszlop már létezik.';
END

-- =====================================================
-- 2. Adatmigráció: Munkalap
-- =====================================================

PRINT 'Lépés 2: Munkalap adatok migrálása...';

DECLARE @migratedMunkalap INT;

UPDATE [dbo].[Hitelesitesek]
SET [MunkalapPaths] = '["' + REPLACE([MunkalapPath], '"', '\"') + '"]'
WHERE [MunkalapPath] IS NOT NULL 
  AND [MunkalapPath] <> ''
  AND ([MunkalapPaths] IS NULL OR [MunkalapPaths] = '');

SET @migratedMunkalap = @@ROWCOUNT;

PRINT '  ✓ ' + CAST(@migratedMunkalap AS NVARCHAR(10)) + ' MunkalapPath migrálva.';

-- =====================================================
-- 3. Adatmigráció: Bizonyítvány
-- =====================================================

PRINT 'Lépés 3: Bizonyítvány adatok migrálása...';

DECLARE @migratedBizonyitvany INT;

UPDATE [dbo].[Hitelesitesek]
SET [BizonyitvanyPaths] = '["' + REPLACE([BizonyitvanyPath], '"', '\"') + '"]'
WHERE [BizonyitvanyPath] IS NOT NULL 
  AND [BizonyitvanyPath] <> ''
  AND ([BizonyitvanyPaths] IS NULL OR [BizonyitvanyPaths] = '');

SET @migratedBizonyitvany = @@ROWCOUNT;

PRINT '  ✓ ' + CAST(@migratedBizonyitvany AS NVARCHAR(10)) + ' BizonyitvanyPath migrálva.';

-- =====================================================
-- 4. Migration history
-- =====================================================

PRINT 'Lépés 4: Migration history frissítése...';

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
BEGIN
	IF NOT EXISTS (
		SELECT * FROM [__EFMigrationsHistory] 
		WHERE [MigrationId] = N'20241215000000_AddMultipleFileSupportForDocuments'
	)
	BEGIN
		INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
		VALUES (N'20241215000000_AddMultipleFileSupportForDocuments', N'8.0.0');

		PRINT '  ✓ Migration history frissítve.';
	END
	ELSE
	BEGIN
		PRINT '  ℹ Migration history már tartalmazza.';
	END
END
ELSE
BEGIN
	PRINT '  ℹ __EFMigrationsHistory tábla nem található.';
END

-- =====================================================
-- KÉSZ!
-- =====================================================

PRINT '========================================';
PRINT '✅ Migration sikeresen befejezve!';
PRINT '========================================';

-- =====================================================
-- ELLENŐRZÉS - Most már biztonságos futtatni
-- =====================================================

PRINT '';
PRINT 'Ellenőrzés:';

-- Oszlopok léteznek?
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') AND name = 'MunkalapPaths')
	PRINT '  ✓ MunkalapPaths oszlop létezik';
ELSE
	PRINT '  ✗ MunkalapPaths oszlop HIÁNYZIK!';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') AND name = 'BizonyitvanyPaths')
	PRINT '  ✓ BizonyitvanyPaths oszlop létezik';
ELSE
	PRINT '  ✗ BizonyitvanyPaths oszlop HIÁNYZIK!';

-- Hány rekord lett migrálva?
DECLARE @countMunkalap INT, @countBizonyitvany INT;

SELECT @countMunkalap = COUNT(*) 
FROM [dbo].[Hitelesitesek] 
WHERE MunkalapPaths IS NOT NULL;

SELECT @countBizonyitvany = COUNT(*) 
FROM [dbo].[Hitelesitesek] 
WHERE BizonyitvanyPaths IS NOT NULL;

PRINT '  ℹ Munkalap JSON rekordok: ' + CAST(@countMunkalap AS NVARCHAR(10));
PRINT '  ℹ Bizonyítvány JSON rekordok: ' + CAST(@countBizonyitvany AS NVARCHAR(10));

GO
