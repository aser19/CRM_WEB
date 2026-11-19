-- ========================================
-- LYUKADÁSJELZŐ VIZSGÁLATI JEGYZŐKÖNYV MIGRÁCIÓ SQL SCRIPT
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'Lyukadasjelzo jegyzokonyv migracio inditasa...';
PRINT '';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LyukadasjelzoAltipus' AND Object_ID = OBJECT_ID(N'[dbo].[Hitelesitesek]'))
BEGIN
	PRINT '1. LyukadasjelzoAltipus oszlop hozzaadasa...';
	ALTER TABLE [dbo].[Hitelesitesek] ADD [LyukadasjelzoAltipus] int NULL;
	PRINT '   OK - LyukadasjelzoAltipus oszlop hozzaadva';
END
ELSE
	PRINT '1. SKIP - LyukadasjelzoAltipus oszlop mar letezik';
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LyukadasjelzoAdatokJson' AND Object_ID = OBJECT_ID(N'[dbo].[Hitelesitesek]'))
BEGIN
	PRINT '2. LyukadasjelzoAdatokJson oszlop hozzaadasa...';
	ALTER TABLE [dbo].[Hitelesitesek] ADD [LyukadasjelzoAdatokJson] nvarchar(max) NULL;
	PRINT '   OK - LyukadasjelzoAdatokJson oszlop hozzaadva';
END
ELSE
	PRINT '2. SKIP - LyukadasjelzoAdatokJson oszlop mar letezik';
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'JegyzokonyvPaths' AND Object_ID = OBJECT_ID(N'[dbo].[Hitelesitesek]'))
BEGIN
	PRINT '3. JegyzokonyvPaths oszlop hozzaadasa...';
	ALTER TABLE [dbo].[Hitelesitesek] ADD [JegyzokonyvPaths] nvarchar(max) NULL;
	PRINT '   OK - JegyzokonyvPaths oszlop hozzaadva';
END
ELSE
	PRINT '3. SKIP - JegyzokonyvPaths oszlop mar letezik';
GO

PRINT '';
PRINT 'Lyukadasjelzo jegyzokonyv migracio befejezve.';
PRINT 'Megjegyzes: a csoporttag (kozbenso vizsgalat) szintu JegyzokonyvKitoltesMod es';
PRINT 'LyukadasjelzoAdatokJson mezok a mar letezo Hitelesitesek.CsoportTagLejaratok JSON';
PRINT 'oszlopba agyazodnak, ezert azokhoz nem szukseges kulon oszlop/migracio.';
GO
