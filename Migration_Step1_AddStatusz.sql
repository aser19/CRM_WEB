-- ========================================
-- 1. LÉPÉS: STATUSZ OSZLOP HOZZÁADÁSA
-- ========================================
USE [CRM]
GO

PRINT 'LÉPÉS 1: Statusz oszlop hozzáadása...'

IF NOT EXISTS (SELECT * FROM sys.columns 
			   WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') 
			   AND name = 'Statusz')
BEGIN
	ALTER TABLE [dbo].[Karbantartasok] 
	ADD [Statusz] int NOT NULL CONSTRAINT DF_Karbantartasok_Statusz DEFAULT 0;

	PRINT '   ✓ Statusz oszlop sikeresen hozzáadva';
END
ELSE
BEGIN
	PRINT '   ! Statusz oszlop már létezik, átugrás';
END

-- Ellenőrzés
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Karbantartasok' 
  AND COLUMN_NAME IN ('Statusz', 'Elvegezve')
ORDER BY COLUMN_NAME;

GO
