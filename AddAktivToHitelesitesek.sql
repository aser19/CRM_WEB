-- Add Aktiv column to Hitelesitesek table
-- This script is idempotent - safe to run multiple times

IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Hitelesitesek' 
	AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Hitelesitesek 
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT 'Aktiv column added to Hitelesitesek table with default value 1 (active).';
END
ELSE
BEGIN
	PRINT 'Aktiv column already exists in Hitelesitesek table.';
END
GO
