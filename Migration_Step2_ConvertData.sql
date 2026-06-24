-- ========================================
-- 2. LÉPÉS: ADATOK KONVERTÁLÁSA
-- ========================================
USE [CRM]
GO

PRINT 'LÉPÉS 2: Adatok konvertálása Elvegezve -> Statusz...'

-- Ellenőrizzük, hogy mindkét oszlop létezik
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND name = 'Elvegezve')
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Karbantartasok]') AND name = 'Statusz')
BEGIN
	DECLARE @elvegezveCount INT;
	DECLARE @tervezettCount INT;

	-- Elvegezve = 1 (true) -> Statusz = 2 (Elvegezve)
	UPDATE [dbo].[Karbantartasok]
	SET [Statusz] = 2
	WHERE [Elvegezve] = 1;

	SET @elvegezveCount = @@ROWCOUNT;

	-- Elvegezve = 0 (false) -> Statusz = 0 (Tervezett) - már alapértelmezett
	SELECT @tervezettCount = COUNT(*)
	FROM [dbo].[Karbantartasok]
	WHERE [Elvegezve] = 0;

	PRINT '   ✓ ' + CAST(@elvegezveCount AS VARCHAR) + ' karbantartás "Elvegezve" (2) státuszra állítva';
	PRINT '   ✓ ' + CAST(@tervezettCount AS VARCHAR) + ' karbantartás "Tervezett" (0) státuszban maradt';

	-- Ellenőrzés
	SELECT 
		Statusz,
		COUNT(*) AS Darab
	FROM [dbo].[Karbantartasok]
	GROUP BY Statusz
	ORDER BY Statusz;
END
ELSE
BEGIN
	PRINT '   ! Hiba: Elvegezve vagy Statusz oszlop nem található!';
	PRINT '   ! Futtasd először a Step1 scriptet!';
END

GO
