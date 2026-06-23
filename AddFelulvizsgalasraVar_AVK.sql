-- Migráció: AddFelulvizsgalasraVar ÁVK típusokhoz
-- Hozzáadja a FelulvizsgalasraVar oszlopot az AvkVedelemTipusok táblához

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'AvkVedelemTipusok') 
	AND name = 'FelulvizsgalasraVar'
)
BEGIN
	ALTER TABLE AvkVedelemTipusok
	ADD FelulvizsgalasraVar BIT NOT NULL DEFAULT 0;

	PRINT 'FelulvizsgalasraVar oszlop sikeresen hozzáadva az AvkVedelemTipusok táblához.';
END
ELSE
BEGIN
	PRINT 'FelulvizsgalasraVar oszlop már létezik az AvkVedelemTipusok táblában.';
END
GO
