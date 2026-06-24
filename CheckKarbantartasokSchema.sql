-- Ellenőrizzük a Karbantartasok tábla jelenlegi állapotát
USE [CRM]
GO

PRINT 'Karbantartasok tábla oszlopai:'
PRINT '================================'
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett,
	COLUMN_DEFAULT AS Alapertelmezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Karbantartasok'
ORDER BY ORDINAL_POSITION;
GO

PRINT ''
PRINT 'EmailKuldesNaplok tábla oszlopai:'
PRINT '================================'
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus,
	IS_NULLABLE AS NullEngedelyezett
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'EmailKuldesNaplok'
ORDER BY ORDINAL_POSITION;
GO
