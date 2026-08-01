-- Migráció: Rb védelmi módok tábla létrehozása
-- Az Rb (robbanásbiztos) berendezéseknél használt "Védelmi mód" értékek tárolására.
-- Ha egy felhasználó olyan értéket ad meg, ami még nincs ebben a táblában,
-- az alkalmazás automatikusan felveszi FelulvizsgalasraVar = 1 (jóváhagyásra váró) állapotban.

IF NOT EXISTS (
	SELECT * FROM sys.tables
	WHERE name = 'RbVedelmiModok'
)
BEGIN
	CREATE TABLE [RbVedelmiModok] (
		[Id] int NOT NULL IDENTITY,
		[Nev] nvarchar(max) NOT NULL,
		[Leiras] nvarchar(max) NULL,
		[Aktiv] bit NOT NULL DEFAULT 1,
		[FelulvizsgalasraVar] bit NOT NULL DEFAULT 0,
		[Letrehozva] datetime2 NOT NULL DEFAULT GETDATE(),
		CONSTRAINT [PK_RbVedelmiModok] PRIMARY KEY ([Id])
	);

	PRINT 'RbVedelmiModok tábla sikeresen létrehozva.';
END
ELSE
BEGIN
	PRINT 'RbVedelmiModok tábla már létezik.';
END
GO
