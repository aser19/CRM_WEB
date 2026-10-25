-- SQL szkript a VVN-NEM bizonyítvány KepzesTipusId hibás hozzárendelésének javítására
--
-- Diagnózis (lefuttatott lekérdezések alapján):
--   - KepzesTipusok.Id = 2  -> "Nem norma szerinti Villámvédelmi Felülvizsgáló"              (46 karakter)
--   - KepzesTipusok.Id = 12 -> "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány" (60 karakter)
--   A VillamNemNormaSzerkeszto.razor / Meresek.razor kódja pontosan az Id=12 nevű típust keresi
--   (VvnNemBizonyitvanyTipusNev konstans), de a felhasználók (pl. Vágó Ádám, FelulvizsgaloKepzesekId=20)
--   az Id=2 típushoz töltötték fel a bizonyítványukat, ezért a PDF-generálás nem találja meg.
--
-- Futtatás előtt ellenőrizze az adatbázis nevét és a connection stringet!

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

DECLARE @HelytelenTipusId int = 2;  -- "Nem norma szerinti Villámvédelmi Felülvizsgáló"
DECLARE @HelyesTipusId    int = 12; -- "Nem norma szerinti villámvédelmi felülvizsgálói bizonyítvány"

-- 1. Áttekintés: mely sorok mutatnak a helytelen (Id=2) típusra
SELECT
	fk.Id AS FelulvizsgaloKepzesekId,
	f.Id AS FelulvizsgaloId,
	f.Nev AS FelulvizsgaloNev,
	fk.BizonyitvanyPath,
	fk.Aktiv
FROM [dbo].[FelulvizsgaloKepzesek] fk
INNER JOIN [dbo].[Felulvizsgalok] f ON f.Id = fk.FelulvizsgaloId
WHERE fk.KepzesTipusId = @HelytelenTipusId;
GO

-- 2. Átvezetés: a helytelen (Id=2) típusú sorokat átállítjuk a helyes (Id=12) típusra,
--    DE csak azoknál a felülvizsgálóknál, akiknek még nincs már létező Id=12 típusú sora
--    (a FelulvizsgaloId + KepzesTipusId egyedi index miatt két azonos pár nem létezhet).
DECLARE @HelytelenTipusId2 int = 2;
DECLARE @HelyesTipusId2    int = 12;

UPDATE fk
SET fk.KepzesTipusId = @HelyesTipusId2,
	fk.Modositva = GETUTCDATE()
FROM [dbo].[FelulvizsgaloKepzesek] fk
WHERE fk.KepzesTipusId = @HelytelenTipusId2
  AND NOT EXISTS (
		SELECT 1 FROM [dbo].[FelulvizsgaloKepzesek] fk2
		WHERE fk2.FelulvizsgaloId = fk.FelulvizsgaloId
		  AND fk2.KepzesTipusId = @HelyesTipusId2
  );

PRINT N'Átállított sorok száma: ' + CAST(@@ROWCOUNT AS nvarchar(10));
GO

-- 3. Ha maradt olyan sor, ahol MÁR LÉTEZIK is Id=12 típusú bizonyítvány ugyanahhoz a
--    felülvizsgálóhoz (tehát a 2. lépés nem tudta átállítani az ütközés miatt),
--    ez a lekérdezés megmutatja ezeket - itt kézzel kell eldönteni, melyik sor maradjon,
--    és a másik (Id=2 típusú, duplikált) sort törölni vagy inaktiválni kell.
SELECT
	fk.Id AS FelulvizsgaloKepzesekId,
	f.Id AS FelulvizsgaloId,
	f.Nev AS FelulvizsgaloNev,
	fk.KepzesTipusId,
	fk.BizonyitvanyPath,
	fk.Aktiv
FROM [dbo].[FelulvizsgaloKepzesek] fk
INNER JOIN [dbo].[Felulvizsgalok] f ON f.Id = fk.FelulvizsgaloId
WHERE fk.KepzesTipusId IN (2, 12)
ORDER BY f.Nev, fk.KepzesTipusId;
GO

-- 4. (Opcionális, hosszú távú rendrakás) A duplikált, immár felesleges "Nem norma szerinti
--    Villámvédelmi Felülvizsgáló" (Id=2) KepzesTipus inaktiválása, hogy a jövőben ne lehessen
--    véletlenül ismét ezt választani a dropdown-ból. CSAK akkor futtassa, ha a 2-es lépés
--    minden sort sikeresen átvitt (a 3. lépés eredménye üres vagy nincs Id=2 sor benne)!
/*
UPDATE [dbo].[KepzesTipusok]
SET Aktiv = 0
WHERE Id = 2;
*/
GO
