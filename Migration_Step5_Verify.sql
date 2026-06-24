-- ========================================
-- VÉGSŐ ELLENŐRZÉS ÉS STATISZTIKA
-- ========================================
USE [CRM]
GO

PRINT ''
PRINT '========================================='
PRINT 'MIGRÁCIÓ ELLENŐRZÉS ÉS STATISZTIKA'
PRINT '========================================='
PRINT ''

-- 1. Karbantartasok tábla oszlopai
PRINT 'Karbantartasok tábla oszlopai:'
PRINT '---------------------------------'
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett,
	COLUMN_DEFAULT AS Alapertelmezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Karbantartasok'
ORDER BY ORDINAL_POSITION;

PRINT ''
PRINT 'Karbantartások státusz szerinti eloszlása:'
PRINT '---------------------------------'
SELECT 
	CASE Statusz
		WHEN 0 THEN 'Tervezett'
		WHEN 1 THEN 'Folyamatban'
		WHEN 2 THEN 'Elvégezve'
		WHEN 3 THEN 'Elhalasztva'
		ELSE 'Ismeretlen'
	END AS StatuszNev,
	Statusz AS StatuszKod,
	COUNT(*) AS Darab
FROM [dbo].[Karbantartasok]
GROUP BY Statusz
ORDER BY Statusz;

PRINT ''
PRINT 'EmailKuldesNaplok tábla kapcsoló oszlopai:'
PRINT '---------------------------------'
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'EmailKuldesNaplok' 
  AND COLUMN_NAME IN ('HitelesitesId', 'MeresId', 'KarbantartasId')
ORDER BY COLUMN_NAME;

PRINT ''
PRINT '========================================='
PRINT 'MIGRÁCIÓ ELLENŐRZÉS BEFEJEZVE'
PRINT '========================================='

GO
