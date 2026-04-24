namespace BiztvillCRM.Services.Interfaces;

public interface IMunkaszamService
{
    /// <summary>
    /// Generál egy új egyedi munkaszámot a megadott céghez.
    /// Formátum: HK-XXXXXX/YYYY (pl. HK-000001/2026)
    /// </summary>
    Task<string> GeneralKovetkezoMunkaszamAsync(int cegId, int meresTipusId = 0);
}
