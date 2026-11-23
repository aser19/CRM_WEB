-- ========================================
-- FOLYADÉKKAL MŰKÖDTETETT LYUKADÁSJELZŐ VIZSGÁLATI JEGYZŐKÖNYV MIGRÁCIÓ SQL SCRIPT
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'Folyadekos lyukadasjelzo jegyzokonyv migracio inditasa...';
PRINT '';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FolyadekosLyukadasjelzoAdatokJson' AND Object_ID = OBJECT_ID(N'[dbo].[Hitelesitesek]'))
BEGIN
	PRINT '1. FolyadekosLyukadasjelzoAdatokJson oszlop hozzaadasa...';
	ALTER TABLE [dbo].[Hitelesitesek] ADD [FolyadekosLyukadasjelzoAdatokJson] nvarchar(max) NULL;
	PRINT '   OK - FolyadekosLyukadasjelzoAdatokJson oszlop hozzaadva';
END
ELSE
	PRINT '1. SKIP - FolyadekosLyukadasjelzoAdatokJson oszlop mar letezik';
GO

PRINT '';
PRINT 'Folyadekos lyukadasjelzo jegyzokonyv migracio befejezve.';
PRINT 'Megjegyzes: a tobbes tetel (LyukadasjelzoTetel) es a csoporttag (kozbenso vizsgalat,';
PRINT 'CsoportTagLejaratReszlet) szintu FolyadekosLyukadasjelzoAdatokJson mezok a mar letezo';
PRINT 'Hitelesitesek.LyukadasjelzoTetelek / CsoportTagLejaratok JSON oszlopokba agyazodnak,';
PRINT 'ezert azokhoz nem szukseges kulon oszlop/migracio.';
GO
