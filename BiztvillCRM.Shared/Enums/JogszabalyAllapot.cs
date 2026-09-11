namespace BiztvillCRM.Shared.Enums;

/// <summary>
/// Egy jogszabály/szabvány és egy méréstípus közötti hozzárendelés 3-állapotú jelölése.
/// Használatos a Jogszabalyok.razor és a MeresTipusKovetelemeny.razor admin oldalakon is,
/// mindkettő ugyanazt a MeresTipusJogszabaly kapcsolótáblát írja/olvassa.
/// </summary>
public enum JogszabalyAllapot
{
    /// <summary>Nincs hozzárendelve a méréstípushoz - nem jelenik meg a jegyzőkönyvben.</summary>
    NemJelenikMeg,

    /// <summary>Hozzárendelve, a jegyzőkönyv írásakor alapból kijelölve.</summary>
    AlapKijelolve,

    /// <summary>Hozzárendelve, de a jegyzőkönyv írásakor alapból nincs kijelölve.</summary>
    AlapNincsKijelolve
}
