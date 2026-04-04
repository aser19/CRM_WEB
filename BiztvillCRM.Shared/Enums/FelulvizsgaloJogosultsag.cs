namespace BiztvillCRM.Shared.Enums;

/// <summary>
/// Felülvizsgáló jogosultsági szintje.
/// Meghatározza, hogy milyen szerepkört tölthet be a vizsgálatnál.
/// </summary>
public enum FelulvizsgaloJogosultsag
{
    /// <summary>Csak segítő felülvizsgálóként dolgozhat</summary>
    Segito = 1,
    
    /// <summary>Felelős felülvizsgálóként és segítőként is dolgozhat</summary>
    Felelos = 2,
    
    /// <summary>Ellenőrként, felelős felülvizsgálóként és segítőként is dolgozhat</summary>
    Ellenor = 3
}