-- =============================================
-- Hitelesítés fájlfeltöltés támogatás
-- Migráció: AddHitelesitesFajlok
-- Dátum: 2026-06-29
-- =============================================

-- Munkalap és Bizonyítvány fájl útvonalak hozzáadása a Hitelesitesek táblához
ALTER TABLE [Hitelesitesek] 
ADD [MunkalapPath] nvarchar(500) NULL;
GO

ALTER TABLE [Hitelesitesek] 
ADD [BizonyitvanyPath] nvarchar(500) NULL;
GO

-- Migráció bejegyzés az EF Core history táblába
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629170906_AddHitelesitesFajlok', N'8.0.0');
GO

-- =============================================
-- Rollback script (ha szükséges visszavonni):
-- =============================================
/*
ALTER TABLE [Hitelesitesek] DROP COLUMN [MunkalapPath];
ALTER TABLE [Hitelesitesek] DROP COLUMN [BizonyitvanyPath];
DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260629170906_AddHitelesitesFajlok';
*/
