using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using BiztvillCRM.Data;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace BiztvillCRM.Services;

public class NavAdoszamService : INavAdoszamService
{
    private readonly IDbContextFactory<CrmDbContext> _dbFactory;
    private readonly HttpClient _http;
    private readonly ILogger<NavAdoszamService> _logger;

    private const string NavTestUrl = "https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryTaxpayer";
    private const string NavElesUrl = "https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryTaxpayer";

    public NavAdoszamService(IDbContextFactory<CrmDbContext> dbFactory, HttpClient http, ILogger<NavAdoszamService> logger)
    {
        _dbFactory = dbFactory;
        _http      = http;
        _logger    = logger;
    }

    public async Task<NavAdoszamEredmeny> LekerdezesByAdoszamAsync(string adoszam, int cegId)
    {
        var tisztaAdoszam = new string(adoszam.Where(char.IsDigit).ToArray());
        if (tisztaAdoszam.Length < 8)
            return Hiba("Érvénytelen adószám formátum. Legalább 8 számjegy szükséges.");

        await using var db = await _dbFactory.CreateDbContextAsync();
        var ceg = await db.Cegek.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cegId);

        if (ceg is null)
            return Hiba("A cég nem található.");

        if (string.IsNullOrWhiteSpace(ceg.NavLoginName)  ||
            string.IsNullOrWhiteSpace(ceg.NavPassword)   ||
            string.IsNullOrWhiteSpace(ceg.NavXmlSignKey) ||
            string.IsNullOrWhiteSpace(ceg.NavTaxNumber))
            return Hiba("A NAV API nincs beállítva. Kérjük töltse ki a cég NAV beállításait.");

        var url              = ceg.NavTesztKornyezet ? NavTestUrl : NavElesUrl;
        var requestId        = "QUERY" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var timestamp        = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // milliszekundummal!
        var loginName    = ceg.NavLoginName.Trim();
        var password     = ceg.NavPassword.Trim();
        var xmlSignKey   = ceg.NavXmlSignKey.Trim();
        var taxNumber    = ceg.NavTaxNumber.Trim();

        var passwordHash     = Sha512Hex(password);
        var requestSignature = Sha3_512Hex(requestId + timestamp + xmlSignKey);

        // === DIAGNÓZIS ===
        var logFajl = Path.Combine(Path.GetTempPath(), "nav_debug.xml");
        var signKeyBytes = Encoding.UTF8.GetBytes(xmlSignKey);
        var rawSignBytes = SHA3_512.HashData(Encoding.UTF8.GetBytes(requestId + timestamp + xmlSignKey));
        var diagInfo = $"""
            SHA3_512.IsSupported: {SHA3_512.IsSupported}
            URL: {url}
            NavTesztKornyezet: {ceg.NavTesztKornyezet}
            Login: '{loginName}' ({loginName.Length} kar)
            Password: '{password}' ({password.Length} kar)
            TaxNumber: '{taxNumber}' ({taxNumber.Length} kar)
            XmlSignKey: '{xmlSignKey}' ({xmlSignKey.Length} kar / {signKeyBytes.Length} byte)
            XmlSignKey HEX: {Convert.ToHexString(signKeyBytes)}
            PasswordHash ({passwordHash.Length} kar): {passwordHash}
            SignatureInput: {requestId + timestamp + xmlSignKey}
            RequestSignature ({requestSignature.Length} kar): {requestSignature}
            RequestSignature byte count: {rawSignBytes.Length}
            RequestSignature HEX manual: {string.Concat(rawSignBytes.Select(b => b.ToString("X2")))}
            RequestSignature via Convert: {Convert.ToHexString(rawSignBytes)}
            Lengths equal: {string.Concat(rawSignBytes.Select(b => b.ToString("X2"))).Length == Convert.ToHexString(rawSignBytes).Length}
            """;
        await File.WriteAllTextAsync(logFajl, diagInfo);
        // =================

        var xml = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <QueryTaxpayerRequest xmlns="http://schemas.nav.gov.hu/OSA/3.0/api"
                                  xmlns:common="http://schemas.nav.gov.hu/NTCA/1.0/common">
              <common:header>
                <common:requestId>{requestId}</common:requestId>
                <common:timestamp>{timestamp}</common:timestamp>
                <common:requestVersion>3.0</common:requestVersion>
                <common:headerVersion>1.0</common:headerVersion>
              </common:header>
              <common:user>
                <common:login>{loginName}</common:login>
                <common:passwordHash cryptoType="SHA-512">{passwordHash}</common:passwordHash>
                <common:taxNumber>{taxNumber}</common:taxNumber>
                <common:requestSignature cryptoType="SHA3-512">{requestSignature}</common:requestSignature>
              </common:user>
              <software>
                <softwareId>BIZTVILLCRM-000001</softwareId>
                <softwareName>BiztvillCRM</softwareName>
                <softwareOperation>LOCAL_SOFTWARE</softwareOperation>
                <softwareMainVersion>1.0</softwareMainVersion>
                <softwareDevName>Biztovill</softwareDevName>
                <softwareDevContact>info@biztovill.hu</softwareDevContact>
                <softwareDevCountryCode>HU</softwareDevCountryCode>
                <softwareDevTaxNumber>{taxNumber}</softwareDevTaxNumber>
              </software>
              <taxNumber>{tisztaAdoszam[..8]}</taxNumber>
            </QueryTaxpayerRequest>
            """;

        try
        {
            var content   = new StringContent(xml, Encoding.UTF8, "application/xml");
            var response  = await _http.PostAsync(url, content);
            var xmlValasz = await response.Content.ReadAsStringAsync();

            // === IDEIGLENES DIAGNÓZIS – töröld éles előtt! ===
            logFajl = Path.Combine(Path.GetTempPath(), "nav_debug.xml");
            await File.WriteAllTextAsync(logFajl,
                $"{diagInfo}\n\n=== KÉRÉS ===\n{xml}\n\n=== VÁLASZ ({(int)response.StatusCode}) ===\n{xmlValasz}");
            // =================================================

            return ParseValasz(xmlValasz);
        }
        catch (Exception ex)
        {
            return Hiba($"Kapcsolati hiba: {ex.Message}");
        }
    }

    private static NavAdoszamEredmeny ParseValasz(string responseXml)
    {
        try
        {
            XNamespace api = "http://schemas.nav.gov.hu/OSA/3.0/api";
            var doc  = XDocument.Parse(responseXml);
            var root = doc.Root;

            var funcCode = root?.Descendants(api + "funcCode").FirstOrDefault()?.Value;
            if (funcCode != "OK")
            {
                // Összes hibaüzenet összegyűjtése
                var uzeneteket = root?.Descendants(api + "notification")
                    .Select(n => $"[{n.Element(api + "notificationCode")?.Value}] {n.Element(api + "notificationText")?.Value}")
                    .ToList();

                var msg = uzeneteket?.Any() == true
                    ? string.Join(" | ", uzeneteket)
                    : root?.Descendants(api + "message").FirstOrDefault()?.Value
                      ?? $"NAV hiba (funcCode: {funcCode ?? "?"})";

                return Hiba(msg);
            }

            var taxpayer = root?.Descendants(api + "taxpayerData").FirstOrDefault();
            if (taxpayer is null)
                return Hiba("Az adószám nem található a NAV rendszerben.");

            var szekhely = taxpayer
                .Descendants(api + "taxpayerAddressItem")
                .FirstOrDefault(x => x.Element(api + "taxpayerAddressType")?.Value == "HQ");

            return new NavAdoszamEredmeny
            {
                Sikeres           = true,
                CegNev            = taxpayer.Element(api + "taxpayerName")?.Value,
                IranyitoSzam      = szekhely?.Element(api + "postalCode")?.Value,
                Telepules         = szekhely?.Element(api + "city")?.Value,
                Kozterulet        = szekhely?.Element(api + "streetName")?.Value,
                KozteruletJellege = szekhely?.Element(api + "publicPlaceCategory")?.Value,
                Hazszam           = szekhely?.Element(api + "number")?.Value,
            };
        }
        catch (Exception ex)
        {
            return Hiba($"XML feldolgozási hiba: {ex.Message}");
        }
    }

    private static NavAdoszamEredmeny Hiba(string uzenet) =>
        new() { Sikeres = false, HibaSzoveg = uzenet };

    private static string Sha512Hex(string input)
    {
        var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes); // UPPERCASE
    }

    private static string Sha3_512Hex(string input)
    {
        var bytes = SHA3_512.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes); // UPPERCASE
    }
}