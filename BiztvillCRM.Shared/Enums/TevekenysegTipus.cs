using System;

namespace BiztvillCRM.Shared.Enums;

/// <summary>
/// Tevékenységi kör típusok - Flags enum, hogy több is kiválasztható legyen.
/// </summary>
[Flags]
public enum TevekenysegTipus
{
    Nincs = 0,
    VillanosMeres = 1,
    Benzinkuttechnika = 2,
    Munkavédelem = 4
}