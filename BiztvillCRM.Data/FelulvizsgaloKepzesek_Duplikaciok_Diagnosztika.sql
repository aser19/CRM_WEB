-- Diagnosztika: miért jelenik meg a felületen 3x a "Nem norma szerinti villámvédelmi
-- felülvizsgálói bizonyítvány" képzés Vágó Ádámnál (Felulvizsgalo.Id = 4), miközben
-- az adatbázisban elsőre csak 1 sort látni.
--
-- A felület (Felulvizsgalok.razor részletek dialógusa) egyszerűen a
-- Felulvizsgalo.Kepzesek navigációs gyűjteményt listázza ki (1 MudExpansionPanel / sor),
-- szűrés és csoportosítás NÉLKÜL. Ha a felületen 3x jelenik meg ugyanaz a típus
-- ugyanazzal a bizonyítványszámmal (P94 B 157890), az szinte biztosan azt jelenti,
-- hogy a FelulvizsgaloKepzesek táblában TÉNYLEGESEN 3 külön sor létezik ehhez a
-- felülvizsgálóhoz és képzéstípushoz - csak az előző szűkített lekérdezés nem fogta
-- be mindet (pl. mert időközben új sorok kerültek be, vagy a KepzesTipusId nem
-- egyezett pontosan a kereséssel).
--
-- Futtatás előtt ellenőrizze az adatbázis nevét!

USE [BiztvillCRM_Database]; -- <-- Módosítsa a saját adatbázis nevére!
GO

-- 1. Vágó Ádám (Id=4) ÖSSZES FelulvizsgaloKepzesek sorának kilistázása, típusnévvel együtt,
--    hogy pontosan lássuk, hány sor és milyen KepzesTipusId/Id values tartoznak hozzá.
SELECT
	fk.Id,
	fk.FelulvizsgaloId,
	fk.KepzesTipusId,
	kt.Nev AS KepzesTipusNev,
	fk.BizonyitvanySzam,
	fk.BizonyitvanyPath,
	fk.Aktiv,
	fk.Letrehozva,
	fk.Modositva
FROM [dbo].[FelulvizsgaloKepzesek] fk
LEFT JOIN [dbo].[KepzesTipusok] kt ON kt.Id = fk.KepzesTipusId
WHERE fk.FelulvizsgaloId = 4
ORDER BY fk.KepzesTipusId, fk.Id;
GO

-- 2. Kifejezetten azokra a sorokra szűrve, amelyek a két hasonló nevű "Nem norma..."
--    típushoz (Id=2 és Id=12) tartoznak, Vágó Ádámnál.
SELECT
	fk.Id,
	fk.KepzesTipusId,
	kt.Nev AS KepzesTipusNev,
	fk.BizonyitvanySzam,
	fk.BizonyitvanyPath,
	fk.Aktiv
FROM [dbo].[FelulvizsgaloKepzesek] fk
LEFT JOIN [dbo].[KepzesTipusok] kt ON kt.Id = fk.KepzesTipusId
WHERE fk.FelulvizsgaloId = 4
  AND fk.KepzesTipusId IN (2, 12);
GO

-- 3. Duplikátumok keresése: ugyanahhoz a felülvizsgálóhoz és KepzesTipusId-hoz
--    tartozó, egynél több aktív sor (ez lenne a tényleges duplikáció oka).
SELECT
	FelulvizsgaloId,
	KepzesTipusId,
	COUNT(*) AS SorokSzama
FROM [dbo].[FelulvizsgaloKepzesek]
WHERE FelulvizsgaloId = 4
GROUP BY FelulvizsgaloId, KepzesTipusId
HAVING COUNT(*) > 1;
GO

-- 4. Fontos: a FelulvizsgaloService.GetByIdWithDetailsAsync() a Kepzesek mellett
--    a Kepzesek.Tovabbkepzesek (KepzesTovabbkepzesek) gyűjteményt is ThenInclude-olja.
--    Ha az Id=20 képzéshez TÖBB mint 1 Tovabbkepzesek-sor tartozik, és a lekérdezés
--    valahol AsNoTracking()-ként/split query nélkül fut, az EF Core cartesian
--    join miatt a Kepzesek listában TÖBBSZÖR szerepelhet ugyanaz az Id=20 sor
--    (annyiszor, ahány Tovabbkepzesek-sora van), miközben a táblában csak 1 sor van.
SELECT
	kt.Id AS TovabbkepzesId,
	kt.FelulvizsgaloKepzesId,
	kt.Datum,
	kt.BizonyitvanySzam,
	kt.Hely,
	kt.Megjegyzes
FROM [dbo].[KepzesTovabbkepzesek] kt
WHERE kt.FelulvizsgaloKepzesId = 20;
GO
