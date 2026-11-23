-- UgyfelCsoport_Migration.sql
-- Célja: lehetővé tenni, hogy ugyanaz a valós ügyfél (pl. "Kovács János") több "Ugyfel" rekordon
-- (pl. "Kiskutya Kft", "FGR Kft") keresztül is szerepeljen, és a tokenes lekérdezés (Lekerdezes.razor)
-- ezek összesített adatait egy listában mutassa.

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UgyfelCsoportok')
BEGIN
	CREATE TABLE [UgyfelCsoportok] (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[Nev] NVARCHAR(200) NOT NULL DEFAULT N'',
		[Letrehozva] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
		CONSTRAINT [PK_UgyfelCsoportok] PRIMARY KEY ([Id])
	);
END
GO

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'dbo.Ugyfelek') AND name = 'UgyfelCsoportId'
)
BEGIN
	ALTER TABLE dbo.Ugyfelek ADD UgyfelCsoportId INT NULL;
END
GO

IF NOT EXISTS (
	SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Ugyfelek_UgyfelCsoportok_UgyfelCsoportId'
)
BEGIN
	ALTER TABLE dbo.Ugyfelek
	ADD CONSTRAINT FK_Ugyfelek_UgyfelCsoportok_UgyfelCsoportId
		FOREIGN KEY (UgyfelCsoportId) REFERENCES dbo.UgyfelCsoportok(Id) ON DELETE SET NULL;
END
GO

IF NOT EXISTS (
	SELECT 1 FROM sys.indexes WHERE name = 'IX_Ugyfelek_UgyfelCsoportId'
)
BEGIN
	CREATE INDEX [IX_Ugyfelek_UgyfelCsoportId] ON [Ugyfelek] ([UgyfelCsoportId]);
END
GO
