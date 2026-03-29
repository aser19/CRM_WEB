using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Services.Interfaces;

/// <summary>Aktuális felhasználó és cég (tenant) adatainak szolgáltatása.</summary>
public interface ITenantService
{
    /// <summary>Aktuális felhasználó cég ID-ja.</summary>
    int GetCurrentCegId();

    /// <summary>Aktuális felhasználó ID-ja.</summary>
    string? GetCurrentUserId();

    /// <summary>Aktuális felhasználó neve.</summary>
    string? GetCurrentUserName();

    /// <summary>Ellenőrzi, hogy a felhasználónak van-e adott szerepköre.</summary>
    bool IsInRole(FelhasznaloSzerepkor role);
}