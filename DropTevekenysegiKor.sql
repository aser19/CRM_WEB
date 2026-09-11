-- Manual schema cleanup: removes the "Tevékenységi kör" (business activity) classification
-- system that was removed from the application code.
-- Run this against the live database after deploying the code changes that removed
-- TevekenysegTipus and the Tevekenyseg/Terulet properties from Ceg, Ugyfel, Gyarto, Jogszabaly.

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cegek]') AND name = 'Tevekenyseg')
BEGIN
	ALTER TABLE [Cegek] DROP COLUMN [Tevekenyseg];
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Ugyfelek]') AND name = 'Tevekenyseg')
BEGIN
	ALTER TABLE [Ugyfelek] DROP COLUMN [Tevekenyseg];
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Gyartok]') AND name = 'Tevekenyseg')
BEGIN
	ALTER TABLE [Gyartok] DROP COLUMN [Tevekenyseg];
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Jogszabalyok]') AND name = 'Terulet')
BEGIN
	ALTER TABLE [Jogszabalyok] DROP COLUMN [Terulet];
END
GO
