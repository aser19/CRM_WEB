-- MeresTagek és MeresTagKapcsolatok táblák létrehozása
-- Célja: a mérésekhez/felülvizsgálatokhoz rendelhető tagek (címkék) tárolása.
-- Ezek nélkül a Meres lekérdezések (.Include(m => m.Tagek)) SQL hibát dobnak,
-- ami pl. a Nem Norma szerinti villámvédelmi jegyzőkönyv szerkesztőjének
-- végtelen betöltését (forgó karika) okozza.

IF NOT EXISTS (
	SELECT 1 FROM sys.tables WHERE name = 'MeresTagek' AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
	CREATE TABLE dbo.MeresTagek
	(
		Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Nev NVARCHAR(100) NOT NULL,
		Szin NVARCHAR(20) NOT NULL DEFAULT '#607D8B'
	);
END
GO

IF NOT EXISTS (
	SELECT 1 FROM sys.tables WHERE name = 'MeresTagKapcsolatok' AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
	CREATE TABLE dbo.MeresTagKapcsolatok
	(
		TagekId INT NOT NULL,
		MeresId INT NOT NULL,
		CONSTRAINT PK_MeresTagKapcsolatok PRIMARY KEY (TagekId, MeresId),
		CONSTRAINT FK_MeresTagKapcsolatok_MeresTagek FOREIGN KEY (TagekId) REFERENCES dbo.MeresTagek(Id) ON DELETE CASCADE,
		CONSTRAINT FK_MeresTagKapcsolatok_Meresek FOREIGN KEY (MeresId) REFERENCES dbo.Meresek(Id) ON DELETE NO ACTION
	);
END
GO
