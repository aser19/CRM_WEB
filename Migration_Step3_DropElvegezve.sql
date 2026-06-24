-- ========================================
-- 3. LÉPÉS: ELVEGEZVE OSZLOP TÖRLÉSE
-- ========================================
USE [CRM]
GO

PRINT 'LÉPÉS 3: Elvegezve oszlop eltávolítása...'

IF EXISTS (SELECT * FROM sys.columns 
		   WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') 
		   AND name = 'Elvegezve')
BEGIN
	-- Először töröljük a default constraint-et, ha van
	DECLARE @ConstraintName NVARCHAR(200);

	SELECT @ConstraintName = dc.name
	FROM sys.default_constraints dc
	INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id 
							  AND dc.parent_object_id = c.object_id
	WHERE c.object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') 
	  AND c.name = 'Elvegezve';

	IF @ConstraintName IS NOT NULL
	BEGIN
		DECLARE @DropConstraintSQL NVARCHAR(500);
		SET @DropConstraintSQL = 'ALTER TABLE [dbo].[Karbantartasok] DROP CONSTRAINT [' + @ConstraintName + ']';
		EXEC sp_executesql @DropConstraintSQL;
		PRINT '   ✓ Default constraint eltávolítva';
	END

	-- Most töröljük az oszlopot
	ALTER TABLE [dbo].[Karbantartasok]
	DROP COLUMN [Elvegezve];

	PRINT '   ✓ Elvegezve oszlop sikeresen törölve';
END
ELSE
BEGIN
	PRINT '   ! Elvegezve oszlop már törölve';
END

-- Ellenőrzés
SELECT 
	COLUMN_NAME AS OszlopNev,
	DATA_TYPE AS Tipus
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Karbantartasok' 
  AND COLUMN_NAME IN ('Statusz', 'Elvegezve')
ORDER BY COLUMN_NAME;

GO
