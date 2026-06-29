-- =============================================
-- Üzemeltető modul - Adatbázis migráció
-- Létrehozva: 2026.06.29
-- Leírás: Admin által létrehozott sablonok és 
--         üzemeltető által rögzített adatok kezelése
-- =============================================

-- Előfeltétel: A Cegek és AspNetUsers táblák már léteznek

BEGIN TRANSACTION;

-- =============================================
-- 1. UzemeltetoSablonok tábla létrehozása
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoSablonok]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UzemeltetoSablonok] (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[Letrehozva] DATETIME2(7) NOT NULL,
		[Modositva] DATETIME2(7) NULL,
		[Nev] NVARCHAR(200) NOT NULL,
		[Leiras] NVARCHAR(1000) NULL,
		[JogszabalyiHivatkozas] NVARCHAR(500) NULL,
		[EllenorzesiIdoszakHonap] INT NULL,
		[Aktiv] BIT NOT NULL,
		[CegId] INT NOT NULL,
		[LetrehozoFelhasznaloId] NVARCHAR(450) NOT NULL,

		CONSTRAINT [PK_UzemeltetoSablonok] PRIMARY KEY CLUSTERED ([Id] ASC),

		CONSTRAINT [FK_UzemeltetoSablonok_Cegek_CegId] 
			FOREIGN KEY([CegId]) REFERENCES [dbo].[Cegek] ([Id])
			ON DELETE NO ACTION,

		CONSTRAINT [FK_UzemeltetoSablonok_AspNetUsers_LetrehozoFelhasznaloId] 
			FOREIGN KEY([LetrehozoFelhasznaloId]) REFERENCES [dbo].[AspNetUsers] ([Id])
			ON DELETE NO ACTION
	);

	PRINT 'UzemeltetoSablonok tábla létrehozva.';
END
ELSE
BEGIN
	PRINT 'UzemeltetoSablonok tábla már létezik.';
END

-- =============================================
-- 2. UzemeltetoSablonMezok tábla létrehozása
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoSablonMezok]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UzemeltetoSablonMezok] (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[UzemeltetoSablonId] INT NOT NULL,
		[MezoNev] NVARCHAR(200) NOT NULL,
		[MezoTipus] NVARCHAR(50) NOT NULL,
		[Kotelezo] BIT NOT NULL,
		[Sorrend] INT NOT NULL,
		[AlapErtek] NVARCHAR(500) NULL,
		[Sugo] NVARCHAR(500) NULL,
		[ValidaciosSzabaly] NVARCHAR(500) NULL,

		CONSTRAINT [PK_UzemeltetoSablonMezok] PRIMARY KEY CLUSTERED ([Id] ASC),

		CONSTRAINT [FK_UzemeltetoSablonMezok_UzemeltetoSablonok_UzemeltetoSablonId] 
			FOREIGN KEY([UzemeltetoSablonId]) REFERENCES [dbo].[UzemeltetoSablonok] ([Id])
			ON DELETE CASCADE
	);

	PRINT 'UzemeltetoSablonMezok tábla létrehozva.';
END
ELSE
BEGIN
	PRINT 'UzemeltetoSablonMezok tábla már létezik.';
END

-- =============================================
-- 3. UzemeltetoAdatok tábla létrehozása
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoAdatok]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UzemeltetoAdatok] (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[Letrehozva] DATETIME2(7) NOT NULL,
		[Modositva] DATETIME2(7) NULL,
		[UzemeltetoSablonId] INT NOT NULL,
		[RogzitesDatum] DATETIME2(7) NOT NULL,
		[KovetkezoEsedekesseg] DATETIME2(7) NULL,
		[Statusz] NVARCHAR(50) NOT NULL,
		[CegId] INT NOT NULL,
		[RogzitoFelhasznaloId] NVARCHAR(450) NOT NULL,
		[MezoErtekekJson] NVARCHAR(MAX) NOT NULL,
		[Megjegyzes] NVARCHAR(1000) NULL,
		[Aktiv] BIT NOT NULL,

		CONSTRAINT [PK_UzemeltetoAdatok] PRIMARY KEY CLUSTERED ([Id] ASC),

		CONSTRAINT [FK_UzemeltetoAdatok_Cegek_CegId] 
			FOREIGN KEY([CegId]) REFERENCES [dbo].[Cegek] ([Id])
			ON DELETE NO ACTION,

		CONSTRAINT [FK_UzemeltetoAdatok_AspNetUsers_RogzitoFelhasznaloId] 
			FOREIGN KEY([RogzitoFelhasznaloId]) REFERENCES [dbo].[AspNetUsers] ([Id])
			ON DELETE NO ACTION,

		CONSTRAINT [FK_UzemeltetoAdatok_UzemeltetoSablonok_UzemeltetoSablonId] 
			FOREIGN KEY([UzemeltetoSablonId]) REFERENCES [dbo].[UzemeltetoSablonok] ([Id])
			ON DELETE NO ACTION
	);

	PRINT 'UzemeltetoAdatok tábla létrehozva.';
END
ELSE
BEGIN
	PRINT 'UzemeltetoAdatok tábla már létezik.';
END

-- =============================================
-- 4. Indexek létrehozása - UzemeltetoSablonok
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonok_CegId')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoSablonok_CegId] 
		ON [dbo].[UzemeltetoSablonok]([CegId] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoSablonok_CegId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonok_Aktiv')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoSablonok_Aktiv] 
		ON [dbo].[UzemeltetoSablonok]([Aktiv] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoSablonok_Aktiv';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonok_LetrehozoFelhasznaloId')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoSablonok_LetrehozoFelhasznaloId] 
		ON [dbo].[UzemeltetoSablonok]([LetrehozoFelhasznaloId] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoSablonok_LetrehozoFelhasznaloId';
END

-- =============================================
-- 5. Indexek létrehozása - UzemeltetoSablonMezok
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoSablonMezok_UzemeltetoSablonId_Sorrend')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoSablonMezok_UzemeltetoSablonId_Sorrend] 
		ON [dbo].[UzemeltetoSablonMezok]([UzemeltetoSablonId] ASC, [Sorrend] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoSablonMezok_UzemeltetoSablonId_Sorrend';
END

-- =============================================
-- 6. Indexek létrehozása - UzemeltetoAdatok
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_CegId')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_CegId] 
		ON [dbo].[UzemeltetoAdatok]([CegId] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_CegId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_UzemeltetoSablonId')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_UzemeltetoSablonId] 
		ON [dbo].[UzemeltetoAdatok]([UzemeltetoSablonId] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_UzemeltetoSablonId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_RogzitesDatum')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_RogzitesDatum] 
		ON [dbo].[UzemeltetoAdatok]([RogzitesDatum] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_RogzitesDatum';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_KovetkezoEsedekesseg')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_KovetkezoEsedekesseg] 
		ON [dbo].[UzemeltetoAdatok]([KovetkezoEsedekesseg] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_KovetkezoEsedekesseg';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_Aktiv')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_Aktiv] 
		ON [dbo].[UzemeltetoAdatok]([Aktiv] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_Aktiv';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UzemeltetoAdatok_RogzitoFelhasznaloId')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UzemeltetoAdatok_RogzitoFelhasznaloId] 
		ON [dbo].[UzemeltetoAdatok]([RogzitoFelhasznaloId] ASC);
	PRINT 'Index létrehozva: IX_UzemeltetoAdatok_RogzitoFelhasznaloId';
END

-- =============================================
-- 7. AspNetRoles tábla - Üzemeltető szerepkör
-- =============================================
-- Az új "Uzemelteto" szerepkör hozzáadása (ha még nem létezik)
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE [Name] = 'Uzemelteto')
BEGIN
	INSERT INTO AspNetRoles ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
	VALUES (NEWID(), 'Uzemelteto', 'UZEMELTETO', NEWID());
	PRINT 'Uzemelteto szerepkör hozzáadva az AspNetRoles táblához.';
END
ELSE
BEGIN
	PRINT 'Uzemelteto szerepkör már létezik.';
END

-- =============================================
-- 8. Migráció bejegyzés a __EFMigrationsHistory táblába
-- =============================================
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260629222035_UzemeltetoModul')
BEGIN
	INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
	VALUES (N'20260629222035_UzemeltetoModul', N'8.0.0');
	PRINT 'Migráció bejegyzés hozzáadva.';
END

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'Üzemeltető modul migráció sikeresen lefutott!';
PRINT '========================================';
PRINT '';
PRINT 'Létrehozott táblák:';
PRINT '  - UzemeltetoSablonok (Admin által kezelt sablonok)';
PRINT '  - UzemeltetoSablonMezok (Sablon mezők definíciója)';
PRINT '  - UzemeltetoAdatok (Üzemeltető által rögzített adatok)';
PRINT '';
PRINT 'Új szerepkör:';
PRINT '  - Uzemelteto';
PRINT '';

GO
