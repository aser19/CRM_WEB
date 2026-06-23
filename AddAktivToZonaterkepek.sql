-- Zonaterkepek tábla: Aktiv oszlop ellenőrzése és hozzáadása (ha szükséges)
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Zonaterkepek' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Zonaterkepek
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT 'Aktiv oszlop sikeresen hozzáadva a Zonaterkepek táblához.';
END
ELSE
BEGIN
	PRINT 'Az Aktiv oszlop már létezik a Zonaterkepek táblában.';
END
GO
