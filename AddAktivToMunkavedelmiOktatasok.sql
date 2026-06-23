-- MunkavedelmiOktatasok tábla: Aktiv oszlop ellenőrzése és hozzáadása (ha szükséges)
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'MunkavedelmiOktatasok' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE MunkavedelmiOktatasok
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT 'Aktiv oszlop sikeresen hozzáadva a MunkavedelmiOktatasok táblához.';
END
ELSE
BEGIN
	PRINT 'Az Aktiv oszlop már létezik a MunkavedelmiOktatasok táblában.';
END
GO
