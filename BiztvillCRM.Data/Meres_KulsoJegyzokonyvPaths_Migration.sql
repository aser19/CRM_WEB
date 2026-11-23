-- Meres.KulsoJegyzokonyvPaths oszlop hozzáadása
-- Célja: régi, nem a szoftverrel kiállított mérési jegyzőkönyvek (PDF, Word, Excel)
-- feltöltött fájlútvonalainak JSON listaként való tárolása mellékletként.

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'dbo.Meresek') AND name = 'KulsoJegyzokonyvPaths'
)
BEGIN
	ALTER TABLE dbo.Meresek ADD KulsoJegyzokonyvPaths NVARCHAR(MAX) NULL;
END
GO
