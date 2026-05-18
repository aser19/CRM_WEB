using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using BiztvillCRM.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BiztvillCRM.Services;

public interface IJegyzokonyvOcrService
{
    Task<JegyzokonyvImportAdatok> FeldolgozAsync(
        Stream fileStream, 
        string fileName, 
        string? customModelId = null);
    
    Task<JegyzokonyvImportAdatok> FeldolgozHierarchikusAsync(
        Stream fileStream, 
        string fileName, 
        string? customModelId = null);
}

public class JegyzokonyvOcrService : IJegyzokonyvOcrService
{
    private readonly DocumentAnalysisClient _client;
    private readonly ILogger<JegyzokonyvOcrService> _logger;
    private readonly string _defaultFallbackModel = "prebuilt-document";

    public JegyzokonyvOcrService(IConfiguration configuration, ILogger<JegyzokonyvOcrService> logger)
    {
        var endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
        var apiKey = configuration["AzureDocumentIntelligence:ApiKey"];
        
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Azure Document Intelligence konfiguráció hiányzik!");
        }

        _client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger = logger;
    }

    // ✅ NORMÁL FELDOLGOZÁS
    public async Task<JegyzokonyvImportAdatok> FeldolgozAsync(
        Stream fileStream,
        string fileName,
        string? customModelId = null)
    {
        var importAdatok = new JegyzokonyvImportAdatok();

        // Teljes tartalom byte[]-ba mentve – így bármikor új stream készíthető
        var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

        try
        {
            _logger.LogInformation("OCR feldolgozás indítása: {FileName}", fileName);

            string modelId = !string.IsNullOrWhiteSpace(customModelId)
                ? customModelId
                : _defaultFallbackModel;

            AnalyzeResult result;

            try
            {
                _logger.LogInformation("📋 OCR model használata: {ModelId}", modelId);

                using var buffer1 = new MemoryStream(fileBytes);
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed, modelId, buffer1);

                result = operation.Value;
            }
            catch (RequestFailedException azureEx) when (azureEx.Status == 404 && modelId != _defaultFallbackModel)
            {
                _logger.LogWarning("⚠️ Custom model '{ModelId}' nem található, fallback: {FallbackModel}",
                    modelId, _defaultFallbackModel);

                // Friss stream a fallback híváshoz
                using var buffer2 = new MemoryStream(fileBytes);
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed, _defaultFallbackModel, buffer2);

                result = operation.Value;
                _logger.LogInformation("✅ Fallback model sikeresen használva");
            }

            _logger.LogInformation("📄 Talált oldalak: {PageCount}", result.Pages.Count);
            _logger.LogInformation("📋 Talált táblázatok: {TableCount}", result.Tables.Count);

            // Ha kevés táblázat, próbáljuk újra prebuilt-layout-tal (csak HME esetén)
            if (result.Tables.Count < 2 && result.Pages.Count > 2 && modelId == "HME")
            {
                _logger.LogWarning("⚠️ Kevés táblázat ({TableCount}), újrapróbálás prebuilt-layout-tal",
                    result.Tables.Count);

                using var bufferRetry = new MemoryStream(fileBytes);
                var retryOperation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed, "prebuilt-layout", bufferRetry);

                result = retryOperation.Value;
                _logger.LogInformation("📋 Újrapróbálás után: {TableCount} táblázat", result.Tables.Count);
            }

            FeldolgozTeljesSzoveg(result.Content, importAdatok);
            FeldolgozOldalanként(result, importAdatok);

            if (result.Tables.Count > 0)
            {
                _logger.LogInformation("🔍 Táblázatok feldolgozása...");

                if (modelId == "AVK")
                {
                    var avkSikerult = FeldolgozAvkTablazat(result.Tables, importAdatok);
                    if (!avkSikerult)
                        FeldolgozEszkozTablazat(result.Tables, importAdatok);
                }
                else if (modelId == "HVM")
                {
                    var hvmSikerult = FeldolgozMeresiPontTablazat(result.Tables, importAdatok);
                    if (!hvmSikerult)
                        _logger.LogWarning("⚠️ Nem sikerült HVM táblázatot felismerni.");
                }
                else
                {
                    FeldolgozEszkozTablazat(result.Tables, importAdatok);
                }
            }

            _logger.LogInformation("✅ Feldolgozás sikeres. {EszkozokSzama} eszköz",
                importAdatok.Eszkozok.Count);

            LogTalaltAdatok(importAdatok);
            return importAdatok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Hiba az OCR feldolgozás során: {FileName}", fileName);
            throw new ApplicationException($"OCR feldolgozás sikertelen: {ex.Message}", ex);
        }
        finally
        {
            await ms.DisposeAsync();
        }
    }

    // ✅ HIERARCHIKUS FELDOLGOZÁS
    public async Task<JegyzokonyvImportAdatok> FeldolgozHierarchikusAsync(
        Stream fileStream,
        string fileName,
        string? customModelId = null)
    {
        var importAdatok = new JegyzokonyvImportAdatok();

        try
        {
            _logger.LogInformation("🔄 Hierarchikus OCR feldolgozás indítása: {FileName}", fileName);

            string modelId = !string.IsNullOrWhiteSpace(customModelId)
                ? customModelId
                : _defaultFallbackModel;

            AnalyzeResult result;

            try
            {
                _logger.LogInformation("📋 OCR model használata: {ModelId}", modelId);

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    fileStream);

                result = operation.Value;
            }
            catch (RequestFailedException azureEx) when (azureEx.Status == 404 && modelId != _defaultFallbackModel)
            {
                _logger.LogWarning("⚠️ Custom model '{ModelId}' nem található, fallback: {FallbackModel}",
                    modelId, _defaultFallbackModel);

                fileStream.Position = 0;

                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    _defaultFallbackModel,
                    fileStream);

                result = operation.Value;
            }

            FeldolgozHierarchikusSzoveg(result.Content, importAdatok);

            if (result.Tables.Count > 0)
            {
                FeldolgozHierarchikusTablazat(result.Tables, importAdatok);
            }

            _logger.LogInformation("✅ Hierarchikus feldolgozás sikeres. {EszkozokSzama} eszköz (alkatrészekkel)",
                importAdatok.Eszkozok.Count);

            return importAdatok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Hiba a hierarchikus OCR feldolgozás során: {FileName}", fileName);
            throw;
        }
    }

    // === PRIVATE METÓDUSOK ===

    private void FeldolgozEszkozTablazat(IReadOnlyList<DocumentTable> tables, JegyzokonyvImportAdatok adatok)
    {
        Dictionary<int, string>? korabbanFelismertFejlec = null; // ✅ Első táblázat fejléce
        
        for (int tableIndex = 0; tableIndex < tables.Count; tableIndex++)
        {
            var table = tables[tableIndex];
            
            _logger.LogInformation("📊 Táblázat #{Index}: {RowCount} sor, {ColumnCount} oszlop", 
                tableIndex + 1, table.RowCount, table.ColumnCount);

            if (table.RowCount < 1) // ✅ JAVÍTVA: 2-ről 1-re (lehet 1 soros is)
            {
                _logger.LogWarning("⚠️ Táblázat #{Index} üres, átugorva", tableIndex + 1);
                continue;
            }

            var felismertFejlec = FeldolgozMergedTablazat(table, adatok, korabbanFelismertFejlec);
            
            // ✅ Ha találtunk fejlécet, eltároljuk következő táblázathoz
            if (felismertFejlec != null && felismertFejlec.Any())
            {
                korabbanFelismertFejlec = felismertFejlec;
            }
        }
    }

    // ✅ Módosított return type és paraméter
    private Dictionary<int, string>? FeldolgozMergedTablazat(
        DocumentTable table, 
        JegyzokonyvImportAdatok adatok,
        Dictionary<int, string>? eloredefinedFejlec = null)
    {
        _logger.LogInformation("  🔧 Merged táblázat feldolgozása... ({RowCount} sor, {ColCount} oszlop)", 
            table.RowCount, table.ColumnCount);
        
        // ✅ DEBUG: Első 2 sor tartalmának kiírása
        _logger.LogInformation("    🔍 DEBUG: Első 2 sor tartalma:");
        for (int debugRow = 0; debugRow < Math.Min(2, table.RowCount); debugRow++)
        {
            var debugCells = table.Cells
                .Where(c => c.RowIndex == debugRow)
                .OrderBy(c => c.ColumnIndex)
                .Select(c => $"[{c.ColumnIndex}]='{c.Content}'")
                .ToList();
            _logger.LogInformation("      Sor {Row}: {Cells}", debugRow, string.Join(" | ", debugCells));
        }
        
        var fejlecOszlopok = new Dictionary<int, string>();
        int startRow = 0;
        
        if (eloredefinedFejlec != null && eloredefinedFejlec.Any())
        {
            fejlecOszlopok = new Dictionary<int, string>(eloredefinedFejlec); // ✅ JAVÍTVA: másolat
            startRow = 0;
            _logger.LogInformation("    📋 Korábbi táblázat fejlécét használjuk");
        }
        else
        {
            // Fejléc felismerése az első sorból
            for (int col = 0; col < table.ColumnCount; col++)
            {
                var fejlecCell = table.Cells.FirstOrDefault(c => c.RowIndex == 0 && c.ColumnIndex == col);
                if (fejlecCell != null && !string.IsNullOrWhiteSpace(fejlecCell.Content))
                {
                    var content = fejlecCell.Content.Trim().ToLower();
                    fejlecOszlopok[col] = content;
                    _logger.LogDebug("    📋 Oszlop {Col}: '{Header}'", col, fejlecCell.Content);
                }
            }
            
            // ✅ JAVÍTÁS: Bővített fejléc felismerés
            bool vanFejlec = fejlecOszlopok.Values.Any(h => 
                h.Contains("megnevez") ||   // "megnevezés", "megneveze"
                h.Contains("eszköz") ||
                h.Contains("munkaeszköz") ||
                h.Contains("típus") || 
                h.Contains("tipus") ||
                h.Contains("type") ||
                h.Contains("gyári") ||
                h.Contains("gyártási") ||
                h.Contains("serial"));
            
            // ✅ JAVÍTÁS: Ha az első sor "Típus: XXX" mintát tartalmaz, az ADAT, nem fejléc!
            bool elsoSorAdatMinta = fejlecOszlopok.Values.Any(h => 
                h.Contains("típus:") || 
                h.Contains("gyári szám:") ||
                Regex.IsMatch(h, @"^\d+$")); // csak szám
            
            if (elsoSorAdatMinta)
            {
                vanFejlec = false;
                _logger.LogWarning("    ⚠️ Fejléc helyett ADATOK az első sorban! (pl. 'Típus: XXX')");
            }
            
            if (vanFejlec && !elsoSorAdatMinta)
            {
                startRow = 1;
                _logger.LogInformation("    ✅ Fejléc felismerve, adatok: 1. sortól");
            }
            else
            {
                startRow = 0;
                _logger.LogWarning("    ⚠️ Nincs fejléc! Minden sort adatként dolgozunk fel.");
                _logger.LogWarning("    ⚠️ Használunk ALAPÉRTELMEZETT oszloppozíciókat...");
                
                // ✅ JAVÍTÁS: Oszloppozíció alapú feltételezés
                fejlecOszlopok.Clear();
                
                if (table.ColumnCount >= 10)
                {
                    // Teljes táblázat struktúra
                    fejlecOszlopok[0] = "sorszám";
                    fejlecOszlopok[1] = "megnevezés";
                    fejlecOszlopok[2] = "típus";
                    fejlecOszlopok[3] = "gyári szám";
                    fejlecOszlopok[4] = "jellemző";
                    fejlecOszlopok[5] = "egyéb";
                    fejlecOszlopok[6] = "megtekintés";
                    fejlecOszlopok[7] = "folytonosság";
                    fejlecOszlopok[8] = "szigetelés";
                    fejlecOszlopok[9] = "szivárgó áram";
                    if (table.ColumnCount >= 11)
                        fejlecOszlopok[10] = "megjegyzés";
                    _logger.LogInformation("    📊 Teljes táblázat struktúra feltételezve (11 oszlop)");
                }
                else if (table.ColumnCount <= 4)
                {
                    // Egyszerű táblázat (csak Típus + Gyári)
                    fejlecOszlopok[0] = "típus";
                    fejlecOszlopok[1] = "gyári szám";
                    if (table.ColumnCount >= 3)
                        fejlecOszlopok[2] = "egyéb";
                    _logger.LogInformation("    📊 Egyszerű táblázat struktúra (3-4 oszlop)");
                }
                else
                {
                    // Közepes táblázat (5-9 oszlop)
                    fejlecOszlopok[0] = "megnevezés";
                    fejlecOszlopok[1] = "típus";
                    fejlecOszlopok[2] = "gyári szám";
                    fejlecOszlopok[3] = "jellemző";
                    if (table.ColumnCount >= 5)
                        fejlecOszlopok[4] = "megtekintés";
                    if (table.ColumnCount >= 6)
                        fejlecOszlopok[5] = "folytonosság";
                    _logger.LogInformation("    📊 Közepes táblázat struktúra ({ColCount} oszlop)", table.ColumnCount);
                }
            }
        }
        
        // ✅ JAVÍTÁS: Bővített szinonimaok
        int? megnevezesCol = TalalOszlop(fejlecOszlopok, 
            "megnevezés", "megneveze", "megnevezé",
            "eszköz", "munkaeszköz", "eszköz neve",
            "név", "name");
        
        int? tipusCol = TalalOszlop(fejlecOszlopok, 
            "típus", "tipus", "type", "model");
        
        int? gyariCol = TalalOszlop(fejlecOszlopok, 
            "gyári", "gyártási", "serial", "gyári szám", "gyártási szám",
            "szám", "sn");
        
        int? leltariCol = TalalOszlop(fejlecOszlopok, 
            "leltári", "leltári szám", "inventory");
        
        int? teljCol = TalalOszlop(fejlecOszlopok, 
            "teljesítmény", "feszültség", "jellemző", "egyéb", 
            "voltage", "power", "jellemző teljesítmény");
        
        int? megtekintCol = TalalOszlop(fejlecOszlopok, 
            "megtekintés", "szemrevétel", "vizuális", "vizsgálat", 
            "megfelelés", "megfelel", "visual");
        
        int? folytCol = TalalOszlop(fejlecOszlopok, 
            "folytonosság", "folyt", "continuity");
        
        int? szigellCol = TalalOszlop(fejlecOszlopok, 
            "szigetel", "szig", "szigetelés", "insulation");
        
        int? szivargoCol = TalalOszlop(fejlecOszlopok, 
            "szivárgó", "áram", "leakage", "szivárgó áram");
        
        int? megjegyzesCol = TalalOszlop(fejlecOszlopok, 
            "megjegyzés", "jegyzet", "comment", "notes");

        _logger.LogInformation("    🔍 Felismert oszlopok: Név={Nev}, Típus={Tipus}, Gyári={Gyari}, Telj={Telj}, Megtekint={Megtekint}, Folyt={Folyt}, Szigell={Szigell}, Szivargo={Szivargo}", 
            megnevezesCol, tipusCol, gyariCol, teljCol, megtekintCol, folytCol, szigellCol, szivargoCol);
        
        // ✅ Adatsorok feldolgozása
        for (int row = startRow; row < table.RowCount; row++)
        {
            var cellak = table.Cells
                .Where(c => c.RowIndex == row)
                .OrderBy(c => c.ColumnIndex)
                .ToList();

            if (cellak.Count < 1) // ✅ JAVÍTVA: 2-ről 1-re
            {
                _logger.LogDebug("      ⚠️ Sor {Row} üres", row);
                continue;
            }

            var eszkoz = new HordozhatoEszkozImport();
            var sorAdatok = new Dictionary<int, string>();
            
            foreach (var cell in cellak)
            {
                var content = cell.Content?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(content))
                {
                    sorAdatok[cell.ColumnIndex] = content;
                }
            }
            
            // ✅ JAVÍTÁS: Részletes debug logging
            _logger.LogDebug("      📝 Sor {Row}: {Cellak} cella -> {Data}", 
                row, sorAdatok.Count, 
                string.Join(" | ", sorAdatok.Select(kv => $"[{kv.Key}]='{kv.Value}'")));
            
            // Adatok kinyerése felismert oszlopokból
            if (megnevezesCol.HasValue && sorAdatok.ContainsKey(megnevezesCol.Value))
            {
                eszkoz.Eszkoznev = sorAdatok[megnevezesCol.Value];
            }

            if (tipusCol.HasValue && sorAdatok.ContainsKey(tipusCol.Value))
            {
                var tipus = sorAdatok[tipusCol.Value];
                // ✅ JAVÍTÁS: Távolítsuk el a "Típus:" prefix-et
                if (tipus.StartsWith("Típus:", StringComparison.OrdinalIgnoreCase))
                    tipus = tipus.Replace("Típus:", "", StringComparison.OrdinalIgnoreCase).Trim();
                eszkoz.Tipus = tipus;
            }

            if (gyariCol.HasValue && sorAdatok.ContainsKey(gyariCol.Value))
            {
                var gyari = sorAdatok[gyariCol.Value];
                // ✅ JAVÍTÁS: Távolítsuk el a "Gyári szám:" prefix-et
                if (gyari.StartsWith("Gyári szám:", StringComparison.OrdinalIgnoreCase))
                    gyari = gyari.Replace("Gyári szám:", "", StringComparison.OrdinalIgnoreCase).Trim();
                eszkoz.GyariSzam = gyari;
            }

            if (leltariCol.HasValue && sorAdatok.ContainsKey(leltariCol.Value))
            {
                eszkoz.Leltariszam = sorAdatok[leltariCol.Value];
            }

            if (teljCol.HasValue && sorAdatok.ContainsKey(teljCol.Value))
            {
                var telj = sorAdatok[teljCol.Value];
                if (telj.Contains("230"))
                    eszkoz.JellemzoTeljesitmeny = "230V";
                else if (telj.Contains("400"))
                    eszkoz.JellemzoTeljesitmeny = "400V";
                else if (telj.Contains("12"))
                    eszkoz.JellemzoTeljesitmeny = "12V";
                else if (telj.Contains("24"))
                    eszkoz.JellemzoTeljesitmeny = "24V";
                else
                    eszkoz.JellemzoTeljesitmeny = telj;
            }

            if (megtekintCol.HasValue && sorAdatok.ContainsKey(megtekintCol.Value))
            {
                var megtekint = sorAdatok[megtekintCol.Value].ToUpper();
                if (megtekint.Contains("MEGFELELT") || megtekint == "MF")
                    eszkoz.Megtekintes = "MF";
                else if (megtekint.Contains("NEM") || megtekint == "NMF")
                    eszkoz.Megtekintes = "NMF";
                else if (megtekint.Contains("KSZ"))
                    eszkoz.Megtekintes = "KSZ";
            }

            if (folytCol.HasValue && sorAdatok.ContainsKey(folytCol.Value))
            {
                var folyt = sorAdatok[folytCol.Value];
                if (!string.IsNullOrWhiteSpace(folyt) && folyt != "KSZ" && folyt != "-")
                    eszkoz.Folytonossag = folyt;
            }

            if (szigellCol.HasValue && sorAdatok.ContainsKey(szigellCol.Value))
            {
                var szigell = sorAdatok[szigellCol.Value];
                if (!string.IsNullOrWhiteSpace(szigell) && szigell != "-")
                    eszkoz.Szigeteles = szigell;
            }

            if (szivargoCol.HasValue && sorAdatok.ContainsKey(szivargoCol.Value))
            {
                var szivargo = sorAdatok[szivargoCol.Value];
                if (!string.IsNullOrWhiteSpace(szivargo) && szivargo != "-")
                    eszkoz.SzivargoAram = szivargo;
            }

            if (megjegyzesCol.HasValue && sorAdatok.ContainsKey(megjegyzesCol.Value))
            {
                var megjegyzes = sorAdatok[megjegyzesCol.Value];
                if (!string.IsNullOrWhiteSpace(megjegyzes) && !megjegyzes.Contains("MEGFELELT"))
                    eszkoz.Megjegyzes = megjegyzes;
            }
            
            // ✅ FALLBACK: "Típus:" "Gyári szám:" minta
            if (string.IsNullOrEmpty(eszkoz.Tipus) && string.IsNullOrEmpty(eszkoz.GyariSzam))
            {
                foreach (var (colIndex, content) in sorAdatok)
                {
                    if (content.StartsWith("Típus:", StringComparison.OrdinalIgnoreCase))
                    {
                        eszkoz.Tipus = content.Replace("Típus:", "", StringComparison.OrdinalIgnoreCase).Trim();
                    }
                    else if (content.StartsWith("Gyári szám:", StringComparison.OrdinalIgnoreCase))
                    {
                        eszkoz.GyariSzam = content.Replace("Gyári szám:", "", StringComparison.OrdinalIgnoreCase).Trim();
                    }
                    else if (string.IsNullOrEmpty(eszkoz.Eszkoznev) && !content.Contains(":"))
                    {
                        eszkoz.Eszkoznev = content;
                    }
                }
            }
            
            // ✅ FALLBACK: Sorszámok kiszűrése
            if (!string.IsNullOrEmpty(eszkoz.Eszkoznev) && Regex.IsMatch(eszkoz.Eszkoznev, @"^\d+\.$"))
            {
                _logger.LogDebug("      ⚠️ Csak sorszám ({Sorsz}), átugorva", eszkoz.Eszkoznev);
                continue; // Ne importáljuk a puszta sorszámokat
            }
            
            // ✅ JAVÍTÁS: Ha nincs név, de van típus, használjuk a típust (NE adjunk prefix-et!)
            if (string.IsNullOrEmpty(eszkoz.Eszkoznev))
            {
                if (!string.IsNullOrEmpty(eszkoz.Tipus))
                {
                    eszkoz.Eszkoznev = eszkoz.Tipus; // ✅ JAVÍTVA: NE "Eszköz (típus)", csak típus
                    _logger.LogDebug("      📝 Megnevezés hiányzik, típus használata: {Tipus}", eszkoz.Tipus);
                }
                else if (sorAdatok.Any())
                {
                    var elsoErtek = sorAdatok.Values.FirstOrDefault(v => 
                        !string.IsNullOrWhiteSpace(v) && 
                        !Regex.IsMatch(v, @"^\d+\.$") &&
                        !v.Contains(":"));
                    if (!string.IsNullOrEmpty(elsoErtek))
                    {
                        eszkoz.Eszkoznev = elsoErtek;
                    }
                }
            }

            // ✅ Validálás és mentés
            bool vanErvenyes = !string.IsNullOrWhiteSpace(eszkoz.Eszkoznev) || 
                              !string.IsNullOrWhiteSpace(eszkoz.GyariSzam) ||
                              !string.IsNullOrWhiteSpace(eszkoz.Tipus);
            
            if (vanErvenyes)
            {
                // ✅ JAVÍTÁS: Duplikáció ellenőrzés - ELSŐSORBAN gyári szám alapján
                bool marLetezik = false;
    
                if (!string.IsNullOrEmpty(eszkoz.GyariSzam))
                {
                    // Elsősorban gyári szám alapján
                    marLetezik = adatok.Eszkozok.Any(e => 
                        !string.IsNullOrEmpty(e.GyariSzam) && 
                        e.GyariSzam.Equals(eszkoz.GyariSzam, StringComparison.OrdinalIgnoreCase));
                }
                else if (!string.IsNullOrEmpty(eszkoz.Eszkoznev) && !string.IsNullOrEmpty(eszkoz.Tipus))
                {
                    // Ha nincs gyári szám, név + típus kombinációval
                    marLetezik = adatok.Eszkozok.Any(e => 
                        !string.IsNullOrEmpty(e.Eszkoznev) && 
                        !string.IsNullOrEmpty(e.Tipus) &&
                        e.Eszkoznev.Equals(eszkoz.Eszkoznev, StringComparison.OrdinalIgnoreCase) &&
                        e.Tipus.Equals(eszkoz.Tipus, StringComparison.OrdinalIgnoreCase));
                }
    
                if (!marLetezik)
                {
                    adatok.Eszkozok.Add(eszkoz);
                    _logger.LogInformation("      ✅ Eszköz hozzáadva: {Nev} | Típus: {Tipus} | Gyári: {Gyari}", 
                        eszkoz.Eszkoznev ?? "(névtelen)", 
                        eszkoz.Tipus ?? "-", 
                        eszkoz.GyariSzam ?? "-");
                }
                else
                {
                    _logger.LogDebug("      ⚠️ Duplikáció, átugorva: {Nev} (Gyári: {Gyari})", 
                        eszkoz.Eszkoznev, eszkoz.GyariSzam);
                }
            }
            else
            {
                _logger.LogDebug("      ⚠️ Üres sor {Row}, átugorva", row);
            }
        }
        
        // ✅ Visszadjuk a felismert fejlécet
        return fejlecOszlopok.Any() ? fejlecOszlopok : null;
    }

    private void FeldolgozTeljesSzoveg(string content, JegyzokonyvImportAdatok adatok)
    {
        _logger.LogInformation("📝 Teljes szöveg elemzése regex-szel...");

        var megrendeloMatch = Regex.Match(content, @"Megrendelő:\s*(.+?)(?:\n|Tel|Email)", RegexOptions.IgnoreCase);
        if (megrendeloMatch.Success)
        {
            adatok.Megrendelo = megrendeloMatch.Groups[1].Value.Trim();
            _logger.LogInformation("  📋 Megrendelő: {Megrendelo}", adatok.Megrendelo);
        }

        var munkaszamMatch = Regex.Match(content, @"Munkaszám:\s*([A-Z0-9\-]+)", RegexOptions.IgnoreCase);
        if (munkaszamMatch.Success)
        {
            adatok.JegyzokonyvSzam = munkaszamMatch.Groups[1].Value.Trim();
            _logger.LogInformation("  📄 Munkaszám: {Munkaszam}", adatok.JegyzokonyvSzam);
        }

        var helyMatch = Regex.Match(content, @"(?:Vizsgálat helye|Telephely):\s*(.+?)(?:\n|$)", RegexOptions.IgnoreCase);
        if (helyMatch.Success)
        {
            adatok.VizsgalatHelye = helyMatch.Groups[1].Value.Trim();
            _logger.LogInformation("  📍 Vizsgálat helye: {Hely}", adatok.VizsgalatHelye);
        }

        FeldolgozEszkozokRegexSzel(content, adatok);
    }

    // ✅ ÚJ: Oldal-alapú feldolgozás
    private void FeldolgozOldalanként(AnalyzeResult result, JegyzokonyvImportAdatok adatok)
    {
        _logger.LogInformation("📄 Oldal-alapú feldolgozás {PageCount} oldalon", result.Pages.Count);
        
        foreach (var page in result.Pages)
        {
            _logger.LogInformation("  📄 Oldal {PageNumber} feldolgozása...", page.PageNumber);
            
            // Összeállítjuk az oldal szövegét a sorokból
            var oldalSzoveg = string.Join("\n", page.Lines.Select(l => l.Content));
            
            // Eszközök keresése ezen az oldalon
            FeldolgozEszkozokRegexSzel(oldalSzoveg, adatok);
            
            // Egyéb mezők keresése
            FeldolgozMetaadatokOldalrol(oldalSzoveg, adatok, page.PageNumber);
        }
    }

    private void FeldolgozMetaadatokOldalrol(string oldalSzoveg, JegyzokonyvImportAdatok adatok, int oldalSzam)
    {
        // Dolgozó neve (általában 3-4. oldal)
        if (string.IsNullOrEmpty(adatok.DolgozoNeve))
        {
            var dolgozoMatch = Regex.Match(oldalSzoveg, @"(?:Dolgozó|Munkatárs):\s*(.+?)(?:\n|$)", RegexOptions.IgnoreCase);
            if (dolgozoMatch.Success)
            {
                adatok.DolgozoNeve = dolgozoMatch.Groups[1].Value.Trim();
                _logger.LogInformation("    ✅ Dolgozó (oldal {Page}): {Nev}", oldalSzam, adatok.DolgozoNeve);
            }
        }
        
        // Rendszám
        if (string.IsNullOrEmpty(adatok.ForgalmiRendszam))
        {
            var rendszamMatch = Regex.Match(oldalSzoveg, @"(?:Rendszám|Forgalmi):\s*([A-Z]{3}-\d{3})", RegexOptions.IgnoreCase);
            if (rendszamMatch.Success)
            {
                adatok.ForgalmiRendszam = rendszamMatch.Groups[1].Value.Trim();
                _logger.LogInformation("    ✅ Rendszám (oldal {Page}): {Rendszam}", oldalSzam, adatok.ForgalmiRendszam);
            }
        }
    }

    // ✅ JAVÍTÁS 1: Regex alapú feldolgozás - NE hardcode-oljuk a "Mérőműszer" nevet
    private void FeldolgozEszkozokRegexSzel(string content, JegyzokonyvImportAdatok adatok)
    {
        var pattern1 = @"Típus:\s*([A-Z0-9\-]+)\s+Gyári szám:\s*(\d+)";
        var matches1 = Regex.Matches(content, pattern1, RegexOptions.IgnoreCase);
        
        foreach (Match match in matches1)
        {
            var tipus = match.Groups[1].Value.Trim();
            var eszkoz = new HordozhatoEszkozImport
            {
                Tipus = tipus,
                GyariSzam = match.Groups[2].Value.Trim(),
                Eszkoznev = tipus // ✅ JAVÍTVA: Használjuk a típust, NE "Mérőműszer (típus)"
            };
            
            if (!adatok.Eszkozok.Any(e => e.GyariSzam == eszkoz.GyariSzam))
            {
                adatok.Eszkozok.Add(eszkoz);
                _logger.LogInformation("  🔧 Eszköz regex-ből: {Nev} (Típus: {Tipus}, Gyári: {Gyari})", 
                    eszkoz.Eszkoznev, eszkoz.Tipus, eszkoz.GyariSzam);
            }
        }
    }

    private void FeldolgozHierarchikusSzoveg(string content, JegyzokonyvImportAdatok adatok)
    {
        var hierarchikusPattern = @"(?<eszkoz>[^\d\n]+?)\s*[\+\-]?\s*(?<darab>\d+)\s*(?:x|db)?\s*(?<alkatresz>[^\n]+)";
        var matches = Regex.Matches(content, hierarchikusPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        int sorszam = adatok.Eszkozok.Count + 1;

        foreach (Match match in matches)
        {
            var eszkozNev = match.Groups["eszkoz"].Value.Trim();
            var darabszamStr = match.Groups["darab"].Value;
            var alkatreszNev = match.Groups["alkatresz"].Value.Trim();

            if (!int.TryParse(darabszamStr, out int darabszam) || darabszam <= 0 || darabszam > 20)
                continue;

            var csoportNev = $"{eszkozNev}-{sorszam}";
            adatok.Eszkozok.Add(new HordozhatoEszkozImport
            {
                Sorszam = sorszam.ToString(),
                Eszkoznev = eszkozNev,
                CsoportNev = csoportNev,
                CsoportSorrend = 0
            });

            for (int i = 1; i <= darabszam; i++)
            {
                adatok.Eszkozok.Add(new HordozhatoEszkozImport
                {
                    Sorszam = "",
                    Eszkoznev = alkatreszNev,
                    CsoportNev = csoportNev,
                    CsoportSorrend = i
                });
            }

            _logger.LogInformation("🔗 Hierarchikus eszköz létrehozva: {Eszkoz} + {Darabszam}x {Alkatresz}", 
                eszkozNev, darabszam, alkatreszNev);

            sorszam++;
        }
    }

    private void FeldolgozHierarchikusTablazat(IReadOnlyList<DocumentTable> tables, JegyzokonyvImportAdatok adatok)
    {
        _logger.LogInformation("⚠️ Hierarchikus táblázat feldolgozás még nem implementált");
    }

    private void LogTalaltAdatok(JegyzokonyvImportAdatok adatok)
    {
        _logger.LogInformation("=== TALÁLAT ÖSSZESÍTŐ ===");
        _logger.LogInformation("Jegyzőkönyv szám: {JkSzam}", adatok.JegyzokonyvSzam ?? "(üres)");
        _logger.LogInformation("Dolgozó neve: {Dolgozo}", adatok.DolgozoNeve ?? "(üres)");
        _logger.LogInformation("Rendszám: {Rendszam}", adatok.ForgalmiRendszam ?? "(üres)");
        _logger.LogInformation("Megrendelő: {Megrendelo}", adatok.Megrendelo ?? "(üres)");
        _logger.LogInformation("Vizsgálat helye: {Hely}", adatok.VizsgalatHelye ?? "(üres)");
        _logger.LogInformation("Eszközök száma: {Count}", adatok.Eszkozok.Count);
        
        if (adatok.Eszkozok.Any())
        {
            _logger.LogInformation("Eszközök listája:");
            foreach (var eszkoz in adatok.Eszkozok)
            {
                _logger.LogInformation("  - {Nev} | Típus: {Tipus} | Gyári: {Gyari}", 
                    eszkoz.Eszkoznev ?? "(névtelen)", 
                    eszkoz.Tipus ?? "-", 
                    eszkoz.GyariSzam ?? "-");
            }
        }
        _logger.LogInformation("========================");
    }

    // ✅ ÚJ HELPER METÓDUS - Add hozzá!
    private int? TalalOszlop(Dictionary<int, string> fejlecek, params string[] kulcsszavak)
    {
        foreach (var (colIndex, fejlec) in fejlecek)
        {
            foreach (var kulcs in kulcsszavak)
            {
                if (fejlec.Contains(kulcs, StringComparison.OrdinalIgnoreCase))
                {
                    return colIndex;
                }
            }
        }
        return null;
    }

    // Hívd meg FeldolgozEszkozTablazat ELŐTT, ha AVK model
    private bool FeldolgozAvkTablazat(IReadOnlyList<DocumentTable> tables, JegyzokonyvImportAdatok adatok)
    {
        foreach (var table in tables)
        {
            // AVK fejléc felismerése: IΔn + t[ms] + MINŐSÍTÉS oszlopok
            var fejlecSor = table.Cells
                .Where(c => c.RowIndex == 0)
                .OrderBy(c => c.ColumnIndex)
                .Select(c => c.Content?.Trim() ?? "")
                .ToList();

            bool isAvkTabla = fejlecSor.Any(h =>
                h.Contains("IAn", StringComparison.OrdinalIgnoreCase) ||
                h.Contains("IΔn", StringComparison.OrdinalIgnoreCase) ||
                h.Contains("MINŐSÍTÉS", StringComparison.OrdinalIgnoreCase));

            if (!isAvkTabla) continue;

            _logger.LogInformation("✅ AVK táblázat felismerve, {RowCount} sorral", table.RowCount);

            // Oszlop indexek a fejlécből
            int? jelCol = null, helyeCol = null, tipusCol = null,
                 inCol = null, idnCol = null, unCol = null, polusCol = null,
                 idnMertCol = null, t1Col = null, t5Col = null, eredmenyCol = null;

            for (int c = 0; c < fejlecSor.Count; c++)
            {
                var h = fejlecSor[c].ToLower();
                if (h.Contains("jele"))                                   jelCol = c;
                else if (h.Contains("helye"))                             helyeCol = c;
                else if (h.Contains("típus") || h.Contains("tipus"))     tipusCol = c;
                else if (h == "in [a]" || h == "in[a]")                  inCol = c;
                else if (h.Contains("névl") || h == "ian [ma]")          idnCol = c;
                else if (h.Contains("un") || h.Contains("[v]"))          unCol = c;
                else if (h.Contains("pólus") || h.Contains("polus"))     polusCol = c;
                else if (h.Contains("mért") || h.Contains("mert"))       idnMertCol = c;
                else if (h.Contains("ian") && h.Contains("t [ms]"))      t1Col = c;
                else if (h.Contains("5x") || h.Contains("5xian"))        t5Col = c;
                else if (h.Contains("minős") || h.Contains("eredmény"))  eredmenyCol = c;
            }

            // Ha t1 és t5 nem különült el, pozíció alapján:
            // [9]= t1×, [10]= t5×
            t1Col ??= fejlecSor.Count > 9 ? 9 : null;
            t5Col ??= fejlecSor.Count > 10 ? 10 : null;
            idnMertCol ??= fejlecSor.Count > 8 ? 8 : null;

            for (int row = 1; row < table.RowCount; row++)
            {
                var cellak = table.Cells
                    .Where(c => c.RowIndex == row)
                    .OrderBy(c => c.ColumnIndex)
                    .ToDictionary(c => c.ColumnIndex, c => c.Content?.Trim() ?? "");

                if (!cellak.Any(kv => !string.IsNullOrWhiteSpace(kv.Value))) continue;

                string Get(int? col) => col.HasValue && cellak.TryGetValue(col.Value, out var v) ? v : "";

                // Sorszámsor átugrása
                var ssz = Get(0);
                if (ssz == "Ssz." || ssz == "#") continue;

                var avkSor = new AvkSor
                {
                    Sorsz      = int.TryParse(ssz.TrimEnd('.'), out var s) ? s : (adatok.AvkSorok.Count + 1),
                    Jele       = Get(jelCol),
                    Helye      = Get(helyeCol),
                    TipusNev   = Get(tipusCol),
                    In         = Get(inCol),
                    IDn        = Get(idnCol),
                    Un         = Get(unCol),
                    Polusszam  = Get(polusCol),
                    IDnMert    = Get(idnMertCol),
                    T1x        = Get(t1Col),
                    T5x        = Get(t5Col),
                    Eredmeny   = Get(eredmenyCol) switch
                    {
                        var e when e.Contains("MEGFELEL") && !e.Contains("NEM") => "MF",
                        var e when e.Contains("NEM") => "NMF",
                        _ => "MF"
                    }
                };

                if (!string.IsNullOrWhiteSpace(avkSor.Jele) ||
                    !string.IsNullOrWhiteSpace(avkSor.TipusNev))
                {
                    adatok.AvkSorok.Add(avkSor);
                    _logger.LogInformation("  ✅ AVK sor: {Jele} | {Tipus} | IDn={IDn} | t1={T1} | t5={T5} | {Eredmeny}",
                        avkSor.Jele, avkSor.TipusNev, avkSor.IDnMert, avkSor.T1x, avkSor.T5x, avkSor.Eredmeny);
                }
            }

            return adatok.AvkSorok.Any();
        }
        return false;
    }

    private bool FeldolgozMeresiPontTablazat(IReadOnlyList<DocumentTable> tables, JegyzokonyvImportAdatok adatok)
    {
        foreach (var table in tables)
        {
            var fejlecSor = table.Cells
                .Where(c => c.RowIndex == 0)
                .OrderBy(c => c.ColumnIndex)
                .Select(c => c.Content?.Trim().ToLowerInvariant() ?? "")
                .ToList();

            // HVM mérési pont táblázat felismerése: tartalmaz "mérési pont" vagy "hurokimpedancia" vagy "túláramvédelem" fejlécet
            bool isHvmTabla = fejlecSor.Any(h =>
                h.Contains("mérési pont") || h.Contains("meresi pont") ||
                h.Contains("hurokimpedancia") || h.Contains("hurok") ||
                h.Contains("túláramvédelem") || h.Contains("tularamvedelem") ||
                h.Contains("pe foly") || h.Contains("pefoly"));

            if (!isHvmTabla)
                continue;

            _logger.LogInformation("✅ HVM mérési pont táblázat felismerve ({OszlopSzam} oszlop, {SorSzam} sor)",
                table.ColumnCount, table.RowCount);

            // Fejléc oszlopok indexének meghatározása
            int colHely = -1, colModszer = -1, colTularamHely = -1, colTularamTipus = -1;
            int colPeFoly = -1, colErtek = -1, colMinosites = -1, colMegjegyzes = -1;

            for (int i = 0; i < fejlecSor.Count; i++)
            {
                var h = fejlecSor[i];
                if (h.Contains("mérési pont") || h.Contains("meresi pont") || h.Contains("hely") && colHely < 0) colHely = i;
                else if (h.Contains("módszer") || h.Contains("modszer") || h == "m" || h.Contains("mérési módszer")) colModszer = i;
                else if ((h.Contains("helye") || h.Contains("táblaszám")) && colTularamHely < 0) colTularamHely = i;
                else if (h.Contains("típus") || h.Contains("tipus") || h.Contains("karak")) colTularamTipus = i;
                else if (h.Contains("pe foly") || h.Contains("pefoly") || h.Contains("védővezetők")) colPeFoly = i;
                else if (h.Contains("érték") || h.Contains("ertek") || h.Contains("hurok") || h.Contains("zs")) colErtek = i;
                else if (h.Contains("minős") || h.Contains("minős") || h.Contains("eredm")) colMinosites = i;
                else if (h.Contains("megjegyz")) colMegjegyzes = i;
            }

            // Ha nincs egyértelmű hely-oszlop, próbáljuk az első oszlopot
            if (colHely < 0) colHely = 0;

            int sorszam = 1;
            for (int rowIdx = 1; rowIdx < table.RowCount; rowIdx++)
            {
                var cellak = table.Cells
                    .Where(c => c.RowIndex == rowIdx)
                    .OrderBy(c => c.ColumnIndex)
                    .ToList();

                if (!cellak.Any()) continue;

                string Cella(int col) => col >= 0
                    ? cellak.FirstOrDefault(c => c.ColumnIndex == col)?.Content?.Trim() ?? ""
                    : "";

                var hely = Cella(colHely);
                if (string.IsNullOrWhiteSpace(hely)) continue;

                var sor = new MeresiPontSor
                {
                    Sorszam           = sorszam++,
                    MeresiPontHelye   = hely,
                    Modszer           = Cella(colModszer),
                    TularamvedelemHelye  = Cella(colTularamHely),
                    TularamvedelemTipusa = Cella(colTularamTipus),
                    PEFolytOhm        = Cella(colPeFoly),
                    ErtekOhm          = Cella(colErtek),
                    Minosites         = string.IsNullOrWhiteSpace(Cella(colMinosites)) ? "MEGFELELT" : Cella(colMinosites),
                    Megjegyzes        = Cella(colMegjegyzes),
                };

                // Hurokimpedancia numerikus parse
                if (decimal.TryParse(sor.ErtekOhm.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var zsVal))
                {
                    sor.MertHurokimpedancia = zsVal;
                }

                adatok.MeresiPontok.Add(sor);
                _logger.LogInformation("  ✅ Mérési pont: {Hely} | Zs={Ertek} | {Min}",
                    sor.MeresiPontHelye, sor.ErtekOhm, sor.Minosites);
            }
        }

        return adatok.MeresiPontok.Any();
    }
}

/// <summary>Akkor aktív, ha nincs Azure Document Intelligence API kulcs konfigurálva.</summary>
public class DisabledOcrService : IJegyzokonyvOcrService
{
    public Task<JegyzokonyvImportAdatok> FeldolgozAsync(Stream fileStream, string fileName, string? customModelId = null)
        => throw new InvalidOperationException("Az OCR szolgáltatás nincs konfigurálva. Állítsd be az AzureDocumentIntelligence:ApiKey értékét.");

    public Task<JegyzokonyvImportAdatok> FeldolgozHierarchikusAsync(Stream fileStream, string fileName, string? customModelId = null)
        => throw new InvalidOperationException("Az OCR szolgáltatás nincs konfigurálva. Állítsd be az AzureDocumentIntelligence:ApiKey értékét.");
}
