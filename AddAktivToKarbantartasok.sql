-- Karbantartasok tábla: Aktiv oszlop hozzáadása
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Karbantartasok' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Karbantartasok
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT 'Aktiv oszlop sikeresen hozzáadva a Karbantartasok táblához.';
END
ELSE
BEGIN
	PRINT 'Az Aktiv oszlop már létezik a Karbantartasok táblában.';
END
GO
