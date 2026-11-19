-- ========================================
-- HITELESITESEK.LyukadasjelzoTetelek OSZLOP MIGRACIO SQL SCRIPT
-- Tobb kulon lyukadasjelzo felvitele egy hitelesitesen belul
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'Hitelesitesek.LyukadasjelzoTetelek migracio inditasa...';
PRINT '';

IF NOT EXISTS (
	SELECT * FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') AND name = 'LyukadasjelzoTetelek'
)
BEGIN
	PRINT '1. Hitelesitesek.LyukadasjelzoTetelek oszlop letrehozasa...';
	ALTER TABLE [dbo].[Hitelesitesek]
		ADD [LyukadasjelzoTetelek] nvarchar(max) NULL;
	PRINT '   OK - Hitelesitesek.LyukadasjelzoTetelek oszlop letrehozva';
END
ELSE
	PRINT '1. SKIP - Hitelesitesek.LyukadasjelzoTetelek oszlop mar letezik';
GO

PRINT '';
PRINT 'Hitelesitesek.LyukadasjelzoTetelek migracio befejezve.';
GO
