-- Meres.VillamNemNormaAdatokJson oszlop hozzáadása
-- Célja: a "Nem Norma szerinti Villámvédelmi Minősítő Irat" (VillamNemNormaSzerkeszto.razor) saját
-- adatainak tárolása KÜLÖN oszlopban.
--
-- Korábban ez az oldal is a JegyzokonyvAdatokJson oszlopba mentett, amit viszont a főlap
-- (JegyzokonyvSzerkeszto.razor) is használ (beleértve a 2 percenkénti automatikus mentést is).
-- Emiatt ha a felhasználó visszanavigált/újranyitotta a főlapot ugyanarra a mérésre, az ottani
-- mentés (kézi vagy automatikus) felülírta/törölte a villámvédelmi oldalon megadott adatokat.

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'dbo.Meresek') AND name = 'VillamNemNormaAdatokJson'
)
BEGIN
	ALTER TABLE dbo.Meresek ADD VillamNemNormaAdatokJson NVARCHAR(MAX) NULL;
END
GO

-- Egyszeri adatmigráció: azoknál a méréseknél, ahol a JegyzokonyvAdatokJson oszlopban valójában
-- a villámvédelmi (VillamNemNormaAdatok) JSON van elmentve (a hiba miatt felülírt/megmaradt állapot),
-- és a VillamNemNormaAdatokJson még üres, átmásoljuk a tartalmat az új oszlopba.
-- A villámvédelmi JSON felismerhető arról, hogy tartalmazza a "VillamBesorolasSorok" vagy
-- "ElemBesorolasSorok" kulcsot.
UPDATE dbo.Meresek
SET VillamNemNormaAdatokJson = JegyzokonyvAdatokJson
WHERE VillamNemNormaAdatokJson IS NULL
  AND JegyzokonyvAdatokJson IS NOT NULL
  AND (
		JegyzokonyvAdatokJson LIKE '%"VillamBesorolasSorok"%'
	 OR JegyzokonyvAdatokJson LIKE '%"ElemBesorolasSorok"%'
  );
GO
