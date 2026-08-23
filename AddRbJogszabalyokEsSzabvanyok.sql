-- =============================================
-- Rb (robbanásbiztos) felülvizsgálathoz tartozó rendeletek és szabványok
-- felvétele a Jogszabalyok törzstáblába, "Robbanásbiztos" taggel megjelölve.
-- Létrehozva: kézi script (a felhasználó preferenciája szerint, nem EF Core migráció)
-- Futtatás: SQL Server Management Studio / sqlcmd, a CRM adatbázison.
-- =============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

-- =============================================
-- 1. "Robbanásbiztos" tag létrehozása, ha még nem létezik
-- =============================================
DECLARE @RbTagId INT;

SELECT @RbTagId = Id FROM JogszabalyTagek WHERE Nev = N'Robbanásbiztos';

IF @RbTagId IS NULL
BEGIN
	INSERT INTO JogszabalyTagek (Nev, Szin) VALUES (N'Robbanásbiztos', N'#F9A825');
	SET @RbTagId = SCOPE_IDENTITY();
	PRINT 'Robbanásbiztos tag létrehozva, Id: ' + CAST(@RbTagId AS NVARCHAR(10));
END
ELSE
BEGIN
	PRINT 'Robbanásbiztos tag már létezik, Id: ' + CAST(@RbTagId AS NVARCHAR(10));
END

-- =============================================
-- 2. Rendeletek (2.1) felvétele, ha még nincsenek
-- =============================================
-- Segédtábla a rendeletek adataival
DECLARE @Rendeletek TABLE (Szam NVARCHAR(100), Cim NVARCHAR(500));
INSERT INTO @Rendeletek (Szam, Cim) VALUES
	(N'54/2014 (XII.5) BM rendelet', N'az Országos Tűzvédelmi Szabályzat kiadásáról'),
	(N'35/2016. (IX. 27.) NGM rendelet', N'a potenciálisan robbanásveszélyes környezetben történő alkalmazásra szánt berendezések, védelmi rendszerek vizsgálatáról és tanúsításáról'),
	(N'40/2017. (XII. 4.) NGM rendelet', N'az összekötő és felhasználói berendezésekről, valamint a potenciálisan robbanásveszélyes közegben működő villamos berendezésekről és védelmi rendszerekről');

INSERT INTO Jogszabalyok (Letrehozva, Szam, Cim, Tipus, Terulet, Aktiv)
SELECT GETUTCDATE(), r.Szam, r.Cim, 2 /* JogszabalyTipus.Rendelet */, 0 /* TevekenysegTipus.Nincs */, 1
FROM @Rendeletek r
WHERE NOT EXISTS (SELECT 1 FROM Jogszabalyok j WHERE j.Szam = r.Szam);

-- Rendeletek hozzárendelése a Robbanásbiztos taghez
INSERT INTO JogszabalyTagKapcsolatok (JogszabalyId, TagekId)
SELECT j.Id, @RbTagId
FROM Jogszabalyok j
INNER JOIN @Rendeletek r ON r.Szam = j.Szam
WHERE NOT EXISTS (
	SELECT 1 FROM JogszabalyTagKapcsolatok k WHERE k.JogszabalyId = j.Id AND k.TagekId = @RbTagId
);

-- =============================================
-- 3. Szabványok (2.2) felvétele, ha még nincsenek
-- =============================================
DECLARE @Szabvanyok TABLE (Szam NVARCHAR(100), Cim NVARCHAR(500));
INSERT INTO @Szabvanyok (Szam, Cim) VALUES
	(N'MSZ EN IEC 60079-0:2018', N'Robbanóképes közegek. 0. rész: Gyártmányok. Általános követelmények'),
	(N'MSZ EN 60079-1:2015', N'Robbanóképes közegek. 1. rész: Készülékek védelme "d" nyomásálló tokozással'),
	(N'MSZ EN 60079-7:2016', N'Robbanóképes közegek. 7. rész: Gyártmányok védelme fokozott biztonsággal, "e"'),
	(N'MSZ EN 60079-11:2012', N'Robbanóképes közegek. 11. rész: Gyártmányok gyújtószikramentes védelemmel "i"'),
	(N'MSZ EN 60079-14:2014', N'Robbanóképes közegek. 14. rész: Villamos berendezések tervezése, kiválasztása és szerelése'),
	(N'MSZ EN 60079-15:2011', N'Robbanóképes közegek. 15. rész: Gyártmányok védelme "n" típusú védelemmel'),
	(N'MSZ EN 60079-17:2014', N'Robbanóképes közegek. 17. rész: Villamos berendezések felülvizsgálata és karbantartása'),
	(N'MSZ EN 60079-25:2011', N'Robbanóképes közegek. 25. rész: Gyújtószikramentes villamos rendszerek'),
	(N'MSZ HD 60364 szabványsorozat', N'létesítéssel kapcsolatos fejezetei'),
	(N'MSZ EN 1127-1:2019', N'Robbanóképes közegek. Robbanásmegelőzés és robbanásvédelem. 1. rész: Alapelvek és módszertan'),
	(N'MSZ EN 13237:2013', N'Potenciálisan robbanásveszélyes közegek. A potenciálisan robbanásveszélyes közegekben való használatra tervezett berendezések és védelmi rendszerek szakkifejezései és meghatározásaik.'),
	(N'MSZ EN ISO 80079-36:2016', N'Robbanóképes közegek. 36. rész: Robbanóképes közegekben használt nem villamos berendezések. Alapmódszer és követelmények'),
	(N'MSZ EN ISO 80079-37:2016', N'Robbanóképes közegek. 37. rész: Robbanóképes közegekben használt nem villamos berendezések. Nem villamos szerkezetbiztonsági védelem "c", védelem a gyújtóforrás ellenőrzésével "b", folyadék alatti védelem "k"');

INSERT INTO Jogszabalyok (Letrehozva, Szam, Cim, Tipus, Terulet, Aktiv)
SELECT GETUTCDATE(), s.Szam, s.Cim, 1 /* JogszabalyTipus.Szabvany */, 0 /* TevekenysegTipus.Nincs */, 1
FROM @Szabvanyok s
WHERE NOT EXISTS (SELECT 1 FROM Jogszabalyok j WHERE j.Szam = s.Szam);

-- Szabványok hozzárendelése a Robbanásbiztos taghez
INSERT INTO JogszabalyTagKapcsolatok (JogszabalyId, TagekId)
SELECT j.Id, @RbTagId
FROM Jogszabalyok j
INNER JOIN @Szabvanyok s ON s.Szam = j.Szam
WHERE NOT EXISTS (
	SELECT 1 FROM JogszabalyTagKapcsolatok k WHERE k.JogszabalyId = j.Id AND k.TagekId = @RbTagId
);

COMMIT TRANSACTION;
PRINT 'Kész: Robbanásbiztos rendeletek és szabványok felvéve, Robbanásbiztos taggel megjelölve.';
