-- =============================================
-- Üzemeltető modul - ROLLBACK (Visszavonás)
-- Létrehozva: 2026.06.29
-- Leírás: Az Üzemeltető modul összes elemének eltávolítása
-- FIGYELEM: Ez a script törli az összes adatot!
-- =============================================

BEGIN TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'FIGYELEM: Üzemeltető modul törlése folyamatban...';
PRINT '========================================';
PRINT '';

-- =============================================
-- 1. Foreign Key-ek törlése
-- =============================================
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoAdatok_UzemeltetoSablonok_UzemeltetoSablonId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoAdatok] DROP CONSTRAINT [FK_UzemeltetoAdatok_UzemeltetoSablonok_UzemeltetoSablonId];
	PRINT 'Foreign key törölve: FK_UzemeltetoAdatok_UzemeltetoSablonok_UzemeltetoSablonId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoAdatok_AspNetUsers_RogzitoFelhasznaloId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoAdatok] DROP CONSTRAINT [FK_UzemeltetoAdatok_AspNetUsers_RogzitoFelhasznaloId];
	PRINT 'Foreign key törölve: FK_UzemeltetoAdatok_AspNetUsers_RogzitoFelhasznaloId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoAdatok_Cegek_CegId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoAdatok] DROP CONSTRAINT [FK_UzemeltetoAdatok_Cegek_CegId];
	PRINT 'Foreign key törölve: FK_UzemeltetoAdatok_Cegek_CegId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoSablonMezok_UzemeltetoSablonok_UzemeltetoSablonId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoSablonMezok] DROP CONSTRAINT [FK_UzemeltetoSablonMezok_UzemeltetoSablonok_UzemeltetoSablonId];
	PRINT 'Foreign key törölve: FK_UzemeltetoSablonMezok_UzemeltetoSablonok_UzemeltetoSablonId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoSablonok_AspNetUsers_LetrehozoFelhasznaloId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoSablonok] DROP CONSTRAINT [FK_UzemeltetoSablonok_AspNetUsers_LetrehozoFelhasznaloId];
	PRINT 'Foreign key törölve: FK_UzemeltetoSablonok_AspNetUsers_LetrehozoFelhasznaloId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UzemeltetoSablonok_Cegek_CegId')
BEGIN
	ALTER TABLE [dbo].[UzemeltetoSablonok] DROP CONSTRAINT [FK_UzemeltetoSablonok_Cegek_CegId];
	PRINT 'Foreign key törölve: FK_UzemeltetoSablonok_Cegek_CegId';
END

-- =============================================
-- 2. Táblák törlése
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoAdatok]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[UzemeltetoAdatok];
	PRINT 'Tábla törölve: UzemeltetoAdatok';
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoSablonMezok]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[UzemeltetoSablonMezok];
	PRINT 'Tábla törölve: UzemeltetoSablonMezok';
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UzemeltetoSablonok]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[UzemeltetoSablonok];
	PRINT 'Tábla törölve: UzemeltetoSablonok';
END

-- =============================================
-- 3. AspNetRoles - Üzemeltető szerepkör törlése
-- =============================================
-- FIGYELEM: Először töröld a felhasználó-szerepkör kapcsolatokat (AspNetUserRoles)
DELETE FROM AspNetUserRoles WHERE RoleId IN (SELECT Id FROM AspNetRoles WHERE [Name] = 'Uzemelteto');
PRINT 'Üzemeltető szerepkör hozzárendelések törölve.';

-- Most törölhető a szerepkör
IF EXISTS (SELECT * FROM AspNetRoles WHERE [Name] = 'Uzemelteto')
BEGIN
	DELETE FROM AspNetRoles WHERE [Name] = 'Uzemelteto';
	PRINT 'Üzemeltető szerepkör törölve az AspNetRoles táblából.';
END

-- =============================================
-- 4. Migráció bejegyzés törlése
-- =============================================
IF EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260629222035_UzemeltetoModul')
BEGIN
	DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260629222035_UzemeltetoModul';
	PRINT 'Migráció bejegyzés törölve.';
END

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'Üzemeltető modul sikeresen eltávolítva!';
PRINT '========================================';
PRINT '';

GO
