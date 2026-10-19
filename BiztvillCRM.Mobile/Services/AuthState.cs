namespace BiztvillCRM.Mobile.Services;

/// <summary>
/// Bejelentkezési állapot kezelése. A tokent és az alap felhasználói adatokat
/// SecureStorage-ban tároljuk (titkosított, platform szintű tárolás).
/// </summary>
public class AuthState
{
    private const string TokenKey = "auth_token";
    private const string TokenExpiresKey = "auth_token_expires";
    private const string UserNameKey = "auth_username";
    private const string TeljesNevKey = "auth_teljesnev";

    public string? Token { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public string? UserName { get; private set; }
    public string? TeljesNev { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token) && ExpiresAtUtc > DateTime.UtcNow;

    /// <summary>Betölti a mentett bejelentkezési állapotot (app indításkor hívjuk).</summary>
    public async Task BetoltAsync()
    {
        Token = await SecureStorage.Default.GetAsync(TokenKey);
        UserName = await SecureStorage.Default.GetAsync(UserNameKey);
        TeljesNev = await SecureStorage.Default.GetAsync(TeljesNevKey);

        var expiresRaw = await SecureStorage.Default.GetAsync(TokenExpiresKey);
        if (DateTime.TryParse(expiresRaw, out var expires))
            ExpiresAtUtc = DateTime.SpecifyKind(expires, DateTimeKind.Utc);
    }

    public async Task BejelentkezveAsync(string token, DateTime expiresAtUtc, string userName, string? teljesNev)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        UserName = userName;
        TeljesNev = teljesNev;

        SecureStorage.Default.Remove(TokenKey);
        await SecureStorage.Default.SetAsync(TokenKey, token);
        await SecureStorage.Default.SetAsync(TokenExpiresKey, expiresAtUtc.ToString("O"));
        await SecureStorage.Default.SetAsync(UserNameKey, userName);
        await SecureStorage.Default.SetAsync(TeljesNevKey, teljesNev ?? string.Empty);
    }

    /// <summary>
    /// Kijelentkezéskor minden, a felhasználóhoz tartozó adatot töröl:
    /// a SecureStorage-ban tárolt tokent, valamint a helyi offline cache-t (SQLite fájl).
    /// </summary>
    public void Kijelentkezes()
    {
        Token = null;
        ExpiresAtUtc = null;
        UserName = null;
        TeljesNev = null;

        SecureStorage.Default.RemoveAll();
    }
}
