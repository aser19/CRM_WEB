namespace BiztvillCRM.Services.Interfaces;

/// <summary>
/// Cégfüggő, automatikusan növekvő lyukadásjelző jegyzőkönyv-sorszámot generáló szolgáltatás.
/// </summary>
public interface ILyukadasjelzoSzamService
{
    /// <summary>
    /// Legenerálja a következő lyukadásjelző jegyzőkönyv sorszámát a megadott (kiállító) céghez.
    /// Formátum: LYUK-XXXXXX/YYYY (pl. LYUK-000001/2026).
    /// </summary>
    Task<string> GeneralKovetkezoSorszamAsync(int cegId);
}
