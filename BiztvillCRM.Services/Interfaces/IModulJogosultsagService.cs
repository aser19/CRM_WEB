using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Services.Interfaces;

public interface IModulJogosultsagService
{
    /// <summary>
    /// Inicializálja a modul jogosultságokat egyetlen adatbázis-hívással.
    /// </summary>
    Task InitializeAsync();
    
    bool HasModule(ModulJogosultsag modul);
    Task<bool> HasModuleAsync(ModulJogosultsag modul);
    ModulJogosultsag GetAktivModulok();
}