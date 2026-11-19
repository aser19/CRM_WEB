-- ========================================
-- LYUKADÁSJELZŐ JEGYZŐKÖNYV SORSZÁM SZÁMLÁLÓ MIGRÁCIÓ SQL SCRIPT
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'LyukadasjelzoJegyzokonyvSzamlalo migracio inditasa...';
PRINT '';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE Name = N'LyukadasjelzoJegyzokonyvSzamlalok')
BEGIN
	PRINT '1. LyukadasjelzoJegyzokonyvSzamlalok tabla letrehozasa...';
	CREATE TABLE [dbo].[LyukadasjelzoJegyzokonyvSzamlalok]
	(
		[Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[CegId] int NOT NULL,
		[Ev] int NOT NULL,
		[UtolsoSorszam] int NOT NULL DEFAULT(0)
	);

	ALTER TABLE [dbo].[LyukadasjelzoJegyzokonyvSzamlalok]
		ADD CONSTRAINT [FK_LyukadasjelzoJegyzokonyvSzamlalok_Cegek]
		FOREIGN KEY ([CegId]) REFERENCES [dbo].[Cegek]([Id]) ON DELETE CASCADE;

	CREATE UNIQUE INDEX [IX_LyukadasjelzoJegyzokonyvSzamlalok_CegId_Ev]
		ON [dbo].[LyukadasjelzoJegyzokonyvSzamlalok]([CegId], [Ev]);

	PRINT '   OK - LyukadasjelzoJegyzokonyvSzamlalok tabla letrehozva';
END
ELSE
	PRINT '1. SKIP - LyukadasjelzoJegyzokonyvSzamlalok tabla mar letezik';
GO

PRINT '';
PRINT 'LyukadasjelzoJegyzokonyvSzamlalo migracio befejezve.';
GO
