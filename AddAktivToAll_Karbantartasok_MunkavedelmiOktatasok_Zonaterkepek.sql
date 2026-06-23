-- Összesítő SQL script: Aktiv oszlop hozzáadása a Karbantartasok, MunkavedelmiOktatasok és Zonaterkepek táblákhoz
-- Futtasd ezt a scriptet a távoli SQL Serveren

PRINT '========================================';
PRINT 'Aktiv oszlopok hozzáadása';
PRINT '========================================';
PRINT '';

-- 1. Karbantartasok tábla
PRINT '1. Karbantartasok tábla ellenőrzése...';
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Karbantartasok' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Karbantartasok
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT '   ✓ Aktiv oszlop sikeresen hozzáadva a Karbantartasok táblához.';
END
ELSE
BEGIN
	PRINT '   ℹ Az Aktiv oszlop már létezik a Karbantartasok táblában.';
END
PRINT '';

-- 2. MunkavedelmiOktatasok tábla
PRINT '2. MunkavedelmiOktatasok tábla ellenőrzése...';
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'MunkavedelmiOktatasok' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE MunkavedelmiOktatasok
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT '   ✓ Aktiv oszlop sikeresen hozzáadva a MunkavedelmiOktatasok táblához.';
END
ELSE
BEGIN
	PRINT '   ℹ Az Aktiv oszlop már létezik a MunkavedelmiOktatasok táblában.';
END
PRINT '';

-- 3. Zonaterkepek tábla
PRINT '3. Zonaterkepek tábla ellenőrzése...';
IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Zonaterkepek' AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Zonaterkepek
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT '   ✓ Aktiv oszlop sikeresen hozzáadva a Zonaterkepek táblához.';
END
ELSE
BEGIN
	PRINT '   ℹ Az Aktiv oszlop már létezik a Zonaterkepek táblában.';
END
PRINT '';

PRINT '========================================';
PRINT 'Script befejezve';
PRINT '========================================';
GO
