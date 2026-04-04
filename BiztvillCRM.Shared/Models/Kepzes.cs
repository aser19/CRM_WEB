using BiztvillCRM.Shared.Enums;

namespace BiztvillCRM.Shared.Models;

/// <summary>Felülvizsgáló képzése/bizonyítványa</summary>
public class Kepzes
{
    public int Id { get; set; }
    public DateTime Letrehozva { get; set; }
    public DateTime? Modositva { get; set; }

    public string Nev { get; set; } = string.Empty;
    public FelulvizsgaloJogosultsag Jogosultsag { get; set; } = FelulvizsgaloJogosultsag.Segito;
    
    // Bizonyítvány típus
    public int? KepzesTipusId { get; set; }
    public KepzesTipus? KepzesTipus { get; set; }
    
    // Alapbizonyítvány
    public string? BizonyitvanySzam { get; set; }
    public DateTime? BizonyitvanyKelte { get; set; }
    public DateTime? BizonyitvanyLejarat { get; set; }
    
    // Továbbképzés
    public string? TovabbkepzesSzam { get; set; }
    public DateTime? UtolsoTovabbkepzes { get; set; }
    
    // Régi mezők kompatibilitásra
    public string? FelujtoKepzesSzam { get; set; }
    public DateTime? KepzesLejarat { get; set; }
    
    public string? Megjegyzes { get; set; }
    public bool Aktiv { get; set; } = true;

    // Cég kapcsolat
    public int CegId { get; set; }
    public Ceg? Ceg { get; set; }
    
    // Számított tulajdonságok
    public bool LehetFelelosFelulvizsgalo => Jogosultsag >= FelulvizsgaloJogosultsag.Felelos;
    public bool LehetSegito => true;
    public bool LehetEllenor => Jogosultsag >= FelulvizsgaloJogosultsag.Ellenor;
}
