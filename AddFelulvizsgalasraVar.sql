-- Migráció: AddFelulvizsgalasraVar
-- Hozzáadja a FelulvizsgalasraVar oszlopot a TularamvedelemTipusok táblához

IF NOT EXISTS (
	SELECT * FROM sys.columns 
	WHERE object_id = OBJECT_ID(N'TularamvedelemTipusok') 
	AND name = 'FelulvizsgalasraVar'
)
BEGIN
	ALTER TABLE TularamvedelemTipusok
	ADD FelulvizsgalasraVar BIT NOT NULL DEFAULT 0;

	PRINT 'FelulvizsgalasraVar oszlop sikeresen hozzáadva.';
END
ELSE
BEGIN
	PRINT 'FelulvizsgalasraVar oszlop már létezik.';
END
GO
