using MudBlazor;

namespace BiztvillCRM.Web.Helpers;

/// <summary>
/// A súgó kategóriákhoz adatbázisban tárolt ikon-kulcsokat (pl. "Business", "Speed")
/// képezi le a megfelelő MudBlazor Material ikon SVG path-ra.
/// </summary>
public static class SugoIkonTerkep
{
    public static readonly Dictionary<string, string> Ikonok = new()
    {
        ["Dashboard"] = Icons.Material.Filled.Dashboard,
        ["Business"] = Icons.Material.Filled.Business,
        ["Speed"] = Icons.Material.Filled.Speed,
        ["VerifiedUser"] = Icons.Material.Filled.VerifiedUser,
        ["HealthAndSafety"] = Icons.Material.Filled.HealthAndSafety,
        ["Engineering"] = Icons.Material.Filled.Engineering,
        ["Gavel"] = Icons.Material.Filled.Gavel,
        ["Assignment"] = Icons.Material.Filled.Assignment,
        ["Analytics"] = Icons.Material.Filled.Analytics,
        ["Settings"] = Icons.Material.Filled.Settings,
        ["AdminPanelSettings"] = Icons.Material.Filled.AdminPanelSettings,
        ["Build"] = Icons.Material.Filled.Build,
        ["Help"] = Icons.Material.Filled.Help,
        ["School"] = Icons.Material.Filled.School,
    };

    public static string Feloldas(string? ikonKulcs) =>
        !string.IsNullOrWhiteSpace(ikonKulcs) && Ikonok.TryGetValue(ikonKulcs, out var ikon)
            ? ikon
            : Icons.Material.Filled.Help;
}
