-- Az RbVedelmiModok tábla kibővítése strukturált mezőkkel:
-- Gazcsoport (IIA/IIB/IIC), Porcsoport (IIIA/IIIB/IIIC), HomersOsztaly (T1-T6 vagy °C érték),
-- EngedelyezettZonak (vesszővel elválasztott zóna lista, pl. "1,2,21,22").
-- Ezeket az admin egyszer tölti ki / hagyja jóvá egy adott Védelmi mód névhez,
-- így a rendszer megbízhatóan visszakeresi ahelyett, hogy minden alkalommal a szabad szöveges
-- Védelmi mód jelölést kellene elemezni.

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[RbVedelmiModok]') AND name = 'Gazcsoport')
BEGIN
	ALTER TABLE [RbVedelmiModok] ADD [Gazcsoport] NVARCHAR(10) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[RbVedelmiModok]') AND name = 'Porcsoport')
BEGIN
	ALTER TABLE [RbVedelmiModok] ADD [Porcsoport] NVARCHAR(10) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[RbVedelmiModok]') AND name = 'HomersOsztaly')
BEGIN
	ALTER TABLE [RbVedelmiModok] ADD [HomersOsztaly] NVARCHAR(20) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[RbVedelmiModok]') AND name = 'EngedelyezettZonak')
BEGIN
	ALTER TABLE [RbVedelmiModok] ADD [EngedelyezettZonak] NVARCHAR(100) NULL;
END
GO
