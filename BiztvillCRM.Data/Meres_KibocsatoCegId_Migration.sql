-- Meres.KibocsatoCegId oszlop hozzáadása
-- Célja: az "Új mérés" dialógusban kiválasztott kibocsátó cég tárolása azoknál a felhasználóknál,
-- akik több céghez (FelhasznaloCegek) is hozzáférnek. Ha NULL, az aktuális (bejelentkezett) cég számít
-- alapértelmezettnek a jegyzőkönyv-generáláshoz (ugyanúgy, mint korábban).

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'dbo.Meresek') AND name = 'KibocsatoCegId'
)
BEGIN
	ALTER TABLE dbo.Meresek ADD KibocsatoCegId INT NULL;
END
GO
