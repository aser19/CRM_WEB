/*
	============================================================================
	BiztvillCRM - "EPH" (kiegyenlítő védelem) mód/osztály felvétele
	============================================================================
	Cél: az ErintesvedelmiModOsztalyok táblába felvenni az "EPH" opciót,
	amelynél a mérési pontoknál:
	  - Túláramvédelem helye/típusa NEM tölthető ki
	  - ÁVK NEM csatolható (VanAvk = 0)
	  - PE folytonosság alapból bepipálva (VanPeFolyt = 1, az UI alapból true-ra állítja)
	  - Minősítés a mért Ω érték alapján: <2Ω MEGFELELT, 2-10Ω MEGFELELT (figyelmeztetéssel),
		>10Ω NEM FELELT MEG (kézzel felülbírálható)

	A script idempotens: ha már létezik "EPH" nevű aktív sor, nem szúr be duplikátumot.
	============================================================================
*/

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF NOT EXISTS (SELECT 1 FROM dbo.ErintesvedelmiModOsztalyok WHERE Nev = N'EPH')
BEGIN
	DECLARE @UjId INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.ErintesvedelmiModOsztalyok);
	DECLARE @UjSorrend INT = (SELECT ISNULL(MAX(Sorrend), 0) + 1 FROM dbo.ErintesvedelmiModOsztalyok);

	INSERT INTO dbo.ErintesvedelmiModOsztalyok (Id, Nev, Leiras, Aktiv, Sorrend, Letrehozva, VanAvk, VanPeFolyt)
	VALUES (
		@UjId,
		N'EPH',
		N'Kiegyenlítő védelem (EPH) - túláramvédelem és ÁVK nem értelmezhető',
		1,
		@UjSorrend,
		SYSDATETIME(),
		0, -- VanAvk: ÁVK nem csatolható
		1  -- VanPeFolyt: PE folytonosság releváns, alapból bepipálva
	);

	PRINT N'EPH mód/osztály felvéve, Id = ' + CAST(@UjId AS NVARCHAR(10));
END
ELSE
BEGIN
	PRINT N'EPH mód/osztály már létezik, nincs teendő.';
END
