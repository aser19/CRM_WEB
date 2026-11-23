-- BCeg_Beolvadas_ACegbe_Migration.sql
-- Célja: egy "B" cég beolvad egy "A" cégbe (pl. 15 év után jogutódlás).
-- A régi jegyzőkönyvek/mérések kiállító adatai (cégnév, bélyegző, logó) nem változhatnak meg utólag,
-- ezért MINDIG 3 lépésben kell futtatni:
--   1) Pillanatkép: minden B cégnek szóló mérésnél, ahol KibocsatoCegId még NULL (azaz eddig
--      a "mindenkori aktuális cég" számított kiállítónak), explicit kitöltjük B id-jával.
--   2) Ügyfelek áthelyezése: Ugyfelek.CegId átírása B-ről A-ra.
--   3) B cég inaktiválása (NEM törlés! A régi Meres.KibocsatoCegId FK-ja még rá mutat).
--
-- Használat: töltsd ki a @BCegId / @ACegId változókat a tényleges Cegek.Id értékekre, majd futtasd.
-- A script idempotens: többszöri futtatás esetén a már átvezetett sorokon nem csinál semmit.

DECLARE @BCegId INT = 0; -- TODO: a beolvadó ("B") cég Id-ja
DECLARE @ACegId INT = 0; -- TODO: a befogadó ("A") cég Id-ja

IF @BCegId = 0 OR @ACegId = 0
BEGIN
	RAISERROR('Kérlek töltsd ki a @BCegId és @ACegId változókat a script futtatása előtt!', 16, 1);
	RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cegek WHERE Id = @BCegId)
BEGIN
	RAISERROR('Nem létezik Ceg a megadott @BCegId azonosítóval.', 16, 1);
	RETURN;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cegek WHERE Id = @ACegId)
BEGIN
	RAISERROR('Nem létezik Ceg a megadott @ACegId azonosítóval.', 16, 1);
	RETURN;
END

BEGIN TRANSACTION;

-- 1) Pillanatkép: a B cég ügyfeleihez tartozó, kiállító cég nélküli mérések "befagyasztása" B-re
UPDATE m
SET m.KibocsatoCegId = @BCegId,
	m.Modositva = GETDATE()
FROM dbo.Meresek m
INNER JOIN dbo.Ugyfelek u ON u.Id = m.UgyfelId
WHERE u.CegId = @BCegId
  AND m.KibocsatoCegId IS NULL;

-- 2) Ügyfelek átvezetése "A" cég alá
UPDATE dbo.Ugyfelek
SET CegId = @ACegId,
	Modositva = GETDATE()
WHERE CegId = @BCegId;

-- 3) "B" cég inaktiválása (megmarad, mert a régi mérések KibocsatoCegId FK-ja rá mutat)
UPDATE dbo.Cegek
SET Aktiv = 0,
	Modositva = GETDATE()
WHERE Id = @BCegId;

COMMIT TRANSACTION;

-- Ellenőrzés: ettől kezdve minden "B"-hez tartozó régi mérésnek legyen kitöltött KibocsatoCegId-ja
-- SELECT COUNT(*) FROM dbo.Meresek m
-- INNER JOIN dbo.Ugyfelek u ON u.Id = m.UgyfelId
-- WHERE u.CegId = @ACegId AND m.KibocsatoCegId IS NULL;
