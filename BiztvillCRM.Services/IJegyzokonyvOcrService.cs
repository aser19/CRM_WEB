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
                
                _logger.LogInformation("✅ Fallback model sikeresen használva");
            }

            _logger.LogInformation("📄 Talált oldalak: {PageCount}", result.Pages.Count);
            _logger.LogInformation("📋 Talált táblázatok: {TableCount}", result.Tables.Count);

            // ✅ Teljes szöveg feldolgozása (eredeti)
            FeldolgozTeljesSzoveg(result.Content, importAdatok);
            
            // ✅ ÚJ: Oldal-alapú feldolgozás (3-8. oldal adataihoz)
            FeldolgozOldalanként(result, importAdatok);

            // Táblázatok feldolgozása (minden oldalról)
            if (result.Tables.Count > 0)
            {
                _logger.LogInformation("🔍 Táblázatok feldolgozása...");
                FeldolgozEszkozTablazat(result.Tables, importAdatok);
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

            if (table.RowCount < 2)
            {
                _logger.LogWarning("⚠️ Táblázat #{Index} túl kicsi, átugorva", tableIndex + 1);
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
        
        // ✅ Fejléc azonosítása (első sor elemzése)
        var fejlecOszlopok = new Dictionary<int, string>();
        int startRow = 0; // Melyik sortól indulnak az adatok
        
        // ✅ Ha van előre megadott fejléc (2. táblázattól), használjuk azt
        if (eloredefinedFejlec != null && eloredefinedFejlec.Any())
        {
            fejlecOszlopok = eloredefinedFejlec;
            startRow = 0; // MINDEN sor adat
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
                    fejlecOszlopok[col] = fejlecCell.Content.Trim().ToLower();
                    _logger.LogDebug("    📋 Oszlop {Col}: {Header}", col, fejlecCell.Content);
                }
            }
            
            // ✅ Ellenőrzés: Van-e valódi fejléc?
            bool vanFejlec = fejlecOszlopok.Values.Any(h => 
                h.Contains("megnevezés") || 
                h.Contains("típus") || 
                h.Contains("gyári") ||
                h.Contains("eszköz"));
            
            if (vanFejlec)
            {
                startRow = 1; // 1-től indulnak az adatok
                _logger.LogInformation("    ✅ Fejléc felismerve, adatok: 1. sortól");
            }
            else
            {
                startRow = 0; // MINDEN sor adat (nincs fejléc)
                _logger.LogWarning("    ⚠️ Nincs fejléc! Minden sort adatként dolgozunk fel.");
                
                // Ha nincs fejléc, ne próbáljuk meg feldolgozni
                if (eloredefinedFejlec == null)
                {
                    _logger.LogWarning("    ⚠️ Első táblázat fejléc nélkül - oszloppozíció alapján dolgozunk");
                    // Alapértelmezett oszlop pozíciók (feltételezés)
                    fejlecOszlopok[1] = "megnevezés"; // 2. oszlop
                    fejlecOszlopok[2] = "típus";       // 3. oszlop
                    fejlecOszlopok[3] = "gyári szám";  // 4. oszlop
                }
            }
        }
        
        // ✅ Oszlopok automatikus felismerése - KIBŐVÍTVE!
        int? megnevezesCol = TalalOszlop(fejlecOszlopok, "megnevezés", "eszköz", "név");
        int? tipusCol = TalalOszlop(fejlecOszlopok, "típus", "type");
        int? gyariCol = TalalOszlop(fejlecOszlopok, "gyári", "serial", "szám");
        int? leltariCol = TalalOszlop(fejlecOszlopok, "leltári", "inventory");
        
        // ✅ ÚJ OSZLOPOK
        int? teljCol = TalalOszlop(fejlecOszlopok, "teljesítmény", "feszültség", "egyéb", "jellemző");
        int? megtekintCol = TalalOszlop(fejlecOszlopok, "megtekintés", "szemre", "vizuális");
        int? folytCol = TalalOszlop(fejlecOszlopok, "folytonosság", "folyt");
        int? szigellCol = TalalOszlop(fejlecOszlopok, "szigetel", "szig");
        int? szivargoCol = TalalOszlop(fejlecOszlopok, "szivárgó", "áram");
        int? megjegyzesCol = TalalOszlop(fejlecOszlopok, "megjegyzés", "jegyzet");

        _logger.LogInformation("    🔍 Felismert oszlopok: Név={Nev}, Típus={Tipus}, Gyári={Gyari}, Telj={Telj}, Megtekint={Megtekint}, Folyt={Folyt}, Szigell={Szigell}, Szivargo={Szivargo}", 
            megnevezesCol, tipusCol, gyariCol, teljCol, megtekintCol, folytCol, szigellCol, szivargoCol);
        
        // ✅ Adatsorok feldolgozása (startRow-tól)
        for (int row = startRow; row < table.RowCount; row++)
        {
            var cellak = table.Cells
                .Where(c => c.RowIndex == row)
                .OrderBy(c => c.ColumnIndex)
                .ToList();

            if (cellak.Count < 2) 
            {
                _logger.LogDebug("      ⚠️ Sor {Row} túl kevés cellát tartalmaz ({Count}), átugorva", 
                    row, cellak.Count);
                continue;
            }

            var eszkoz = new HordozhatoEszkozImport();
            var sorAdatok = new Dictionary<int, string>();
            
            // Cellák beolvasása
            foreach (var cell in cellak)
            {
                var content = cell.Content?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(content))
                {
                    sorAdatok[cell.ColumnIndex] = content;
                }
            }
            
            _logger.LogDebug("      📝 Sor {Row}: {Cellak} cella", row, sorAdatok.Count);
            
            // ✅ Adatok kinyerése felismert oszlopokból
            if (megnevezesCol.HasValue && sorAdatok.ContainsKey(megnevezesCol.Value))
            {
                eszkoz.Eszkoznev = sorAdatok[megnevezesCol.Value];
            }

            if (tipusCol.HasValue && sorAdatok.ContainsKey(tipusCol.Value))
            {
                eszkoz.Tipus = sorAdatok[tipusCol.Value];
            }
            
            if (gyariCol.HasValue && sorAdatok.ContainsKey(gyariCol.Value))
            {
                eszkoz.GyariSzam = sorAdatok[gyariCol.Value];
            }
            
            if (leltariCol.HasValue && sorAdatok.ContainsKey(leltariCol.Value))
            {
                eszkoz.Leltariszam = sorAdatok[leltariCol.Value];
            }

            // ✅ ÚJ MEZŐK KIOLVASÁSA
            if (teljCol.HasValue && sorAdatok.ContainsKey(teljCol.Value))
            {
                var telj = sorAdatok[teljCol.Value];
                // Normalizálás: "230 V" -> "230V", "230 V / 1010 W" -> "230V"
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
                // Normalizálás: "MEGFELELT" -> "MF", "NEM MEGFELELT" -> "NMF"
                if (megtekint.Contains("MEGFELELT") || megtekint == "MF")
                    eszkoz.Megtekintes = "MF";
                else if (megtekint.Contains("NEM") || megtekint == "NMF")
                    eszkoz.Megtekintes = "NMF";
                else if (megtekint.Contains("KSZ"))
                    eszkoz.Megtekintes = "KSZ"; // Külső szemrevétel
            }

            if (folytCol.HasValue && sorAdatok.ContainsKey(folytCol.Value))
            {
                var folyt = sorAdatok[folytCol.Value];
                // Szűrjük ki a "KSZ" és üres értékeket
                if (!string.IsNullOrWhiteSpace(folyt) && folyt != "KSZ" && folyt != "-")
                    eszkoz.Folytonossag = folyt;
            }

            if (szigellCol.HasValue && sorAdatok.ContainsKey(szigellCol.Value))
            {
                var szigell = sorAdatok[szigellCol.Value];
                // ">200" formátumot megtartjuk
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
                // "MEGFELELT" megjegyzést ne vegyük át
                if (!string.IsNullOrWhiteSpace(megjegyzes) && !megjegyzes.Contains("MEGFELELT"))
                    eszkoz.Megjegyzes = megjegyzes;
            }
            
            // ✅ FALLBACK: Ha nem találtunk oszlopokat, próbáljuk meg a "Típus:" "Gyári szám:" mintát
            if (string.IsNullOrEmpty(eszkoz.Tipus) && string.IsNullOrEmpty(eszkoz.GyariSzam))
            {
                foreach (var (colIndex, content) in sorAdatok)
                {
                    if (content.StartsWith("Típus:", StringComparison.OrdinalIgnoreCase))
                    {
                        eszkoz.Tipus = content.Replace("Típus:", "").Trim();
                    }
                    else if (content.StartsWith("Gyári szám:", StringComparison.OrdinalIgnoreCase))
                    {
                        eszkoz.GyariSzam = content.Replace("Gyári szám:", "").Trim();
                    }
                    else if (string.IsNullOrEmpty(eszkoz.Eszkoznev) && !content.Contains(":"))
                    {
                        eszkoz.Eszkoznev = content;
                    }
                }
            }
            
            // ✅ FALLBACK 2: Szűrjük ki a sorszámokat (pl. "24.", "25.")
            if (!string.IsNullOrEmpty(eszkoz.Eszkoznev) && Regex.IsMatch(eszkoz.Eszkoznev, @"^\d+\.$"))
            {
                _logger.LogDebug("      ⚠️ Csak sorszám ({Sorsz}), átugorva", eszkoz.Eszkoznev);
                continue; // Ne importáljuk a puszta sorszámokat
            }
            
            // ✅ FALLBACK 3: Ha még mindig nincs név, generáljunk egyet
            if (string.IsNullOrEmpty(eszkoz.Eszkoznev))
            {
                if (!string.IsNullOrEmpty(eszkoz.Tipus))
                {
                    eszkoz.Eszkoznev = $"Eszköz ({eszkoz.Tipus})";
                }
                else if (sorAdatok.Any())
                {
                    // Az első nem üres cella legyen a név (csak ha nem sorszám)
                    var elsoErtek = sorAdatok.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v) && !Regex.IsMatch(v, @"^\d+\.$"));
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
                // Duplikáció ellenőrzés
                bool marLetezik = adatok.Eszkozok.Any(e => 
                    (!string.IsNullOrEmpty(e.GyariSzam) && e.GyariSzam == eszkoz.GyariSzam) ||
                    (!string.IsNullOrEmpty(e.Eszkoznev) && e.Eszkoznev == eszkoz.Eszkoznev));
                
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
                    _logger.LogDebug("      ⚠️ Duplikáció, átugorva: {Nev}", eszkoz.Eszkoznev);
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

    private void FeldolgozEszkozokRegexSzel(string content, JegyzokonyvImportAdatok adatok)
    {
        var pattern1 = @"Típus:\s*([A-Z0-9\-]+)\s+Gyári szám:\s*(\d+)";
        var matches1 = Regex.Matches(content, pattern1, RegexOptions.IgnoreCase);
        
        foreach (Match match in matches1)
        {
            var eszkoz = new HordozhatoEszkozImport
            {
                Tipus = match.Groups[1].Value.Trim(),
                GyariSzam = match.Groups[2].Value.Trim(),
                Eszkoznev = $"Mérőműszer ({match.Groups[1].Value.Trim()})"
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
}
