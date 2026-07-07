-- ========================================
-- BIZTOVILL CRM - PENDING MIGRÁCIÓK
-- Létrehozva: 2026-07-07
-- ========================================
-- FONTOS: Futtasd sorrendben, NE hagyd ki egyiket sem!
-- ========================================

BEGIN TRANSACTION;
GO

-- ========================================
-- 1. KarbantartasStatusz migráció
-- ========================================

PRINT 'Migráció 1/3: KarbantartasStatusz';
GO

-- UgyfelLekerdezesiTokenek: oszlop átnevezés
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UgyfelLekerdezesiTokenek') AND name = 'Lejarat')
BEGIN
	EXEC sp_rename 'UgyfelLekerdezesiTokenek.Lejarat', 'LejarDatum', 'COLUMN';
END
GO

-- Karbantartasok: oszlop átnevezés és új oszlopok
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Karbantartasok') AND name = 'Elvegezve')
BEGIN
	EXEC sp_rename 'Karbantartasok.Elvegezve', 'Aktiv', 'COLUMN';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Karbantartasok') AND name = 'Statusz')
BEGIN
	ALTER TABLE [Karbantartasok] ADD [Statusz] int NOT NULL DEFAULT 0;
END
GO

-- VizsgalatiSablonok: új oszlopok
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VizsgalatiSablonok') AND name = 'AdatokJson')
BEGIN
	ALTER TABLE [VizsgalatiSablonok] ADD [AdatokJson] nvarchar(max) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VizsgalatiSablonok') AND name = 'CegId')
BEGIN
	ALTER TABLE [VizsgalatiSablonok] ADD [CegId] int NULL;
END
GO

-- TularamvedelemTipusok: új oszlop
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TularamvedelemTipusok') AND name = 'FelulvizsgalasraVar')
BEGIN
	ALTER TABLE [TularamvedelemTipusok] ADD [FelulvizsgalasraVar] bit NOT NULL DEFAULT 0;
END
GO

-- MeresTipusok: új oszlop
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MeresTipusok') AND name = 'FoMeres')
BEGIN
	ALTER TABLE [MeresTipusok] ADD [FoMeres] bit NOT NULL DEFAULT 0;
END
GO

-- Meresek: új oszlop
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Meresek') AND name = 'Aktiv')
BEGIN
	ALTER TABLE [Meresek] ADD [Aktiv] bit NOT NULL DEFAULT 0;
	-- Meglévő sorok aktívra állítása
	UPDATE [Meresek] SET [Aktiv] = 1;
END
GO

-- JegyzokonyvSablonTetelek: új oszlop
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('JegyzokonyvSablonTetelek') AND name = 'CegId')
BEGIN
	ALTER TABLE [JegyzokonyvSablonTetelek] ADD [CegId] int NULL;
END
GO

-- Hitelesitesek: új oszlopok
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Hitelesitesek') AND name = 'Aktiv')
BEGIN
	ALTER TABLE [Hitelesitesek] ADD [Aktiv] bit NOT NULL DEFAULT 0;
	-- Meglévő sorok aktívra állítása
	UPDATE [Hitelesitesek] SET [Aktiv] = 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Hitelesitesek') AND name = 'CsoportTagLejaratok')
BEGIN
	ALTER TABLE [Hitelesitesek] ADD [CsoportTagLejaratok] nvarchar(max) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Hitelesitesek') AND name = 'HitelesitesCsoportId')
BEGIN
	ALTER TABLE [Hitelesitesek] ADD [HitelesitesCsoportId] int NULL;
END
GO

-- Eszközök: új oszlopok
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Eszkozok') AND name = 'HitelesitesCsoportId')
BEGIN
	ALTER TABLE [Eszkozok] ADD [HitelesitesCsoportId] int NULL;
END
GO

-- Foreign Key: Hitelesitesek -> HitelesitesCsoportok
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Hitelesitesek_HitelesitesCsoportok_HitelesitesCsoportId')
BEGIN
	ALTER TABLE [Hitelesitesek]
	ADD CONSTRAINT [FK_Hitelesitesek_HitelesitesCsoportok_HitelesitesCsoportId]
	FOREIGN KEY ([HitelesitesCsoportId]) REFERENCES [HitelesitesCsoportok]([Id]) ON DELETE SET NULL;
END
GO

-- Foreign Key: Eszkozok -> HitelesitesCsoportok
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Eszkozok_HitelesitesCsoportok_HitelesitesCsoportId')
BEGIN
	ALTER TABLE [Eszkozok]
	ADD CONSTRAINT [FK_Eszkozok_HitelesitesCsoportok_HitelesitesCsoportId]
	FOREIGN KEY ([HitelesitesCsoportId]) REFERENCES [HitelesitesCsoportok]([Id]) ON DELETE SET NULL;
END
GO

-- Index: Hitelesitesek
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Hitelesitesek_Aktiv')
BEGIN
	CREATE INDEX [IX_Hitelesitesek_Aktiv] ON [Hitelesitesek]([Aktiv]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Hitelesitesek_HitelesitesCsoportId')
BEGIN
	CREATE INDEX [IX_Hitelesitesek_HitelesitesCsoportId] ON [Hitelesitesek]([HitelesitesCsoportId]);
END
GO

-- Index: Meresek
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Meresek_Aktiv')
BEGIN
	CREATE INDEX [IX_Meresek_Aktiv] ON [Meresek]([Aktiv]);
END
GO

-- Index: Eszkozok
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Eszkozok_HitelesitesCsoportId')
BEGIN
	CREATE INDEX [IX_Eszkozok_HitelesitesCsoportId] ON [Eszkozok]([HitelesitesCsoportId]);
END
GO

-- ========================================
-- 2. HitelesitesFileUploads migráció
-- ========================================

PRINT 'Migráció 2/3: HitelesitesFileUploads';
GO

-- Hitelesitesek: file path oszlopok
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Hitelesitesek') AND name = 'BizonyitvanyPath')
BEGIN
	ALTER TABLE [Hitelesitesek] ADD [BizonyitvanyPath] nvarchar(500) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Hitelesitesek') AND name = 'MunkalapPath')
BEGIN
	ALTER TABLE [Hitelesitesek] ADD [MunkalapPath] nvarchar(500) NULL;
END
GO

-- FelulvizsgaloKepzesek: file path oszlop
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FelulvizsgaloKepzesek') AND name = 'BizonyitvanyPath')
BEGIN
	ALTER TABLE [FelulvizsgaloKepzesek] ADD [BizonyitvanyPath] nvarchar(max) NULL;
END
GO

-- Felulvizsgalok: aláírás path
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Felulvizsgalok') AND name = 'AlairasPath')
BEGIN
	ALTER TABLE [Felulvizsgalok] ADD [AlairasPath] nvarchar(max) NULL;
END
GO

-- ========================================
-- 3. AddUzemeltetoSablonFelhasznalo migráció
-- ========================================

PRINT 'Migráció 3/3: AddUzemeltetoSablonFelhasznalo';
GO

-- UzemeltetoSablonFelhasznalok tábla létrehozása
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UzemeltetoSablonFelhasznalok')
BEGIN
	CREATE TABLE [UzemeltetoSablonFelhasznalok] (
		[Id] int NOT NULL IDENTITY,
		[Letrehozva] datetime2 NOT NULL,
		[UzemeltetoSablonId] int NOT NULL,
		[FelhasznaloId] nvarchar(450) NOT NULL,
		[HozzarendeloFelhasznaloId] nvarchar(450) NOT NULL,
		[Aktiv] bit NOT NULL,
		CONSTRAINT [PK_UzemeltetoSablonFelhasznalok] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_AspNetUsers_FelhasznaloId] 
			FOREIGN KEY ([FelhasznaloId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
		CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_AspNetUsers_HozzarendeloFelhasznaloId] 
			FOREIGN KEY ([HozzarendeloFelhasznaloId]) REFERENCES [AspNetUsers] ([Id]),
		CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_UzemeltetoSablonok_UzemeltetoSablonId] 
			FOREIGN KEY ([UzemeltetoSablonId]) REFERENCES [UzemeltetoSablonok] ([Id]) ON DELETE CASCADE
	);
END
GO

-- Indexek létrehozása
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonFelhasznalok_Aktiv')
BEGIN
	CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_Aktiv] ON [UzemeltetoSablonFelhasznalok]([Aktiv]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonFelhasznalok_FelhasznaloId')
BEGIN
	CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_FelhasznaloId] ON [UzemeltetoSablonFelhasznalok]([FelhasznaloId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonFelhasznalok_HozzarendeloFelhasznaloId')
BEGIN
	CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_HozzarendeloFelhasznaloId] ON [UzemeltetoSablonFelhasznalok]([HozzarendeloFelhasznaloId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonFelhasznalok_UzemeltetoSablonId_FelhasznaloId')
BEGIN
	CREATE UNIQUE INDEX [IX_UzemeltetoSablonFelhasznalok_UzemeltetoSablonId_FelhasznaloId] 
	ON [UzemeltetoSablonFelhasznalok]([UzemeltetoSablonId], [FelhasznaloId]);
END
GO

-- ========================================
-- Migration History frissítése
-- ========================================

PRINT 'Migration history frissítése...';
GO

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260624203518_KarbantartasStatusz')
BEGIN
	INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
	VALUES (N'20260624203518_KarbantartasStatusz', N'8.0.0');
END
GO

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260629163936_HitelesitesFileUploads')
BEGIN
	INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
	VALUES (N'20260629163936_HitelesitesFileUploads', N'8.0.0');
END
GO

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260707210153_AddUzemeltetoSablonFelhasznalo')
BEGIN
	INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
	VALUES (N'20260707210153_AddUzemeltetoSablonFelhasznalo', N'8.0.0');
END
GO

COMMIT TRANSACTION;
GO

PRINT '✓ Összes pending migráció sikeresen lefutott!';
GO
