-- Add Aktiv column to Meresek table
-- This script is idempotent - safe to run multiple times

IF NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
	WHERE TABLE_NAME = 'Meresek' 
	AND COLUMN_NAME = 'Aktiv'
)
BEGIN
	ALTER TABLE Meresek 
	ADD Aktiv BIT NOT NULL DEFAULT 1;

	PRINT 'Aktiv column added to Meresek table with default value 1 (active).';
END
ELSE
BEGIN
	PRINT 'Aktiv column already exists in Meresek table.';
END
GO
