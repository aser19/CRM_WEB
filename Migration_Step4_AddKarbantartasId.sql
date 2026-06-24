-- ========================================
-- 4. LÉPÉS: KARBANTARTASID HOZZÁADÁSA
-- ========================================
USE [CRM]
GO

PRINT 'LÉPÉS 4: KarbantartasId oszlop hozzáadása EmailKuldesNaplok táblához...'

IF NOT EXISTS (SELECT * FROM sys.columns 
			   WHERE object_id = OBJECT_ID(N'[dbo].[EmailKuldesNaplok]') 
			   AND name = 'KarbantartasId')
BEGIN
	ALTER TABLE [dbo].[EmailKuldesNaplok]
	ADD [KarbantartasId] int NULL;

	PRINT '   ✓ KarbantartasId oszlop sikeresen hozzáadva';
END
ELSE
BEGIN
	PRINT '   ! KarbantartasId oszlop már létezik';
END

-- Ellenőrzés
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'EmailKuldesNaplok' 
  AND COLUMN_NAME IN ('KarbantartasId', 'HitelesitesId', 'MeresId')
ORDER BY COLUMN_NAME;

GO
