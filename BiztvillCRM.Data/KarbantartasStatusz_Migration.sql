-- ========================================
-- KARBANTARTÁS STÁTUSZ MIGRÁCIÓ SQL SCRIPT  
-- ========================================
USE [CRM]
GO

SET NOCOUNT ON;
GO

PRINT 'Karbantartás státusz migráció indítása...';
PRINT '';

-- 1. STATUSZ OSZLOP
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND name = 'Statusz')
BEGIN
    PRINT '1. Statusz oszlop hozzáadása...';
    ALTER TABLE [dbo].[Karbantartasok] ADD [Statusz] int NOT NULL CONSTRAINT DF_Karbantartasok_Statusz DEFAULT 0;
    PRINT '   OK - Statusz oszlop hozzáadva';
END
ELSE
    PRINT '1. SKIP - Statusz oszlop már létezik';
GO

-- 2. ADATKONVERZIÓ
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND name = 'Elvegezve')
BEGIN
    PRINT '2. Adatok konvertálása...';
    UPDATE [dbo].[Karbantartasok] SET [Statusz] = 2 WHERE [Elvegezve] = 1;
    PRINT '   OK - Adatok konvertálva';
END
ELSE
    PRINT '2. SKIP - Elvegezve oszlop már törölve';
GO

-- 3. ELVEGEZVE TÖRLÉSE
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND name = 'Elvegezve')
BEGIN
    PRINT '3. Elvegezve oszlop eltávolítása...';
    DECLARE @ConstraintName NVARCHAR(200);
    SELECT @ConstraintName = dc.name FROM sys.default_constraints dc INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id WHERE c.object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND c.name = 'Elvegezve';
    IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE [dbo].[Karbantartasok] DROP CONSTRAINT [' + @ConstraintName + ']');
    ALTER TABLE [dbo].[Karbantartasok] DROP COLUMN [Elvegezve];
    PRINT '   OK - Elvegezve oszlop törölve';
END
ELSE
    PRINT '3. SKIP - Elvegezve oszlop már törölve';
GO

-- 4. KARBANTARTASID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EmailKuldesNaplok]') AND name = 'KarbantartasId')
BEGIN
    PRINT '4. KarbantartasId oszlop hozzáadása...';
    ALTER TABLE [dbo].[EmailKuldesNaplok] ADD [KarbantartasId] int NULL;
    PRINT '   OK - KarbantartasId hozzáadva';
END
ELSE
    PRINT '4. SKIP - KarbantartasId már létezik';
GO

PRINT '';
PRINT '*** MIGRÁCIÓ BEFEJEZVE! ***';
GO
