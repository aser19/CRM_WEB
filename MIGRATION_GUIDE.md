# 🗄️ Adatbázis Migration Útmutató

## ⚡ Gyors Útmutató (5 perc)

### 1️⃣ Nyisd meg az SQL scriptet
- Fájl: `BiztvillCRM.Data\Migrations\MultipleFileSupport.sql`
- Program: SQL Server Management Studio (SSMS)

### 2️⃣ Módosítsd az adatbázis nevet
Az első sorban:
```sql
USE [BiztvillCRM]  -- ← MÓDOSÍTSD a saját adatbázis nevedre!
```

### 3️⃣ Futtasd le a scriptet
- Nyomj `F5`-öt vagy kattints az "Execute" gombra
- Várd meg, amíg befejezi (pár másodperc)

### 4️⃣ Ellenőrizd az eredményt
Futtasd le ezt a query-t:
```sql
SELECT 
	COUNT(*) AS 'Összes hitelesítés',
	SUM(CASE WHEN MunkalapPaths IS NOT NULL THEN 1 ELSE 0 END) AS 'Munkalap JSON',
	SUM(CASE WHEN BizonyitvanyPaths IS NOT NULL THEN 1 ELSE 0 END) AS 'Bizonyítvány JSON'
FROM [dbo].[Hitelesitesek];
```

### 5️⃣ Kész! 🎉
Indítsd újra az alkalmazást és tesztelj!

---

## 📋 Részletes Lépések

### Előfeltételek
- [ ] SQL Server Management Studio telepítve
- [ ] Hozzáférés az adatbázishoz (write jogosultság)
- [ ] **BACKUP ELKÉSZÍTVE!** ⚠️

### Migration futtatása

#### 1. Backup készítése (FONTOS!)
```sql
BACKUP DATABASE [BiztvillCRM] 
TO DISK = N'C:\Backup\BiztvillCRM_BeforeMigration.bak'
WITH FORMAT, INIT, 
NAME = N'BiztvillCRM-Teljes adatbázis biztonsági mentés', 
SKIP, NOREWIND, NOUNLOAD, STATS = 10;
GO
```

#### 2. Migration script futtatása
```powershell
# SSMS-ben:
# 1. File > Open > File...
# 2. Válaszd ki: BiztvillCRM.Data\Migrations\MultipleFileSupport.sql
# 3. Módosítsd: USE [BiztvillCRM] → saját DB
# 4. Execute (F5)
```

#### 3. Ellenőrzés
```sql
-- Új oszlopok léteznek?
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Hitelesitesek'
AND COLUMN_NAME IN ('MunkalapPaths', 'BizonyitvanyPaths');

-- Adatok migrálva?
SELECT TOP 5
	Id, Datum,
	MunkalapPath, MunkalapPaths,
	BizonyitvanyPath, BizonyitvanyPaths
FROM Hitelesitesek
WHERE MunkalapPaths IS NOT NULL OR BizonyitvanyPaths IS NOT NULL;
```

---

## ❓ Mi történik a migration során?

### 1. Új oszlopok létrehozása
```sql
ALTER TABLE [Hitelesitesek] ADD [MunkalapPaths] NVARCHAR(MAX) NULL;
ALTER TABLE [Hitelesitesek] ADD [BizonyitvanyPaths] NVARCHAR(MAX) NULL;
```

### 2. Adatok migrálása
Minden létező `MunkalapPath` → JSON lista formátumba kerül:
```
Régi: "Ceg1/Ugyfel1/munkalap_2024.pdf"
Új:   ["Ceg1/Ugyfel1/munkalap_2024.pdf"]
```

### 3. Migration history frissítése
```sql
INSERT INTO [__EFMigrationsHistory] 
VALUES (N'20241215000000_AddMultipleFileSupportForDocuments', N'8.0.0');
```

---

## 🔄 Rollback (ha vissza kell állítani)

### Teljes rollback
```sql
BEGIN TRANSACTION;

-- Oszlopok törlése
ALTER TABLE [dbo].[Hitelesitesek] DROP COLUMN IF EXISTS [MunkalapPaths];
ALTER TABLE [dbo].[Hitelesitesek] DROP COLUMN IF EXISTS [BizonyitvanyPaths];

-- Migration history törlése
DELETE FROM [__EFMigrationsHistory] 
WHERE [MigrationId] = N'20241215000000_AddMultipleFileSupportForDocuments';

COMMIT TRANSACTION;

PRINT '✓ Rollback befejezve.';
```

### Backup visszaállítása
```sql
USE master;
GO

ALTER DATABASE [BiztvillCRM] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

RESTORE DATABASE [BiztvillCRM] 
FROM DISK = N'C:\Backup\BiztvillCRM_BeforeMigration.bak'
WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5;
GO

ALTER DATABASE [BiztvillCRM] SET MULTI_USER;
GO
```

---

## ⚠️ Gyakori hibák

### Hiba: "Cannot insert duplicate key"
**Ok:** Migration már korábban futott.
**Megoldás:** Script újrafuttatása biztonságos (IF NOT EXISTS ellenőrzések vannak).

### Hiba: "Invalid column name 'MunkalapPaths'"
**Ok:** Alkalmazás újraindítása hiányzik.
**Megoldás:** Állítsd le és indítsd újra az alkalmazást.

### Hiba: "The database ... is currently in use"
**Ok:** Futó kapcsolatok az adatbázison.
**Megoldás:** 
```sql
USE master;
ALTER DATABASE [BiztvillCRM] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- migration script futtatása
ALTER DATABASE [BiztvillCRM] SET MULTI_USER;
```

---

## ✅ Post-Migration Checklist

Miután lefutott a migration:

- [ ] Új oszlopok létrejöttek
- [ ] Régi adatok átkonvertálva JSON-ba
- [ ] Migration history frissítve
- [ ] Alkalmazás újraindítva
- [ ] Fájl feltöltés tesztelve
- [ ] Fájl letöltés tesztelve
- [ ] Státusz jelzések helyesen működnek
- [ ] Régi hitelesítések továbbra is működnek

---

## 📞 Segítség

Ha elakadtál:
1. Ellenőrizd az SQL script outputját (Messages tab)
2. Futtasd le az ellenőrző query-ket
3. Nézd meg a MULTIPLE_FILE_SUPPORT_SUMMARY.md fájlt
4. Készíts rollback-et és próbáld újra

---

**Utolsó frissítés:** 2024-12-15  
**Script verzió:** 1.0
