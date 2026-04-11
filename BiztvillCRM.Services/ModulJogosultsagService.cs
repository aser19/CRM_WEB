using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Services;

public class ModulJogosultsagService : IModulJogosultsagService
{
    private readonly ITenantService _tenantService;
    private readonly ICegService _cegService;
    
    // Cache a modul jogosultságokhoz
    private ModulJogosultsag? _cachedModulok;
    private bool _isInitialized;

    public ModulJogosultsagService(ITenantService tenantService, ICegService cegService)
    {
        _tenantService = tenantService;
        _cegService = cegService;
    }

    /// <summary>
    /// Inicializálja a modul jogosultságokat egyetlen adatbázis-hívással.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        var cegId = _tenantService.GetCurrentCegId();
        if (cegId != 0)
        {
            var ceg = await _cegService.GetByIdAsync(cegId);
            _cachedModulok = ceg?.AktivModulok ?? ModulJogosultsag.Nincs;
        }
        else
        {
            _cachedModulok = ModulJogosultsag.Nincs;
        }
        
        _isInitialized = true;
    }

    public bool HasModule(ModulJogosultsag modul)
    {
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            return true;

        if (!_isInitialized)
            throw new InvalidOperationException("ModulJogosultsagService nincs inicializálva. Hívd meg az InitializeAsync()-et először.");

        return _cachedModulok?.HasFlag(modul) ?? false;
    }

    public async Task<bool> HasModuleAsync(ModulJogosultsag modul)
    {
        if (_tenantService.IsInRole(FelhasznaloSzerepkor.Admin))
            return true;

        await InitializeAsync();
        return _cachedModulok?.HasFlag(modul) ?? false;
    }

    public ModulJogosultsag GetAktivModulok()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("ModulJogosultsagService nincs inicializálva.");
            
        return _cachedModulok ?? ModulJogosultsag.Nincs;
    }
}