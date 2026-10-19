-- ========================================
-- MUNKASZÁM MODUL MIGRÁCIÓ SQL SCRIPT
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'Munkaszám modul migráció indítása...';
PRINT '';

-- 1. MUNKASZAMOK TÁBLA
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Munkaszamok' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	PRINT '1. Munkaszamok tábla létrehozása...';
	CREATE TABLE [dbo].[Munkaszamok] (
		[Id] int NOT NULL IDENTITY(1,1),
		[CegId] int NOT NULL,
		[Letrehozva] datetime2 NOT NULL,
		[Modositva] datetime2 NULL,
		[Szam] nvarchar(50) NOT NULL,
		[Megnevezes] nvarchar(300) NULL,
		[Aktiv] bit NOT NULL,
		CONSTRAINT [PK_Munkaszamok] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_Munkaszamok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [dbo].[Cegek] ([Id]) ON DELETE CASCADE
	);
	PRINT '   OK - Munkaszamok tábla létrehozva';
END
ELSE
	PRINT '1. SKIP - Munkaszamok tábla már létezik';
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Munkaszamok_CegId' AND object_id = OBJECT_ID(N'[dbo].[Munkaszamok]'))
BEGIN
	PRINT '2. IX_Munkaszamok_CegId index létrehozása...';
	CREATE INDEX [IX_Munkaszamok_CegId] ON [dbo].[Munkaszamok] ([CegId]);
	PRINT '   OK - Index létrehozva';
END
ELSE
	PRINT '2. SKIP - IX_Munkaszamok_CegId index már létezik';
GO

-- 3. HITELESITESEK.MUNKASZAMID OSZLOP
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]') AND name = 'MunkaszamId')
BEGIN
	PRINT '3. MunkaszamId oszlop hozzáadása a Hitelesitesek táblához...';
	ALTER TABLE [dbo].[Hitelesitesek] ADD [MunkaszamId] int NULL;
	PRINT '   OK - MunkaszamId oszlop hozzáadva';
END
ELSE
	PRINT '3. SKIP - MunkaszamId oszlop már létezik';
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Hitelesitesek_MunkaszamId' AND object_id = OBJECT_ID(N'[dbo].[Hitelesitesek]'))
BEGIN
	PRINT '4. IX_Hitelesitesek_MunkaszamId index létrehozása...';
	CREATE INDEX [IX_Hitelesitesek_MunkaszamId] ON [dbo].[Hitelesitesek] ([MunkaszamId]);
	PRINT '   OK - Index létrehozva';
END
ELSE
	PRINT '4. SKIP - IX_Hitelesitesek_MunkaszamId index már létezik';
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Hitelesitesek_Munkaszamok_MunkaszamId')
BEGIN
	PRINT '5. FK_Hitelesitesek_Munkaszamok_MunkaszamId foreign key létrehozása...';
	ALTER TABLE [dbo].[Hitelesitesek]
		ADD CONSTRAINT [FK_Hitelesitesek_Munkaszamok_MunkaszamId]
		FOREIGN KEY ([MunkaszamId]) REFERENCES [dbo].[Munkaszamok] ([Id]) ON DELETE SET NULL;
	PRINT '   OK - Foreign key létrehozva';
END
ELSE
	PRINT '5. SKIP - FK_Hitelesitesek_Munkaszamok_MunkaszamId foreign key már létezik';
GO

-- 6. MUNKASZAMSZAMLALOK TÁBLA (MunkaszamTorzsService/MunkaszamService által generált sorszámozáshoz)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MunkaszamSzamlalok' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	PRINT '6. MunkaszamSzamlalok tábla létrehozása...';
	CREATE TABLE [dbo].[MunkaszamSzamlalok] (
		[Id] int NOT NULL IDENTITY(1,1),
		[CegId] int NOT NULL,
		[Ev] int NOT NULL,
		[UtolsoSorszam] int NOT NULL,
		CONSTRAINT [PK_MunkaszamSzamlalok] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_MunkaszamSzamlalok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [dbo].[Cegek] ([Id]) ON DELETE CASCADE
	);
	PRINT '   OK - MunkaszamSzamlalok tábla létrehozva';
END
ELSE
	PRINT '6. SKIP - MunkaszamSzamlalok tábla már létezik';
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MunkaszamSzamlalok_CegId_Ev' AND object_id = OBJECT_ID(N'[dbo].[MunkaszamSzamlalok]'))
BEGIN
	PRINT '7. IX_MunkaszamSzamlalok_CegId_Ev unique index létrehozása...';
	CREATE UNIQUE INDEX [IX_MunkaszamSzamlalok_CegId_Ev] ON [dbo].[MunkaszamSzamlalok] ([CegId], [Ev]);
	PRINT '   OK - Index létrehozva';
END
ELSE
	PRINT '7. SKIP - IX_MunkaszamSzamlalok_CegId_Ev index már létezik';
GO

PRINT '';
PRINT 'Munkaszám modul migráció befejezve.';
GO
